using System;
using Cirrious.CrossCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paragon.Container.Core.DataSaving;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class ProductsCatalogViewModel : ViewModelBase
    {
        public ProductsCatalogViewModel()
        {
            _productsCatalog = Mvx.Resolve<IProductsCatalog>();
            _userInteraction = Mvx.Resolve<IUserInteraction>();
            _parametersManager = Mvx.Resolve<IParametersManager>();
            _settingsManager = Mvx.Resolve<Common.ISettingsManager>();
            _shortcutManager = Mvx.Resolve<Common.IShortcutManager>();
            _resourcesProvider = Mvx.Resolve<Common.IResourcesProvider>();
            

            ProductClickCommand = new Common.Mvvm.Command(ProductClickCommandImpl);
            PinToStartCommand = new Common.Mvvm.Command(PinToStartCommandImpl, false);

           
            BackCommand.IsExecutable = false;
            GotoCatalogCommand.IsExecutable = false;
            GotoDictionaryCommand.IsExecutable = false;
            GotoHistoryCommand.IsExecutable = false;
            GotoFavoritesCommand.IsExecutable = false;

            
            Initialize();
        }

        public ObservableCollection<Product> PurchasedProducts
        {
            get { return _purchasedProducts; }
        }
        public ObservableCollection<Product> Products
        {
            get { return _products; }
        }
        public Product CurrentProduct
        {
            get { return _currentProduct; }
            set
            {
                _currentProduct = value;
                RaisePropertyChanged(() => CurrentProduct);

                UpdateCurrentProductState();
            }
        }

        public string CurrentLanguage1
        {
            get { return _CurrentLanguage1; }
            set
            {
                _CurrentLanguage1= value;
                LanguagesList2 = UpdateLanguagesList(_LanguagesList2);
                UpdateProducts();
                RaisePropertyChanged(()=>CurrentLanguage1);
            }
        }

        public string CurrentLanguage2
        {
            get { return _CurrentLanguage2; }
            set
            {
                _CurrentLanguage2 = value;
                LanguagesList1 = UpdateLanguagesList(_LanguagesList1);
                UpdateProducts();
                RaisePropertyChanged(()=>CurrentLanguage2);
            }
        }


        public List<string> Languages
        {
            get
            {
                return _LanguagesList;
            }
        }

        public List<string> LanguagesList1
        {
            get { return _LanguagesList1; }
            set
            {
                _LanguagesList1 = value;
                
                RaisePropertyChanged(()=>LanguagesList1);
            }
        }

        public List<string> LanguagesList2
        {
            get { return _LanguagesList2; }
            set
            {
                _LanguagesList2 = value;
                RaisePropertyChanged(() => LanguagesList2);
            }
        } 
        public bool LanguageFilterEnabled
        {
            get { return _languageFilterEnabled; }
            set
            {
                _languageFilterEnabled=value;
                RaisePropertyChanged(()=>LanguageFilterEnabled);
            }
        }

        public bool NoProductSelected
        {
            get { return _noProductSelected; }
            set
            {
                _noProductSelected = value;
                RaisePropertyChanged(()=>NoProductSelected);
            }
        }
        public Common.Mvvm.Command ProductClickCommand { get; private set; }
        public Common.Mvvm.Command RestorePurchaseCommand
        {
            get { return _productsCatalog.UpdatePurchasesStatusCommand; }
        }
        public Common.Mvvm.Command PinToStartCommand { get; private set; }

        public async override void OnNavigateTo()
        {
            _settingsManager.Set(Settings.LastProductId, null);

            _productsCatalog.ProductPurchased += OnProductsCatalogProductPurchased;
            _productsCatalog.ProductLaunched += OnProductsCatalogProductLaunched;
            if (LanguageFilterEnabled == true)
            {
                try
                {
                    LanguagesFilterData languagesPair = await _languageFilterDataSavingServiceService.GetSavedPair();
                    if(!Languages.Contains(languagesPair.Language1)) throw new Exception();//На случай смены локали.Чтобы не попали названия языков из другой локали
                    CurrentLanguage1 = languagesPair.Language1;
                    CurrentLanguage2 = languagesPair.Language2;
                }
                catch (Exception)
                {
                    CurrentLanguage1 = _resourcesProvider.GetResource("Any");
                    CurrentLanguage2 = _resourcesProvider.GetResource("Any");
                }

                
            }
            if (_parametersManager.ContainsKey(Parameters.LastContainerProduct))
            {
                Product lastProduct = _parametersManager.Get<Product>(Parameters.LastContainerProduct);
                if (lastProduct != null)
                {
                    CurrentProduct = lastProduct;
                }
            }
        }
        public override void OnNavigateFrom()
        {
            _productsCatalog.ProductPurchased -= OnProductsCatalogProductPurchased;
            _productsCatalog.ProductLaunched -= OnProductsCatalogProductLaunched;

            _languageFilterDataSavingServiceService.SaveCurrentPair(new LanguagesFilterData(CurrentLanguage1,CurrentLanguage2));
            _parametersManager.Set(Parameters.LastContainerProduct, CurrentProduct);
        }

        private async void Initialize()
        {
            GetLanguagesLists();
            FillProducts();
            SetCurrentProduct();
            InitializeLanguageFilter();
            await HandleExpiredProducts();
        }

        private void InitializeLanguageFilter()
        {
            if (Products.Count >= 10)
            {
                LanguageFilterEnabled = true;
                _LanguagesList.Add(_resourcesProvider.GetResource("Any"));
                CurrentLanguage1 = _LanguagesList[0];
                CurrentLanguage2 = _LanguagesList[0];
                _languageFilterDataSavingServiceService = new LanguageFilterDataSavingService();
            }
            else
            {
                LanguageFilterEnabled = false;
            }
        }

        private void FillProducts()
        {
            PurchasedProducts.Clear();
            Products.Clear();

            IReadOnlyCollection<Product> products = _productsCatalog.GetProducts();
            foreach (Product product in products)
            {
                
                if (product.IsPurchased)
                {
                    PurchasedProducts.Add(product);
                }
                else if (!product.RemovedFromSale)
                {
                    Products.Add(product);
                }

            }
       
        }

      

        private void UpdateProducts()
        {
            var allProducts = _productsCatalog.GetProducts();

            foreach (var product in allProducts)
            {
                var language1 = _resourcesProvider.GetResource("Lang" + product.Language1);
                var language2 = _resourcesProvider.GetResource("Lang" + product.Language2);
                var any = _resourcesProvider.GetResource("Any");
                if (((language1 == _CurrentLanguage1 || _CurrentLanguage1 == any) && ((language2 == _CurrentLanguage2 || _CurrentLanguage2 == any)) ||
                    ((language2 == _CurrentLanguage1 || _CurrentLanguage1 == any) && ((language1 == _CurrentLanguage2 || CurrentLanguage2 == any)))))
                {
                    if (!product.IsPurchased && !Products.Contains(product) && !product.RemovedFromSale)
                    {
                        Products.Add(product);
                    }
                    else if (product.IsPurchased && !PurchasedProducts.Contains(product))
                    {
                        PurchasedProducts.Add(product);
                    }
                }
                else
                {
                    if (!product.IsPurchased && Products.Contains(product))
                    {
                        Products.Remove(product);
                    }
                    else if (product.IsPurchased && PurchasedProducts.Contains(product))
                    {
                        PurchasedProducts.Remove(product);
                    }
                }
                if (!Products.Contains(_currentProduct) || !PurchasedProducts.Contains(_currentProduct))
                {
                    //Если ранее выбранный словарь не попал в отфильтрованный список то выставляем самый первый в списке в качестве выбранного
                    if (PurchasedProducts.Count != 0)
                    {
                        CurrentProduct = PurchasedProducts[0];
                    }
                    else if (Products.Count != 0)
                    {
                        CurrentProduct = Products[0];
                    }
                    else
                    {
                        //
                    }
                }
            }
        }

        private void GetLanguagesLists()
           //Получаем список всех словарей и из каждого вынимаем пару языков
        {
            var products = _productsCatalog.GetProducts();
            foreach (var product in products)
            {
                if (!_LanguagesList.Contains(_resourcesProvider.GetResource("Lang"+product.Language1)))
                {
                    _LanguagesList.Add(_resourcesProvider.GetResource("Lang"+product.Language1));
                }
                if (!_LanguagesList.Contains(_resourcesProvider.GetResource("Lang"+product.Language2)))
                {
                    _LanguagesList.Add(_resourcesProvider.GetResource("Lang"+product.Language2));
                }

            }
        }

        private List<string> UpdateLanguagesList(List<string> listIn)
        {
            var list = listIn;
            foreach (var s in _LanguagesList)
            {
                var allProducts = _productsCatalog.GetProducts();

                foreach (var product in allProducts)
                {
                    var language1 = _resourcesProvider.GetResource("Lang" + product.Language1);
                    var language2 = _resourcesProvider.GetResource("Lang" + product.Language2);
                    var any = _resourcesProvider.GetResource("Any");
                    if (((language1 == _CurrentLanguage1 || _CurrentLanguage1 == any) && ((language2 == s || s == any)) ||
                        ((language2 == _CurrentLanguage1 || _CurrentLanguage1 == any) && ((language1 == s || s == any)))))
                    {
                           if(!list.Contains(s)) list.Add(s);


                    }
                    else
                    {
                        if (list.Contains(s)) list.Remove(s);
                    }                  
                }
            }
            return list;
        }
        private void SetCurrentProduct()
        {
            CurrentProduct = null;

            if (PurchasedProducts.Count > 0)
            {
                CurrentProduct = PurchasedProducts[0];
            }
            else if (Products.Count > 0)
            {
                CurrentProduct = Products[0];
            }
        }
        private async Task HandleExpiredProducts()
        {
            StringBuilder expiredProductNames = new StringBuilder();

            IReadOnlyCollection<Product> allProducts = _productsCatalog.GetProducts();
            List<Product> expiredProducts = allProducts.Where(p => p.IsExpired == true).ToList();
            foreach (Product product in expiredProducts)
            {
                expiredProductNames.Append(product.Name + '\n');
                product.IsExpired = false;
            }

            if (expiredProducts.Count > 0)
            {
                string message = _resourcesProvider.GetResource("ProductExpiredMessage") + '\n' + expiredProductNames.ToString();
                await _userInteraction.InformationMessage(message);
            }
        }

        private void OnProductsCatalogProductPurchased(int productId)
        {
            FillProducts();

            Product purchasedProduct = PurchasedProducts.Where(p => p.Id == productId).FirstOrDefault();
            if (purchasedProduct != null)
            {
                CurrentProduct = purchasedProduct;
            }
        }
        private async void OnProductsCatalogProductLaunched(DictionaryInfo info)
        {
            await Utils.OpenProduct(info);
            ShowViewModel<DictionaryViewModel>();
        }

        private void UpdateCurrentProductState()
        {
            foreach (Product product in PurchasedProducts)
            {
                product.IsCurrent = false;
            }

            foreach (Product product in Products)
            {
                product.IsCurrent = false;
            }

            if (CurrentProduct != null)
            {
                CurrentProduct.IsCurrent = true;
                if (CurrentProduct.IsPurchased)
                {
                    PinToStartCommand.IsExecutable = true;
                    return;
                }
            }

            PinToStartCommand.IsExecutable = false;
        }

        private void ProductClickCommandImpl(object param)
        {
            Product clickedProduct = param as Product;
            if (clickedProduct != null)
            {
                CurrentProduct = clickedProduct;
            }
        }
        private async void PinToStartCommandImpl()
        {
            if (CurrentProduct != null)
            {
                Common.ShortcutInfo info = new Common.ShortcutInfo();
                info.Id = CurrentProduct.Id.ToString();
                info.Name = CurrentProduct.Name;
                info.Icon = CurrentProduct.Icon;

                await _shortcutManager.Add(info);
            }
        }

        private Product _currentProduct;
        private readonly List<string> _LanguagesList = new List<string>();
        private  List<string> _LanguagesList1 = new List<string>();
        private  List<string> _LanguagesList2 = new List<string>(); 
        private bool _noProductSelected;
        private string _CurrentLanguage1;
        private string _CurrentLanguage2;
        private readonly ObservableCollection<Product> _purchasedProducts = new ObservableCollection<Product>();
        private readonly ObservableCollection<Product> _products = new ObservableCollection<Product>();
        private ILanguagesFilterDataSavingService _languageFilterDataSavingServiceService;
        private readonly IProductsCatalog _productsCatalog;
        private readonly IUserInteraction _userInteraction;
        private readonly IParametersManager _parametersManager;
        private readonly Common.ISettingsManager _settingsManager;
        private readonly Common.IShortcutManager _shortcutManager;
        private readonly Common.IResourcesProvider _resourcesProvider;
        private bool _languageFilterEnabled = false;
    }
}
