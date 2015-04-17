using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Paragon.Container.Models
{
    [XmlRoot("Catalog")]
    public struct ProductsCatalog
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public int Version { get; set; }

        [XmlElement("Product")]
        public List<Product> Products { get; set; }
    }
}
