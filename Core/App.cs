using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Paragon.Container.Core.DataSaving;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Paragon.Container.Core
{
    public sealed class App : MvxApplication
    {
        public App()
        {
            Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<ViewModels.SplashScreenViewModel>());
            Mvx.RegisterSingleton<IParametersManager>(() => new ParametersManager());
            Mvx.RegisterSingleton<IProductsDatabase>(() => new ProductsDatabase());
            Mvx.RegisterSingleton<IProductsCatalog>(() => new ProductsCatalog());
            Mvx.RegisterSingleton<IDictionaryService>(() => new DictionaryService());
            Mvx.RegisterSingleton<IArticleRenderingService>(() => new ArticleHtmlRenderer());
            Mvx.RegisterSingleton<IHistoryService>(() => new HistoryService());
            Mvx.RegisterSingleton<IFavoritesService>(() => new FavoritesService());
        }
    }
}
