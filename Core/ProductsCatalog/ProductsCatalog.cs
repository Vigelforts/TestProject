using Cirrious.CrossCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Paragon.Container.Core
{
    internal sealed class ProductsCatalog : IProductsCatalog
    {
        internal ProductsCatalog()
        {
            _fileAccessorFactory = Mvx.Resolve<Common.IFileAccessorFactory>();
            _fileDownloader = Mvx.Resolve<Common.IFileDownloader>();
            _inAppService = Mvx.Resolve<Common.IInAppService>();
            _resourcesProvider = Mvx.Resolve<Common.IResourcesProvider>();
            _productsDatabase = Mvx.Resolve<IProductsDatabase>();
            _userInteraction = Mvx.Resolve<IUserInteraction>();

            UpdatePurchasesStatusCommand = new Common.Mvvm.Command(UpdatePurchasesStatusCommandImpl, true);

            _productsDatabase.ProductAdded += OnProductsDatabaseProductAdded;

            ReadCatalog();
        }

        public event Action<int> ProductPurchased;
        public event Action<DictionaryInfo> ProductLaunched;


        public int Id
        {
            get
            {
                return _catalogModel.Id;
            }
        }

        public Common.Mvvm.Command UpdatePurchasesStatusCommand { get; private set; }

        public IReadOnlyCollection<Product> GetProducts()
        {
            _locker.WaitOne();
            return _products;
        }
        public async Task<bool> EnterCode(string key)
        {
            _locker.WaitOne();

            if (!_fileDownloader.CheckInternetConnection())
            {
                await _userInteraction.InformationMessage(
                    _resourcesProvider.GetResource("NoInternetConnectionMessage"));

                return false;
            }

            SerialCodeResponse response = await SerialCodeHandler.HandleCode(key);
            if (response.IsValid && !IsKeyExpired(response) && !string.IsNullOrEmpty(response.BaseId))
            {
                Product product = GetProductByBaseId(response.BaseId);
                if (product != null)
                {
                    if (!response.IsTemporary)
                    {
                        _productsDatabase.AddProduct(product.Id);

                        string message = string.Format("{0}\n{1}", _resourcesProvider.GetResource("EnterCodeOk"), product.Name);
                        await _userInteraction.InformationMessage(message);
                    }
                    else
                    {
                        _productsDatabase.AddProduct(product.Id, response.ValidUntil);

                        string expiryDate = response.ValidUntil.ToString("dd MMMM yyyy");
                        string message = string.Format(_resourcesProvider.GetResource("EnterTempCodeOk"), expiryDate);
                        await _userInteraction.InformationMessage(string.Format("{0}\n\n{1}", message, product.Name));
                    }

                    Common.Delegate.Call(ProductPurchased, product.Id);
                }
            }
            else
            {
                await _userInteraction.ErrorMessage(_resourcesProvider.GetResource("EnterCodeError"));
            }

            return response.IsValid;
        }

        private async void ReadCatalog()
        {
            _locker.Reset();
            await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        _catalogModel = await CatalogReader.ReadCatalog(_fileAccessorFactory.GetResourceFileAccessor());
                        if (_catalogModel.Products == null || _catalogModel.Products.Count == 0)
                        {
                            throw new Exception("Products not found in catalog");
                        }

                        _products = new List<Product>();
                        var sortedProducts = _catalogModel.Products.OrderBy(p => p.Priority).Reverse();
                        foreach (Models.Product model in sortedProducts)
                        {
                            Product product = new Product(model);
                            product.ProductLaunched += ProductOnProductLaunched;
                            _products.Add(product);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        _locker.Set();
                    }
                });
        }

        private bool IsKeyExpired(SerialCodeResponse response)
        {
            if (response.IsTemporary && response.ValidUntil < DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        private void OnProductsDatabaseProductAdded(int productId)
        {
            _locker.WaitOne();

            Common.Delegate.Call<int>(ProductPurchased, productId);
        }

        private async void UpdatePurchasesStatusCommandImpl()
        {
            _locker.WaitOne();

            foreach (Models.Product product in _catalogModel.Products)
            {
                if (_inAppService.IsItemPurchased(product.Id.ToString()))
                {
                    if (!_productsDatabase.HasProduct(product.Id))
                    {
                        _productsDatabase.AddProduct(product.Id);
                        Common.Delegate.Call(ProductPurchased, product.Id);
                    }
                }
            }

            await ShowMessage();
        }

        private void ProductOnProductLaunched(DictionaryInfo info)
        {
            Common.Delegate.Call(ProductLaunched, info);
        }

        private async Task ShowMessage()
        {
            List<string> purchasedProducts = new List<string>();
            foreach (Product product in _products)
            {
                if (product.IsPurchased)
                {
                    purchasedProducts.Add(product.Name);
                }
            }

            await _userInteraction.RestorePurchaseMessage(purchasedProducts);
        }

        private Product GetProductByBaseId(string baseId)
        {
            foreach (Models.Product product in _catalogModel.Products)
            {
                foreach (Models.PBase pBase in product.Bases)
                {
                    if (string.Compare(pBase.Id, baseId, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return _products.Where(p => p.Id == product.Id).First();
                    }
                }
            }

            return null;
        }

        private Models.ProductsCatalog _catalogModel;
        private List<Product> _products;
        private readonly Common.IFileAccessorFactory _fileAccessorFactory;
        private readonly Common.IFileDownloader _fileDownloader;
        private readonly Common.IInAppService _inAppService;
        private readonly Common.IResourcesProvider _resourcesProvider;
        private readonly IProductsDatabase _productsDatabase;
        private readonly IUserInteraction _userInteraction;
        private readonly ManualResetEvent _locker = new ManualResetEvent(true);
    }
}
