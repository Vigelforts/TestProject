using System.Collections.Generic;
using System.Xml.Serialization;

namespace Paragon.Container.Core
{
    public struct ProductsDatabaseModel
    {
        [XmlAttribute]
        public int Version { get; set; }

        public List<DatabaseProductModel> Products { get; set; }
    }

    public struct DatabaseProductModel
    {
        public int Id { get; set; }
        public long Date { get; set; }
        public long Expire { get; set; }
    }
}
