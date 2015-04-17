using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Paragon.Container.Models
{
    public struct Product
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string Language1 { get; set; }

        [XmlAttribute]
        public string Language2 { get; set; }

        [XmlAttribute]
        public string Level { get; set; }

        [XmlAttribute]
        public int Priority { get; set; }

        [XmlAttribute]
        public bool RemovedFromSale { get; set; }

        [XmlElement("strings")]
        public List<ProductStrings> Strings { get; set; }

        public List<ProductColor> Colors { get; set; }
        
        public string Icon { get; set; }

        [XmlElement("component")]
        public List<PBase> Bases { get; set; }
    }

    public struct ProductStrings
    {
        [XmlAttribute]
        public string Locale { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }

    public struct ProductColor
    {
        [XmlAttribute]
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
