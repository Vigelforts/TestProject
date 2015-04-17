using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Paragon.Container.Core
{
    public sealed class PBase : MvxNotifyPropertyChanged
    {
        internal PBase(Models.PBase model)
        {
            _model = model;

            _userInteraction = Mvx.Resolve<IUserInteraction>();
            _fileDownloader = Mvx.Resolve<Common.IFileDownloader>();
            _resourcesProvider = Mvx.Resolve<Common.IResourcesProvider>();
            _uiDispatcher = Mvx.Resolve<Common.IUIDispatcher>();

            _fileDownloader.Started += FileDownloaderOnStarted;
            _fileDownloader.Progress += FileDownloaderOnProgress;
            _fileDownloader.Complited += FileDownloaderOnComplited;
            _fileDownloader.ErrorOccurred += FileDownloaderOnErrorOccurred;

            DownloadCommand = new Common.Mvvm.Command(DownloadCommandImpl, false);
            StopCommand = new Common.Mvvm.Command(StopCommandImpl, false);

            Initialize();
        }

        public event Action StateChanged;

        public PBaseTypes Type { get; private set; }
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    SetName();
                }

                return _name;
            }
        }
        public string Size { get; private set; }
        public bool IsDownloaded
        {
            get { return _isDownloaded; }
            private set
            {
                _isDownloaded = value;
                RaisePropertyChanged(() => IsDownloaded);
            }
        }
        public bool IsNotDownloaded
        {
            get { return _isNotDownloaded; }
            private set
            {
                _isNotDownloaded = value;
                RaisePropertyChanged(() => IsNotDownloaded);
            }
        }
        public bool IsDownloading
        {
            get { return _isDownloading; }
            private set
            {
                _isDownloading = value;
                RaisePropertyChanged(() => IsDownloading);
            }
        }
        public int DownloadProgress
        {
            get { return _downloadProgress; }
            private set
            {
                _downloadProgress = value;
                RaisePropertyChanged(() => DownloadProgress);
            }
        }

        public Common.Mvvm.Command DownloadCommand { get; private set; }
        public Common.Mvvm.Command StopCommand { get; private set; }

        public async Task<string> GetPath()
        {
            if (string.IsNullOrEmpty(_path))
            {
                await CheckBaseFile();
            }

            return _path;
        }

        private async void Initialize()
        {
            _uri = new Uri(_model.Url);
            SetType();
            SetSize();

            _locker.Reset();
            await Task.Factory.StartNew(async () =>
                {
                    await CheckBaseFile();
                    _locker.Set();
                });
        }
        private void SetType()
        {
            Common.IFileAccessorFactory fileAccessorFactory = Mvx.Resolve<Common.IFileAccessorFactory>();

            switch (_model.Type)
            {
                case "dict":
                    Type = PBaseTypes.Dictionary;
                    _fileAccessor = fileAccessorFactory.GetLocalFileAccessor();
                    _baseFileNameFormat = "{0}.sdc";
                    break;

                case "sound":
                    Type = PBaseTypes.Sound;
                    _fileAccessor = fileAccessorFactory.GetLocalFileAccessor();
                    _baseFileNameFormat = "{0}.sdc";
                    break;

                case "morphology":
                    Type = PBaseTypes.Morphology;
                    _fileAccessor = fileAccessorFactory.GetResourceFileAccessor();
                    _baseFileNameFormat = "Bases\\{0}.sdc";
                    break;

                case "demo_dict":
                    Type = PBaseTypes.Demo;
                    _fileAccessor = fileAccessorFactory.GetResourceFileAccessor();
                    _baseFileNameFormat = "Bases\\{0}.sdc";
                    break;

                default:
                    Debug.WriteLine(string.Format("Unknown base type: {0}", _model.Type));
                    Type = PBaseTypes.Unknown;
                    _fileAccessor = fileAccessorFactory.GetResourceFileAccessor();
                    _baseFileNameFormat = "Bases\\{0}.sdc";
                    break;
            }

            _baseFileName = string.Format(_baseFileNameFormat, _model.Id);
        }
        private void SetName()
        {
            switch (Type)
            {
                case PBaseTypes.Dictionary:
                    _name = _resourcesProvider.GetResource("Dictionary");
                    break;

                case PBaseTypes.Sound:
                    _name = string.Format(_resourcesProvider.GetResource("SoundFormat"), GetLocalizedLanguage());
                    break;

                default:
                    _name = string.Empty;
                    break;
            }
        }
        private void SetSize()
        {
            double mbytes = _model.Size / 1024f / 1024f;
            Size = string.Format("{0} Mb", mbytes.ToString("F1"));
        }

        private string GetLocalizedLanguage()
        {
            string localizedLanguage = _resourcesProvider.GetResource("Lang" + _model.LanguageFrom);
            if (string.IsNullOrEmpty(localizedLanguage))
            {
                return _model.LanguageFrom;
            }

            return localizedLanguage;
        }

        private async Task CheckBaseFile()
        {
            try
            {
                Uri uri = new Uri(string.Format(_baseFileNameFormat, _model.Id), UriKind.Relative);
                Common.FileInfo? baseFileInfo = await _fileAccessor.RequestFileInfo(uri);
                
                if (baseFileInfo.HasValue && baseFileInfo.Value.Size == (ulong)_model.Size)
                {
                    SetBaseState(PBaseState.Downloaded);
                    _path = baseFileInfo.Value.LocalPath;
                }
                else if (_fileDownloader.IsDownloading(_baseFileName))
                {
                    SetBaseState(PBaseState.Downloading);
                }
                else
                {
                    SetBaseState(PBaseState.NotDownloaded);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                SetBaseState(PBaseState.NotDownloaded);
            }
        }

        private async void SetBaseState(PBaseState stat)
        {
            await _uiDispatcher.Execute(() =>
                {
                    switch (stat)
                    {
                        case PBaseState.NotDownloaded:
                            IsDownloaded = false;
                            IsNotDownloaded = true;
                            IsDownloading = false;
                            StopCommand.IsExecutable = false;
                            DownloadCommand.IsExecutable = true;
                            break;

                        case PBaseState.Downloading:
                            IsDownloaded = false;
                            IsNotDownloaded = true;
                            IsDownloading = true;
                            DownloadCommand.IsExecutable = false;
                            StopCommand.IsExecutable = true;
                            break;

                        case PBaseState.Downloaded:
                            IsDownloaded = true;
                            IsNotDownloaded = false;
                            IsDownloading = false;
                            DownloadCommand.IsExecutable = false;
                            StopCommand.IsExecutable = false;
                            break;
                    }

                    Common.Delegate.Call(StateChanged);
                });
        }

        private void DownloadCommandImpl()
        {
            _locker.WaitOne();
            _fileDownloader.Start(_uri, _baseFileName);
        }
        private void StopCommandImpl()
        {
            _locker.WaitOne();
            _fileDownloader.Cancel(_baseFileName);
        }

        private void FileDownloaderOnStarted(string fileName)
        {
            _locker.WaitOne();

            if (fileName == _baseFileName)
            {
                SetBaseState(PBaseState.Downloading);
            }
        }
        private async void FileDownloaderOnProgress(string fileName, int persent)
        {
            _locker.WaitOne();

            if (fileName != _baseFileName)
            {
                return;
            }

            await _uiDispatcher.Execute(() =>
                {
                    DownloadProgress = persent;
                });
        }
        private void FileDownloaderOnComplited(string fileName, bool result)
        {
            _locker.WaitOne();

            if (fileName != _baseFileName)
            {
                return;
            }

            if (result)
            {
                SetBaseState(PBaseState.Downloaded);
            }
            else
            {
                SetBaseState(PBaseState.NotDownloaded);
            }

            DownloadProgress = 0;
        }
        private async void FileDownloaderOnErrorOccurred(string fileName, string message)
        {
            _locker.WaitOne();

            if (fileName != _baseFileName)
            {
                return;
            }

            await _userInteraction.ErrorMessage(string.Empty);
        }

        private Uri _uri;
        private string _path = string.Empty;
        private string _name;
        private string _baseFileName;
        private bool _isDownloaded = false;
        private bool _isNotDownloaded = false;
        private bool _isDownloading = false;
        private int _downloadProgress = 0;

        private Common.IFileAccessor _fileAccessor;
        private string _baseFileNameFormat;
        private readonly Models.PBase _model;
        private readonly IUserInteraction _userInteraction;
        private readonly Common.IFileDownloader _fileDownloader;
        private readonly Common.IResourcesProvider _resourcesProvider;
        private readonly Common.IUIDispatcher _uiDispatcher;

        private readonly ManualResetEvent _locker = new ManualResetEvent(true);
    }

    public enum PBaseTypes
    {
        Dictionary, Sound, Morphology, Demo, Unknown
    }

    public enum PBaseState
    {
        NotDownloaded, Downloading, Downloaded
    }
}
