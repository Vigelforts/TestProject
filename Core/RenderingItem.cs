using Cirrious.CrossCore;
using System;
using System.Linq;

namespace Paragon.Container.Core
{
    internal class RenderingItem
    {
        public RenderingItem(DictionaryItem item, bool needHighlight)
        {
            Item = item;
            NeedHighlight = needHighlight;
        }

        public DictionaryItem Item { get; private set; }
        public RenderingItemType Type
        {
            get
            {
                if (_type == RenderingItemType.None)
                {
                    DetermineItemType();
                }

                return _type;
            }
        }
        public bool NeedHighlight { get; private set; }

        private void DetermineItemType()
        {
            _dictionaryService = Mvx.Resolve<IDictionaryService>();

            if (IsSearchResultsItem())
            {
                return;
            }

            if (IsAddtionalItem())
            {
                _type = RenderingItemType.AddtionalItem;
                return;
            }

            _type = RenderingItemType.DictionaryItem;
        }
        private bool IsSearchResultsItem()
        {
            foreach (DictionaryList list in _dictionaryService.SearchResults)
            {
                if (list.Items.Contains(Item))
                {
                    if (list.Type == DictionaryListType.Headword)
                    {
                        _type = RenderingItemType.SearchResultHeadwordItem;
                    }
                    else
                    {
                        _type = RenderingItemType.SearchResultItem;
                    }

                    return true;
                }
            }

            return false;
        }
        private bool IsAddtionalItem()
        {
            foreach (AdditionalArticlesList list in _dictionaryService.AdditionalArticles)
            {
                if (list.Index == Item.Index.ListIndex)
                {
                    return true;
                }
            }

            return false;
        }

        private RenderingItemType _type = RenderingItemType.None;
        private IDictionaryService _dictionaryService;
    }

    internal enum RenderingItemType
    {
        None, DictionaryItem, SearchResultItem, SearchResultHeadwordItem, AddtionalItem
    }
}
