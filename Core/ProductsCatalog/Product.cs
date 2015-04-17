using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Paragon.Container.Core
{
    public sealed class Product : MvxNotifyPropertyChanged
    {
        internal Product(Models.Product model)
        {
            _model = model;

            _inAppService = Mvx.Resolve<Common.IInAppService>();
            _userInteraction = Mvx.Resolve<IUserInteraction>();
            _productsDatabase = Mvx.Resolve<IProductsDatabase>();
            _basesManager = new BasesManager(_model.Bases);

            _productsDatabase.ProductAdded += ProductsDatabaseOnProductAdded;
            _productsDatabase.ProductExpired += OnProductsDatabaseProductExpired;
            _basesManager.DictBaseStateChanged += BasesManagerOnDictBaseStateChanged;

            _buyCommand = new Common.Mvvm.Command(BuyCommandImpl);
            _demoCommand = new Common.Mvvm.Command(DemoCommandImpl, false);
            _launchCommand = new Common.Mvvm.Command(LaunchCommandImpl, false);

            _inAppService.ErrorOccurred += InAppServiceOnErrorOccurred;

            Initialize();
        }

        public event Action<DictionaryInfo> ProductLaunched;

        public int Id { get; private set; }

        public string Language1
        {
            get { return _model.Language1; }
        }

        public string Language2
        {
            get { return _model.Language2; }
        }

        public bool IsVisible
        {//Определяет видимость словаря в на странице каталога продуктов
            set
            {
                _isVisible = value;
                RaisePropertyChanged(()=>IsVisible);
            }
            get { return _isVisible; }
        }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ProductLevel Level { get; private set; }
        public byte[] Icon { get; private set; }
        public string Price
        {
            get
            {
                if (_price == null)
                {
                    _inAppService.RequestPrice(_model.Id.ToString(), PriceCallback);
                }

                return _price;
            }
            private set
            {
                _price = value;
                RaisePropertyChanged(() => Price);
            }
        }
        public bool IsPurchased
        {
            get { return _isPurchased; }
            private set
            {
                _isPurchased = value;
                RaisePropertyChanged(() => IsPurchased);
            }
        }
        public bool RemovedFromSale
        {
            get
            {
                return _model.RemovedFromSale;
            }
        }
        public bool IsNotPurchased
        {
            get { return _isNotPurchased; }
            private set
            {
                _isNotPurchased = value;
                RaisePropertyChanged(() => IsNotPurchased);
            }
        }
        public bool IsExpired
        {
            get { return _isExpired; }
            set
            {
                _isExpired = value;
                RaisePropertyChanged(() => IsExpired);
            }
        }
        public bool IsCurrent
        {
            get { return _isCurrent; }
            internal set
            {
                _isCurrent = value;
                RaisePropertyChanged(() => IsCurrent);
            }
        }

        public ObservableCollection<PBase> DownloadableBases
        {
            get { return _basesManager.DownloadableBases; }
        }

        public Common.Mvvm.Command BuyCommand
        {
            get { return _buyCommand; }
        }
        public Common.Mvvm.Command DemoCommand
        {
            get
            {
                lock (_locker)
                {
                    if (_basesManager.DemoBase != null)
                    {
                        _demoCommand.IsExecutable = true;
                    }
                    else
                    {
                        _demoCommand.IsExecutable = false;
                    }

                    return _demoCommand;
                }
            }
        }
        public Common.Mvvm.Command LaunchCommand
        {
            get { return _launchCommand; }
        }

        public async Task<DictionaryInfo> GetInfo()
        {
            bool isDemo = !IsPurchased;

            PBase dictBase = null;
            if (isDemo)
            {
                dictBase = _basesManager.DemoBase;
            }
            else
            {
                dictBase = _basesManager.DictBase;
            }

            string dictBasePath = await dictBase.GetPath();
            if (string.IsNullOrEmpty(dictBasePath))
            {
                throw new Exception("File of dictionary base is not found");
            }

            DictionaryInfo dictionaryInfo = new DictionaryInfo(_model.Id, Name, isDemo);
            dictionaryInfo.DictBasePath = dictBasePath;
            dictionaryInfo.Colors = _model.Colors;

            foreach (PBase morphoBase in _basesManager.MorphoBases)
            {
                string morphoBasePath = await morphoBase.GetPath();
                if (!string.IsNullOrEmpty(morphoBasePath))
                {
                    dictionaryInfo.MorphoBasesPaths.Add(morphoBasePath);
                }
            }

            foreach (PBase soundBase in _basesManager.SoundBases)
            {
                string soundBasePath = await soundBase.GetPath();
                if (!string.IsNullOrEmpty(soundBasePath))
                {
                    dictionaryInfo.SoundBasesPaths.Add(soundBasePath);
                }
            }

            return dictionaryInfo;
        }

        private void UpdateStatus()
        {
            lock (_locker)
            {
                IsPurchased = _productsDatabase.HasProduct(_model.Id);
                IsNotPurchased = !IsPurchased;
                LaunchCommand.IsExecutable = IsPurchased && _basesManager.DictBase.IsDownloaded;
            }
        }

        private void Initialize()
        {
            lock (_locker)
            {
                Id = _model.Id;

                Localize();
                SetLevel();

                if (!string.IsNullOrEmpty(_model.Icon))
                {
                    Icon = Convert.FromBase64String(_model.Icon);
                }

                IsPurchased = _productsDatabase.HasProduct(_model.Id);
                IsNotPurchased = !IsPurchased;
            }
        }
        private void Localize()
        {
            string locale = LocaleInformation.GetLocale();

            foreach (Models.ProductStrings strings in _model.Strings)
            {
                if (locale.ToLower() == strings.Locale.ToLower())
                {
                    Name = strings.Name;
                    Description = strings.Description;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description))
            {
                return;
            }

            foreach (Models.ProductStrings strings in _model.Strings)
            {


                if (strings.Locale.ToLower() == "english")
                {
                    Name = strings.Name;
                    Description = strings.Description;//Если среди переводов описания словаря нет перевода под текущую локаль то берем английское описание
                    break;

                }
            }
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description))
            {
                return;
            }
            foreach (Models.ProductStrings strings in _model.Strings)
            {
                Name = strings.Name;
                Description = strings.Description;

                if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description))
                {
                    return;//На случай если нет английского перевода и под локаль его тоже нет.Берем первое попавшеесся.В идеале сюда мы дойти уже не должны!!
                }
            }
        }        
        private void SetLevel()
        {
            switch (_model.Level.ToLower())
            {
                case "advanced":
                    Level = ProductLevel.Advanced;
                    break;

                case "premium":
                    Level = ProductLevel.Premium;
                    break;

                case "basic":
                    Level = ProductLevel.Basic;
                    break;

                case "school":
                    Level = ProductLevel.School;
                    break;

                case "concise":
                    Level = ProductLevel.Concise;
                    break;

                default:
                    Level = ProductLevel.Other;
                    break;
            }
        }

        private async void BuyCommandImpl()
        {
            if (IsPurchased)
            {
                return;
            }

            bool result = await _inAppService.BuyItem(_model.Id.ToString());
            if (result)
            {
                IsPurchased = true;
                IsNotPurchased = false;
                _productsDatabase.AddProduct(_model.Id);
            }
        }
        private async void DemoCommandImpl()
        {
            DictionaryInfo dictionaryInfo = await GetInfo();
            Common.Delegate.Call(ProductLaunched, dictionaryInfo);
        }
        private async void LaunchCommandImpl()
        {
            DictionaryInfo dictionaryInfo = await GetInfo();
            Common.Delegate.Call(ProductLaunched, dictionaryInfo);
        }

        private void ProductsDatabaseOnProductAdded(int productId)
        {
            if (productId == _model.Id)
            {
                UpdateStatus();
            }
        }
        private void OnProductsDatabaseProductExpired(int productId)
        {
            if (productId == _model.Id)
            {
                IsExpired = true;
            }
        }
        private void BasesManagerOnDictBaseStateChanged()
        {
            UpdateStatus();
        }
        private void PriceCallback(string productId, string price)
        {
            if (Id.ToString() == productId)
            {
                Price = price;
            }
        }

        private async void InAppServiceOnErrorOccurred(string productId, string message)
        {
            if (productId != _model.Id.ToString())
            {
                return;
            }

            await _userInteraction.ErrorMessage(string.Empty);
        }

        private Common.Mvvm.Command _buyCommand;
        private Common.Mvvm.Command _demoCommand;
        private Common.Mvvm.Command _launchCommand;

        private bool _isCurrent = false;
        private bool _isPurchased = false;
        private bool _isNotPurchased = false;
        private bool _isExpired = false;
        private bool _isVisible = true;
        private string _price;
        private readonly Models.Product _model;
        private readonly Common.IInAppService _inAppService;
        private readonly IUserInteraction _userInteraction;
        private readonly IProductsDatabase _productsDatabase;
        private readonly BasesManager _basesManager;
        private readonly object _locker = new object();
    }
}
