using System;
using System.Diagnostics;

namespace Paragon.Container.Core
{
    public enum MetadataTypes
    {
        Text, Link, Sound, PopupArticle
    }

    public class Metadata
    {
        internal Metadata(MetadataTypes type, uint listIndex, uint entryIndex, string label)
        {
            _type = type;
            _listIndex = listIndex;
            _entryIndex = entryIndex;
            _label = label == null ? string.Empty : label;

            _word = string.Empty;
            _languageCode = 0;
            _soundIndex = 0;
        }
        internal Metadata(MetadataTypes type, string word, uint languageCode)
        {
            _type = type;
            _word = word;
            _languageCode = languageCode;

            _listIndex = 0;
            _entryIndex = 0;
            _soundIndex = 0;
            _label = string.Empty;
        }
        internal Metadata(MetadataTypes type, uint soundIndex)
        {
            _type = type;
            _soundIndex = soundIndex;

            _listIndex = 0;
            _entryIndex = 0;
            _languageCode = 0;
            _word = string.Empty;
            _label = string.Empty;
        }

        public MetadataTypes Type
        {
            get { return _type; }
        }

        public uint ListIndex
        {
            get { return _listIndex; }
        }
        public uint EntryIndex
        {
            get { return _entryIndex; }
        }
        public uint SoundIndex
        {
            get { return _soundIndex; }
        }
        public string Label
        {
            get { return _label; }
        }
        public string Word
        {
            get { return _word; }
        }
        public uint LanguageCode
        {
            get { return _languageCode; }
        }

        public static Metadata Parse(string data)
        {
            try
            {
                string[] parts = data.Split(';');

                switch (parts[0])
                {
                    case "text":
                        return new Metadata(MetadataTypes.Text, parts[1], uint.Parse(parts[2]));

                    case "link":
                        return new Metadata(MetadataTypes.Link, uint.Parse(parts[1]), uint.Parse(parts[2]), parts[3]);

                    case "sound":
                        return new Metadata(MetadataTypes.Sound, uint.Parse(parts[1]));

                    case "popupArticle":
                        return new Metadata(MetadataTypes.PopupArticle, uint.Parse(parts[1]), uint.Parse(parts[2]), parts[3]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            throw new FormatException("Metadata unsupported format");
        }

        public override string ToString()
        {
            switch (_type)
            {
                case MetadataTypes.Text:
                    return string.Format("text;{0};{1}", _word, _languageCode);

                case MetadataTypes.Link:
                    return string.Format("link;{0};{1};{2}", _listIndex, _entryIndex, _label);

                case MetadataTypes.Sound:
                    return string.Format("sound;{0}", _soundIndex);

                case MetadataTypes.PopupArticle:
                    return string.Format("popupArticle;{0};{1};{2}", _listIndex, _entryIndex, _label);
            }

            throw new FormatException("Metadata unsupported format");
        }

        private MetadataTypes _type;
        private uint _listIndex;
        private uint _entryIndex;
        private uint _soundIndex;
        private string _label;
        private string _word;
        private uint _languageCode;
    }
}
