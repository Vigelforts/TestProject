using System;
using System.Collections.Generic;

namespace Paragon.Container.Core
{
    public struct DictionaryInfo
    {
        internal DictionaryInfo(int id, string name, bool isDemo)
            : this()
        {
            Id = id;
            Name = name;
            IsDemo = isDemo;
            MorphoBasesPaths = new List<string>();
            SoundBasesPaths = new List<string>();
            Colors = new List<Models.ProductColor>();
        }

        public int Id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsDemo { get; internal set; }
        public string DictBasePath { get; internal set; }
        public List<string> MorphoBasesPaths { get; internal set; }
        public List<string> SoundBasesPaths { get; internal set; }
        public List<Models.ProductColor> Colors { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj is DictionaryInfo)
            {
                DictionaryInfo other = (DictionaryInfo)obj;
                return Id == other.Id && IsDemo == other.IsDemo;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ IsDemo.GetHashCode();
        }

        public static bool operator ==(DictionaryInfo a, DictionaryInfo b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(DictionaryInfo a, DictionaryInfo b)
        {
            return !(a == b);
        }
    }
}
