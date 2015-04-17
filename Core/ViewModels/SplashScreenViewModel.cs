using Cirrious.CrossCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class SplashScreenViewModel : ViewModelBase
    {
        public SplashScreenViewModel()
        {
            _parametersManager = Mvx.Resolve<IParametersManager>();
            _settingsManager = Mvx.Resolve<Common.ISettingsManager>();
            _uiDispatcher = Mvx.Resolve<Common.IUIDispatcher>();
            _productsCatalog = Mvx.Resolve<IProductsCatalog>();
        }

        public override void OnNavigateTo()
        {
            base.OnNavigateTo();
            ShowStartPage();
        }

        private async void ShowStartPage()
        {
            string tileId = _parametersManager.Get<string>(Parameters.LaunchedTileId);
            int productId;
            if (int.TryParse(tileId, out productId))
            {
                await ShowProduct(productId);
                return;
            }

            object lastProductIdItem = _settingsManager.Get(Settings.LastProductId);
            if (lastProductIdItem is int)
            {
                int lastProductId = (int)lastProductIdItem;
                await ShowProduct(lastProductId);
                return;
            }

            await _uiDispatcher.Execute(() =>
                {
                    ShowViewModel<ProductsCatalogViewModel>();
                });
        }

        private async Task ShowProduct(int productId)
        {
            try
            {
                IReadOnlyCollection<Product> products = _productsCatalog.GetProducts();
                Product lastProduct = products.First(p => p.Id == productId);
                DictionaryInfo productInfo = await lastProduct.GetInfo();

                await Utils.OpenProduct(productInfo);
                ShowViewModel<DictionaryViewModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowViewModel<ProductsCatalogViewModel>();
            }
        }

        private readonly IParametersManager _parametersManager;
        private readonly Common.ISettingsManager _settingsManager;
        private readonly Common.IUIDispatcher _uiDispatcher;
        private readonly IProductsCatalog _productsCatalog;
    }
}
