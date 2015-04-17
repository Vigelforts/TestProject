using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paragon.Container.Core
{
    internal sealed class BasesManager
    {
        public BasesManager(List<Models.PBase> bases)
        {
            _bases = bases;
        }

        public event Action DictBaseStateChanged;
        
        public PBase DictBase
        {
            get
            {
                EnsureInitialized();
                return _dictBase;
            }
        }
        public PBase DemoBase
        {
            get
            {
                EnsureInitialized();
                return _demoBase;
            }
        }
        public List<PBase> MorphoBases
        {
            get
            {
                EnsureInitialized();
                return _morphoBases;
            }
        }
        public List<PBase> SoundBases
        {
            get
            {
                EnsureInitialized();
                return _soundBases;
            }
        }
        public ObservableCollection<PBase> DownloadableBases
        {
            get
            {
                EnsureInitialized();
                return _downloadableBases;
            }
        }

        private void EnsureInitialized()
        {
            lock (_locker)
            {
                if (!_isInitialized)
                {
                    InitializeBases();
                }
            }
        }
        private void InitializeBases()
        {
            _soundBases = new List<PBase>();
            _morphoBases = new List<PBase>();
            _downloadableBases = new ObservableCollection<PBase>();

            foreach (Models.PBase baseModel in _bases)
            {
                PBase baseViewModel = new PBase(baseModel);

                switch (baseViewModel.Type)
                {
                    case PBaseTypes.Dictionary:
                        _downloadableBases.Add(baseViewModel);
                        _dictBase = baseViewModel;
                        _dictBase.StateChanged += DictBaseOnStateChanged;
                        break;

                    case PBaseTypes.Demo:
                        _demoBase = baseViewModel;
                        break;

                    case PBaseTypes.Sound:
                        _downloadableBases.Add(baseViewModel);
                        _soundBases.Add(baseViewModel);
                        break;

                    case PBaseTypes.Morphology:
                        _morphoBases.Add(baseViewModel);
                        break;
                }
            }

            if (_dictBase == null)
            {
                throw new Exception("Dictionary base is not found");
            }

            int dictBaseIndex = _downloadableBases.IndexOf(_dictBase);
            _downloadableBases.Move(dictBaseIndex, 0);

            _isInitialized = true;
        }

        private void DictBaseOnStateChanged()
        {
            Common.Delegate.Call(DictBaseStateChanged);
        }

        private PBase _dictBase;
        private PBase _demoBase;
        private List<PBase> _morphoBases;
        private List<PBase> _soundBases;
        private ObservableCollection<PBase> _downloadableBases;

        private bool _isInitialized = false;
        private readonly List<Models.PBase> _bases;
        private readonly object _locker = new object();
    }
}
