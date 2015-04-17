using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Paragon.Dictionary;

namespace Paragon.Container.Core
{
    internal sealed class DictionaryService : MvxNotifyPropertyChanged, IDictionaryService
    {
        public DictionaryService()
        {
            _resourcesProvider = Mvx.Resolve<Common.IResourcesProvider>();
            _itemsController = Mvx.Resolve<IItemsController>();
            _searchQueryTimer = Mvx.Resolve<Common.ITimer>();

            AdditionalArticles = new ObservableCollection<AdditionalArticlesList>();
            Hints = new ObservableCollection<DictionaryItem>();
            SearchResults = new ObservableCollection<DictionaryList>();

            SearchCommand = new Common.Mvvm.Command(SearchCommandImpl);
            SwitchDirectionCommand = new Common.Mvvm.Command(SwitchDirectionCommandImpl);
            HintClickCommand = new Common.Mvvm.Command(HintClickCommandImpl);

            _searchQueryTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            _searchQueryTimer.Tick += SearchQueryTimerOnTick;
        }

        public event Action<DictionaryItem> ItemClicked;
        public event Action<DictionaryList> HeaderClicked;

        public int ProductId { get; private set; }

        public ObservableCollection<AdditionalArticlesList> AdditionalArticles { get; private set; }

        public ObservableCollection<DictionaryItem> Hints { get; private set; }
        public ObservableCollection<DictionaryList> SearchResults { get; private set; }

        public bool ShowNoSearchResultsHint
        {
            get { return _showNoSearchResultsHint; }
            private set
            {
                _showNoSearchResultsHint = value;
                RaisePropertyChanged(() => ShowNoSearchResultsHint);
            }
        }

        public int SearchResultsListsMaxCount
        {
            get { return _searchResultsListsMaxCount; }
            set
            {
                _searchResultsListsMaxCount = value;
                RaisePropertyChanged(() => SearchResultsListsMaxCount);
            }
        }

        public Dictionary.Language LanguageFrom
        {
            get { return _languageFrom; }
            private set
            {
                _languageFrom = value;
                RaisePropertyChanged(() => LanguageFrom);
            }
        }
        public Dictionary.Language LanguageTo
        {
            get { return _languageTo; }
            private set
            {
                _languageTo = value;
                RaisePropertyChanged(() => LanguageTo);
            }
        }

        public Common.Mvvm.Command SearchCommand { get; private set; }
        public Common.Mvvm.Command SwitchDirectionCommand { get; private set; }
        public Common.Mvvm.Command HintClickCommand { get; private set; }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                lock (_locker)
                {
                    string newValue = value;
                    if (newValue.Length > MaxSearchQuerySymbolsCount)
                    {
                        newValue = newValue.Substring(0, MaxSearchQuerySymbolsCount);
                    }

                    if (newValue == _searchQuery)
                    {
                        return;
                    }

                    _searchQuery = newValue;
                    RaisePropertyChanged(() => SearchQuery);

                    CheckDirection();
                    _searchQueryTimer.Start();
                }
            }
        }
        public int MaxSearchQuerySymbolsCount
        {
            get { return _maxSearchQuerySymbolsCount; }
        }

        public void Open(DictionaryInfo info)
        {
            lock (_locker)
            {
                ProductId = info.Id;
                _dictBaseId = Path.GetFileNameWithoutExtension(info.DictBasePath);

                Dictionary.DictionaryFactory.Initialize(info.DictBasePath, info.SoundBasesPaths, info.MorphoBasesPaths);
                _dictionary = Dictionary.DictionaryFactory.GetDictionary();
                _articleService = Dictionary.DictionaryFactory.GetArticleService();

                Initialize();
            }
        }

        public DictionaryItem GetItemByIndex(Dictionary.ListItemIndex index)
        {
            lock (_locker)
            {
                return new DictionaryItem(_dictionary.GetItemByIndex(index));
            }
        }
        public DictionaryItem GetItemByWord(string word, uint languageCode)
        {
            lock (_locker)
            {
                IEnumerable<Dictionary.WordList> dictLists = GetDictionaryListsByLanguage(languageCode);
                foreach (Dictionary.WordList list in dictLists)
                {
                    Dictionary.ListItem result = list.FindWord(word);
                    if (result != null)
                    {
                        return new DictionaryItem(result);
                    }
                }

                return null;
            }
        }
        public List<string> GetAllWordForms(string query)
        {
            lock (_locker)
            {
                Dictionary.Language language = _dictionary.RecognizeLanguage(query);
                if (language == null)
                {
                    language = LanguageFrom;
                }

                List<string> result = new List<string>();

                string[] words = query.Split(' ');
                foreach (string word in words)
                {
                    result.AddRange(_dictionary.GetAllWordForms(word, language));
                }

                return result;
            }
        }
        public Dictionary.WordList GetWordListByIndex(int index)
        {
            return _lists.Where(l => l.Index == index).First();
        }

        public Dictionary.ListItemIndex? FindMorphoTable(string word)
        {
            var lists = _lists.Where(l => l.Type == Dictionary.WordListTypes.MorphologyArticles);
            foreach (Dictionary.WordList list in lists)
            {
                Dictionary.ListItem result = list.FindWord(word, false);
                if (result != null)
                {
                    return result.Index;
                }
            }

            return null;
        }

        public Dictionary.Version GetDictionaryVersion()
        {
            lock (_locker)
            {
                return _dictionary.GetDictionaryVersion();
            }
        }
        public uint GetEngineViersion()
        {
            lock (_locker)
            {
                return 114;
            }
        }

        private void Initialize()
        {
            ResetSearchResults();
            SearchQuery = string.Empty;

            if (_dictionary != null)
            {
                _lists = _dictionary.GetAllLists();
                FillDirections();
                FillAdditionalArticles();
            }
            else
            {
                _lists = new List<Dictionary.WordList>();
                AdditionalArticles.Clear();
                LanguageFrom = null;
                LanguageTo = null;
            }

            SearchResultsListsMaxCount = GetSearchLists().Count();
        }

        private void FillDirections()
        {
            List<Dictionary.WordList> dictLists = GetDictionaryLists().ToList();
            LanguageFrom = dictLists[0].LanguageFrom;
            LanguageTo = dictLists[0].LanguageTo;

            _canSwitchDirection = false;
            foreach (Dictionary.WordList dictList in dictLists)
            {
                if (dictList.LanguageFrom != dictLists[0].LanguageFrom)
                {
                    _canSwitchDirection = true;
                    break;
                }
            }
        }
        private void FillAdditionalArticles()
        {
            AdditionalArticles.Clear();

            var lists = _lists.Where(l => l.Type == Dictionary.WordListTypes.AdditionalInfo);
            foreach (Dictionary.WordList list in lists)
            {
                if (Hacks.NeedExcludeAddtionalList(list.TypeId))
                {
                    continue;
                }

                AdditionalArticles.Add(new AdditionalArticlesList(list));
            }
        }

        private void SearchCommandImpl()
        {
            lock (_locker)
            {
                if (_searchQueryTimer.IsRunning)
                {
                    _searchQueryTimer.Stop();
                    StartSearch();
                }
            }
        }
        private void SwitchDirectionCommandImpl()
        {
            lock (_locker)
            {
                SwitchDirection();
            }
        }
        private void HintClickCommandImpl(object parameter)
        {
            lock (_locker)
            {
                DictionaryItem item = parameter as DictionaryItem;
                if (item != null)
                {
                    SearchQuery = item.Title;
                }
            }
        }

        private void StartSearch()
        {
            lock (_locker)
            {
                ResetSearchResults();
                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    SearchHints();
                    SearchItems();

                    if (SearchResults.Count == 0)
                    {
                        SearchSimilar();
                    }

                    if (SearchResults.Count == 0)
                    {
                        ShowNoSearchResultsHint = true;
                    }
                    else
                    {
                        SearchResults[0].IsCurrent = true;
                        ShowNoSearchResultsHint = false;
                    }
                }
            }
        }
        private void SearchHints()
        {
            _itemsController.ResetHints();

            IEnumerable<Dictionary.WordList> dictLists = GetDictionaryListsByLanguage(LanguageFrom.Code);
            foreach (Dictionary.WordList dictList in dictLists)
            {
                var resultList = dictList.WildSearch(SearchQuery, _hintsMaxCount);
                if (resultList.Items.Count == 0)
                {
                    //Здесь должен быть поиск  с морфологией
                }
                if (resultList.Items.Count == 0)
                {
                    
                    resultList = dictList.SpellingSearch(SearchQuery);//Поиск с опечатками
                }
                if (resultList.Items.Count > 0)
                {
                    foreach (Dictionary.ListItem item in resultList.Items)
                    {
                        if (_itemsController.CanAddHint())
                        {
                            if (!_articleService.IsEqualsStrings(item.ShowVariants[Dictionary.ShowVariantsTypes.Show], SearchQuery))
                            {
                                Hints.Add(new DictionaryItem(item));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }
        private void SearchItems()
        {
            IEnumerable<Dictionary.WordList> searchLists = GetSearchLists();
            foreach (Dictionary.WordList searchList in searchLists)
            {
                Dictionary.SearchResultsList resultList;
                if (searchList.Type == Dictionary.WordListTypes.Dictionary)
                {
                    resultList = searchList.NextWordsSearch(SearchQuery, 20);
                }
                else
                {
                    resultList = searchList.Search(SearchQuery);
                }
                foreach (var item in resultList.Items.Where(item => item.ShowVariants.ContainsValue("three-D") &&
                                                                 (SearchQuery.ToLower() == "have" || _searchQuery.ToLower() == "has")))
                {
                    resultList.Items.Remove(item);//Эпичный костыль для контейнера Редхаус.Проблема где-то глубоко в ядре на плюсах
                    break;
                }
                if (resultList.Items.Count > 0)
                {
                    DictionaryListType listType = GetDictionaryListType(searchList.Type);
                    DictionaryList list = new DictionaryList(Utils.GetWordListName(searchList), listType, resultList.Items);
                    list.ItemClicked += ListOnItemClicked;
                    list.HeaderClicked += ListOnHeaderClicked;

                    SearchResults.Add(list);
                }
            }
        }
        private void SearchSimilar()
        {
            IEnumerable<Dictionary.WordList> dictLists = GetDictionaryListsByLanguage(LanguageFrom.Code);
            foreach (Dictionary.WordList dictList in dictLists)
            {
                Dictionary.SearchResultsList resultList = dictList.FuzzySearch(SearchQuery, _didYouMeanMaxCount);
                if (resultList.Items.Count > 0)
                {
                    DictionaryList list = new DictionaryList(_resourcesProvider.GetResource("DidYouMean"), DictionaryListType.Other, resultList.Items);
                    list.ItemClicked += ListOnItemClicked;
                    list.HeaderClicked += ListOnHeaderClicked;

                    SearchResults.Add(list);
                    break;
                }
            }
        }

        private void CheckDirection()
        {
            if (Hacks.NeedDisableAutoSwitchDirection(_dictBaseId))
            {
                return;
            }

            if (LanguageFrom != null && !string.IsNullOrEmpty(_searchQuery))
            {
                if (_dictionary.NeedSwitchDirection(_searchQuery, LanguageFrom))
                {
                    SwitchDirection();
                }
            }
        }
        private void SwitchDirection()
        {
            if (_canSwitchDirection)
            {
                Dictionary.Language languageFrom = LanguageFrom;
                LanguageFrom = LanguageTo;
                LanguageTo = languageFrom;

                if (!string.IsNullOrEmpty(SearchQuery))
                {
                    StartSearch();
                }
            }
        }

        private void ResetSearchResults()
        {
            _searchQueryTimer.Stop();
            Hints.Clear();
            SearchResults.Clear();
        }

        private void SearchQueryTimerOnTick()
        {
            _searchQueryTimer.Stop();
            StartSearch();
        }
        private void ListOnItemClicked(DictionaryItem item)
        {
            Common.Delegate.Call(ItemClicked, item);
        }
        private void ListOnHeaderClicked(DictionaryList sender)
        {
            Common.Delegate.Call(HeaderClicked, sender);
        }

        private IEnumerable<Dictionary.WordList> GetSearchLists()
        {
            List<Dictionary.WordList> fullTextLists = GetFullTextLists().ToList();
            if (fullTextLists.Count != 0)
            {
                return fullTextLists;
            }

            return GetDictionaryListsByLanguage(LanguageFrom.Code);
        }
        private IEnumerable<Dictionary.WordList> GetFullTextLists()
        {
            var ftsLists = from l in _lists
                           where l.Type == Dictionary.WordListTypes.FullTextSearch || l.Type == Dictionary.WordListTypes.FullTextSearchHeadword
                           select l;

            if (_canSwitchDirection)
            {
                return from l in ftsLists
                       where l.LanguageFrom.Code == LanguageFrom.Code
                       select l;
            }

            return ftsLists;
        }
        private IEnumerable<Dictionary.WordList> GetDictionaryLists()
        {
            List<Dictionary.WordList> dictionaryLists = _lists.Where(l => l.Type == Dictionary.WordListTypes.Dictionary).ToList();
            if (dictionaryLists.Count == 0)
            {
                dictionaryLists = _lists.Where(l => l.Type == Dictionary.WordListTypes.InApp).ToList();
            }

            return dictionaryLists;
        }
        private IEnumerable<Dictionary.WordList> GetDictionaryListsByLanguage(uint languageCode)
        {
            return from l in GetDictionaryLists() where l.LanguageFrom.Code == languageCode select l;
        }

        private DictionaryListType GetDictionaryListType(Dictionary.WordListTypes source)
        {
            switch (source)
            {
                case Dictionary.WordListTypes.FullTextSearchHeadword:
                    return DictionaryListType.Headword;

                default:
                    return DictionaryListType.Other;
            }
        }

        private bool _canSwitchDirection = false;
        private bool _showNoSearchResultsHint = true;
        private Dictionary.IDictionary _dictionary;
        private Dictionary.IArticleService _articleService;
        private List<Dictionary.WordList> _lists = new List<Dictionary.WordList>();
        private Dictionary.Language _languageFrom;
        private Dictionary.Language _languageTo;
        private int _searchResultsListsMaxCount;
        private string _searchQuery;
        private string _dictBaseId;
        private readonly Common.IResourcesProvider _resourcesProvider;
        private readonly IItemsController _itemsController;
        private readonly Common.ITimer _searchQueryTimer;
        private readonly object _locker = new object();

        private const int _hintsMaxCount = 20;
        private const int _didYouMeanMaxCount = 12;
        private const int _maxSearchQuerySymbolsCount = 50;
    }
}
