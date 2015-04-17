using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cirrious.CrossCore;
using Paragon.Common;

namespace Paragon.Container.Core.DataSaving.DataAccessors
{
    public class DataAccessorGeneric<TType>
        where TType : class
    {

        private readonly IFileAccessor _fileAccessor;
        private IDictionaryService _dictionaryService;
        private readonly Uri _fileUri;

        public DataAccessorGeneric(Uri uri)
        {
            Common.IFileAccessorFactory fileAccessorFactory = Mvx.Resolve<Common.IFileAccessorFactory>();
            _fileAccessor = fileAccessorFactory.GetRoamingFileAccessor();

            _dictionaryService = Mvx.Resolve<IDictionaryService>();

            _fileUri = uri;
        }

        public async Task WriteData(TType data)
        {
            try
            {
                var serializer = new XmlSerializer(typeof (TType));
                var builder = new StringBuilder();

                using (TextWriter writer = new StringWriter(builder))
                {
                    serializer.Serialize(writer, data);
                }

                await _fileAccessor.WriteFile(_fileUri, builder.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task<TType> ReadData()
        {
            
            string file = await _fileAccessor.ReadFile(_fileUri);
            if (file == null)
            {
                throw new FileNotFoundException();
            }
            var serializer = new XmlSerializer(typeof (TType));
            TType result;
            using (TextReader reader = new StringReader(file))
            {
                result = (TType) serializer.Deserialize(reader);
            }
            return result;

        }
    }
}