using Cirrious.CrossCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Paragon.Container.Core.DataSaving.DataAccessors
{
    internal sealed class DataAccessorV1
    {
        internal DataAccessorV1(Uri fileUri)
        {
            Common.IFileAccessorFactory fileAccessorFactory = Mvx.Resolve<Common.IFileAccessorFactory>();
            _fileAccessor = fileAccessorFactory.GetLocalFileAccessor();

            _dictionaryService = Mvx.Resolve<IDictionaryService>();

            _fileUri = fileUri;
        }

        public async Task<IReadOnlyCollection<DictionaryItem>> ReadData()
        {
            List<DictionaryItem> items = new List<DictionaryItem>();

            try
            {
                string file = await _fileAccessor.ReadFile(_fileUri);
                if (!string.IsNullOrEmpty(file))
                {
                    string[] parts = file.Split('*');
                    foreach (string part in parts)
                    {
                        Dictionary.ListItemIndex itemIndex = Dictionary.ListItemIndex.FromString(part);
                        DictionaryItem item = _dictionaryService.GetItemByIndex(itemIndex);
                        DictionaryItem realItem = _dictionaryService.GetItemByWord(item.Title, item.LanguageCode);
                        items.Add(realItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return items;
        }
        public async Task DeleteFile()
        {
            try
            {
                await _fileAccessor.DeleteFile(_fileUri);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private readonly Uri _fileUri;
        private readonly Common.IFileAccessor _fileAccessor;
        private readonly IDictionaryService _dictionaryService;
    }
}
