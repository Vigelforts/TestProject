using Cirrious.CrossCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Paragon.Container.Core.DataSaving.DataAccessors
{
    internal sealed class DataAccessorV2
    {
        internal DataAccessorV2(Uri fileUri)
        {
            Common.IFileAccessorFactory fileAccessorFactory = Mvx.Resolve<Common.IFileAccessorFactory>();
            _fileAccessor = fileAccessorFactory.GetRoamingFileAccessor();

            _dictionaryService = Mvx.Resolve<IDictionaryService>();

            _fileUri = fileUri;
        }

        public async Task<IReadOnlyList<DictionaryItem>> ReadData()
        {
            List<DictionaryItem> items = new List<DictionaryItem>();

            try
            {
                string file = await _fileAccessor.ReadFile(_fileUri);

                XmlSerializer serializer = new XmlSerializer(typeof(Data));
                using (TextReader reader = new StringReader(file))
                {
                    Data data = (Data)serializer.Deserialize(reader);
                    foreach (DataItem itemModel in data.Items)
                    {
                        DictionaryItem item = _dictionaryService.GetItemByWord(itemModel.Title, (uint)itemModel.Language);
                        if (item != null)
                        {
                            items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return items;
        }
        public async Task WriteData(IEnumerable<DictionaryItem> items)
        {
            try
            {
                Data data = GetData(items);

                XmlSerializer serializer = new XmlSerializer(typeof(Data));
                StringBuilder builder = new StringBuilder();
                using (TextWriter writer = new StringWriter(builder))
                {
                    serializer.Serialize(writer, data);
                }

                await _fileAccessor.WriteFile(_fileUri, builder.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private Data GetData(IEnumerable<DictionaryItem> items)
        {
            Data data = new Data();
            foreach (DictionaryItem item in items)
            {
                string word = string.IsNullOrEmpty(item.SortKey) ? item.Title : item.SortKey;

                DataItem itemModel = new DataItem();
                itemModel.Title = word;
                itemModel.Language = (int)item.LanguageCode;

                data.Items.Add(itemModel);
            }

            return data;
        }

        private readonly Uri _fileUri;
        private readonly Common.IFileAccessor _fileAccessor;
        private readonly IDictionaryService _dictionaryService;
    }
}
