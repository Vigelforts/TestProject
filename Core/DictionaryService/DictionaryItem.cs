using Cirrious.CrossCore;
using System;

namespace Paragon.Container.Core
{
    public sealed class DictionaryItem
    {
        internal DictionaryItem(Dictionary.ListItem model)
        {
            _model = model;
            _hasSound = _model.HasSound;

            _dictionaryService = Mvx.Resolve<IDictionaryService>();

            Initialize();
        }

        public Dictionary.ListItemIndex Index
        {
            get { return _model.Index; }
        }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string PartOfSpeech { get; private set; }
        public string SortKey { get; set; }
        public string Label { get; set; }
        public uint LanguageCode
        {
            get { return _model.LanguageCode; }
        }
        public Dictionary.ListItemIndex? MorphoTableIndex
        {
            get
            {
                return _morphoTableIndex;
            }
        }
        public bool HasSound
        {
            get
            {
                return _hasSound;
            }
            set
            {
                _hasSound = value;
            }
        }
        public bool HasHierarchy
        {
            get
            {
                return _model.HasHierarchy;
            }
        }

        public DictionaryItem GetRealItem()
        {
            string word = string.IsNullOrEmpty(SortKey) ? Title : SortKey;
            return _dictionaryService.GetItemByWord(word, LanguageCode);
        }

        public override bool Equals(object obj)
        {
            DictionaryItem other = obj as DictionaryItem;
            if (other != null)
            {
                return _model.Equals(other._model);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return _model.GetHashCode();
        }

        internal Dictionary.ListItem GetModel()
        {
            return _model;
        }

        private void Initialize()
        {
            SetTitle();
            SetSubtitle();
            SetPartOfSpeech();

            if (_model.ShowVariants.ContainsKey(Dictionary.ShowVariantsTypes.Label))
            {
                Label = _model.ShowVariants[Dictionary.ShowVariantsTypes.Label];
            }
            else
            {
                Label = string.Empty;
            }

            if (_model.ShowVariants.ContainsKey(Dictionary.ShowVariantsTypes.SortKey))
            {
                SortKey = _model.ShowVariants[Dictionary.ShowVariantsTypes.SortKey];
            }
            else
            {
                SortKey = string.Empty;
            }

            if (Hacks.NeedSwitchShowVariants(Index.BaseId) && !string.IsNullOrEmpty(Subtitle))
            {
                string title = Title;
                Title = Subtitle;
                Subtitle = title;
            }

            _morphoTableIndex = _dictionaryService.FindMorphoTable(Title);
        }
        private void SetTitle()
        {
            if (_model.ShowVariants.ContainsKey(Dictionary.ShowVariantsTypes.Show))
            {
                Title = RemoveSpecialSymbols(_model.ShowVariants[Dictionary.ShowVariantsTypes.Show]);
            }
            else
            {
                Title = string.Empty;
            }
        }
        private void SetSubtitle()
        {
            if (_model.ShowVariants.ContainsKey(Dictionary.ShowVariantsTypes.Phrase))
            {
                Subtitle = RemoveSpecialSymbols(_model.ShowVariants[Dictionary.ShowVariantsTypes.Phrase]);
            }
            else if (_model.ShowVariants.ContainsKey(Dictionary.ShowVariantsTypes.ShowSecondary))
            {
                Subtitle = RemoveSpecialSymbols(_model.ShowVariants[Dictionary.ShowVariantsTypes.ShowSecondary]);
            }
            else
            {
                Subtitle = string.Empty;
            }
        }
        private void SetPartOfSpeech()
        {
            if (_model.ShowVariants.ContainsKey(Dictionary.ShowVariantsTypes.PartOfSpeech))
            {
                PartOfSpeech = RemoveSpecialSymbols(_model.ShowVariants[Dictionary.ShowVariantsTypes.PartOfSpeech]);
            }
            else
            {
                PartOfSpeech = string.Empty;
            }
        }

        private string RemoveSpecialSymbols(string src)
        {
            return src
                .Trim()
                .Replace("\n", "")
                .Replace("\t", "");
        }

        private bool _hasSound = false;
        private Dictionary.ListItemIndex? _morphoTableIndex;
        private readonly Dictionary.ListItem _model;
        private readonly IDictionaryService _dictionaryService;
    }
}
