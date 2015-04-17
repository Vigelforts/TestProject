using Paragon.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Paragon.Container.Core
{
    internal static class CatalogReader
    {
        public static async Task<Models.ProductsCatalog> ReadCatalog(IFileAccessor fileAccessor)
        {
            string text = await fileAccessor.ReadFile(CatalogFileUri);
            return ParseCatalog(text);
        }

        private static Models.ProductsCatalog ParseCatalog(string text)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Models.ProductsCatalog));
            using (TextReader reader = new StringReader(text))
            {
                Models.ProductsCatalog catalog = (Models.ProductsCatalog)serializer.Deserialize(reader);
                return catalog;
            }
        }

        private static readonly Uri CatalogFileUri = new Uri("catalog.xml", UriKind.Relative);
    }
}
