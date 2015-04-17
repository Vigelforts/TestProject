using System;
using System.Xml.Serialization;

namespace Paragon.Container.Models
{
    public struct PBase
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public int MajorVersion { get; set; }

        [XmlAttribute]
        public int MinorVersion { get; set; }

        [XmlAttribute]
        public string Url { get; set; }

        [XmlAttribute]
        public long Size { get; set; }

        [XmlAttribute]
        public string LanguageFrom { get; set; }

        [XmlAttribute]
        public string LanguageTo { get; set; }
    }
}
