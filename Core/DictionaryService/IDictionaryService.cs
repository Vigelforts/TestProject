using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paragon.Container.Core
{
    public interface IDictionaryService
    {
        event Action<DictionaryItem> ItemClicked;
        event Action<DictionaryList> HeaderClicked;

        int ProductId { get; }

        ObservableCollection<AdditionalArticlesList> AdditionalArticles { get; }

        ObservableCollection<DictionaryItem> Hints { get; }
        ObservableCollection<DictionaryList> SearchResults { get; }

        bool ShowNoSearchResultsHint { get; }

        int SearchResultsListsMaxCount { get; }

        Dictionary.Language LanguageFrom { get; }
        Dictionary.Language LanguageTo { get; }

        Common.Mvvm.Command SearchCommand { get; }
        Common.Mvvm.Command SwitchDirectionCommand { get; }
        Common.Mvvm.Command HintClickCommand { get; }

        string SearchQuery { get; set; }
        int MaxSearchQuerySymbolsCount { get; }

        void Open(DictionaryInfo info);

        DictionaryItem GetItemByIndex(Dictionary.ListItemIndex index);
        DictionaryItem GetItemByWord(string word, uint languageCode);
        List<string> GetAllWordForms(string query);
        Dictionary.WordList GetWordListByIndex(int index);

        Dictionary.ListItemIndex? FindMorphoTable(string word);

        Dictionary.Version GetDictionaryVersion();
        uint GetEngineViersion();
    }
}
