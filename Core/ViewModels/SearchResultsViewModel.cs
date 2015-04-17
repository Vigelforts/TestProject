using Cirrious.CrossCore;
using System;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class SearchResultsViewModel : ViewModelBase
    {
        public SearchResultsViewModel()
        {
            _dictionary = Mvx.Resolve<IDictionaryService>();
            _parametersManager = Mvx.Resolve<IParametersManager>();
            _resourcesProvider = Mvx.Resolve<Common.IResourcesProvider>();
        }

        public IDictionaryService Dictionary
        {
            get { return _dictionary; }
        }

        public DictionaryList CurrentList
        {
            get { return _currentList; }
            set
            {
                _currentList = value;
                _parametersManager.Set(Parameters.SelectedSearchResultsList, _currentList);
                RaisePropertyChanged(() => CurrentList);
            }
        }

        public override void OnNavigateTo()
        {
            _dictionary.ItemClicked += DictionaryOnItemClicked;

            Header = string.Format(@"{0} ""{1}""", _resourcesProvider.GetResource("SearchResultsLabel"), _dictionary.SearchQuery);

            DictionaryList selectedList = _parametersManager.Get<DictionaryList>(Parameters.SelectedSearchResultsList);
            if (selectedList != null)
            {
                CurrentList = selectedList;
            }
            else if (Dictionary.SearchResults.Count > 0)
            {
                CurrentList = Dictionary.SearchResults[0];
            }
            else
            {
                CurrentList = null;
            }
        }
        public override void OnNavigateFrom()
        {
            _dictionary.ItemClicked -= DictionaryOnItemClicked;
        }

        private void DictionaryOnItemClicked(DictionaryItem item)
        {
            RenderingItem renderingItem = new RenderingItem(item, true);
            _parametersManager.Set(Parameters.RenderingItem, renderingItem);
            ShowViewModel<ArticleViewModel>();
        }

        private DictionaryList _currentList;
        private readonly IDictionaryService _dictionary;
        private readonly IParametersManager _parametersManager;
        private readonly Common.IResourcesProvider _resourcesProvider;
    }
}
