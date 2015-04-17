using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Paragon.Container.Core.DataSaving;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paragon.Container.Core
{
    internal static class Utils
    {
        public static async Task OpenProduct(DictionaryInfo info)
        {
            Common.ISettingsManager settingsManager = Mvx.Resolve<Common.ISettingsManager>();
            IParametersManager parametersManager = Mvx.Resolve<IParametersManager>();
            IAppStylesService appStylesService = Mvx.Resolve<IAppStylesService>();

            parametersManager.Set(Parameters.LaunchedProduct, info);

            if (!info.IsDemo)
            {
                settingsManager.Set(Settings.LastProductId, info.Id);
            }

            IDictionaryService dictionary = Mvx.Resolve<IDictionaryService>();
            dictionary.Open(info);

            IArticleRenderingService articleRenderingService = Mvx.Resolve<IArticleRenderingService>();
            articleRenderingService.Reset();

            DataConverter dataConverter = new DataConverter();
            await dataConverter.ConvertHistory(info.Id);
            await dataConverter.ConvertFavorites(info.Id);

            IHistoryService historyService = Mvx.Resolve<IHistoryService>();
            historyService.Reset();

            IFavoritesService favoritesService = Mvx.Resolve<IFavoritesService>();
            favoritesService.Reset();

            appStylesService.SetDefaultColors();
            foreach (Models.ProductColor color in info.Colors)
            {
                appStylesService.SetColor(color.Title, color.Value);
            }
        }

        public static string GetWordListName(Dictionary.WordList list)
        {
            string locale = LocaleInformation.GetLocale();
            foreach (Dictionary.LocalizedString localizedName in list.NameStrings)
            {
                if (localizedName.Language.FullName == locale)
                {
                    return localizedName.Value;
                }
            }

            if (list.NameStrings.Count > 0)
            {
                return list.NameStrings[0].Value;
            }

            return string.Empty;
        }
    }
}
