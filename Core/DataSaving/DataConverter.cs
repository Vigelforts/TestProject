using Cirrious.CrossCore;
using Paragon.Container.Core.DataSaving.DataAccessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paragon.Container.Core.DataSaving
{
    internal sealed class DataConverter
    {
        public async Task ConvertFavorites(int productId)
        {
            Uri fileUriV1 = new Uri(string.Format(FavoritesService.FileNameFormat, "", productId), UriKind.Relative);
            Uri fileUriV2 = new Uri(string.Format(FavoritesService.FileNameFormat, 2, productId), UriKind.Relative);

            await Convert(fileUriV1, fileUriV2);
        }
        public async Task ConvertHistory(int productId)
        {
            Uri fileUriV1 = new Uri(string.Format(HistoryService.FileNameFormat, "", productId), UriKind.Relative);
            Uri fileUriV2 = new Uri(string.Format(HistoryService.FileNameFormat, 2, productId), UriKind.Relative);

            await Convert(fileUriV1, fileUriV2);
        }

        private async Task Convert(Uri fileUriV1, Uri fileUriV2)
        {
            DataAccessorV1 dataAccessorV1 = new DataAccessorV1(fileUriV1);
            List<DictionaryItem> itemsV1 = new List<DictionaryItem>(await dataAccessorV1.ReadData());

            if (itemsV1.Count > 0)
            {

                DataAccessorV2 dataAccessorV2 = new DataAccessorV2(fileUriV2);
                List<DictionaryItem> itemsV2 = new List<DictionaryItem>(await dataAccessorV2.ReadData());

                foreach (DictionaryItem itemV1 in itemsV1)
                {
                    if (!itemsV2.Contains(itemV1))
                    {
                        itemsV2.Add(itemV1);
                    }
                }

                await dataAccessorV2.WriteData(itemsV2.OrderBy(i => i.Title).Select(i => i));
            }

            await dataAccessorV1.DeleteFile();
        }
    }
}
