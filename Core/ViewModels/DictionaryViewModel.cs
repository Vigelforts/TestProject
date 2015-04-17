using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Paragon.Container.Core.DataSaving;
using System;
using System.Linq;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class DictionaryViewModel : ViewModelBase
    {
        public DictionaryViewModel()
        {
            BackCommand.IsExecutable = false;
            GotoDictionaryCommand.IsExecutable = false;

            _dictionaryService = Mvx.Resolve<IDictionaryService>();
            _historyService = Mvx.Resolve<IHistoryService>();
            _favoritesService = Mvx.Resolve<IFavoritesService>();
            _parametersManager = Mvx.Resolve<IParametersManager>();

            SearchQuerySubmittedCommand = new Common.Mvvm.Command(SearchQuerySubmittedCommandImpl);
        }
        public override void Start()
        {
            base.Start();

            DictionaryInfo info = _parametersManager.Get<DictionaryInfo>(Parameters.LaunchedProduct);
            Header = info.Name;
        }

        public Common.Mvvm.Command SearchQuerySubmittedCommand { get; private set; }

        public IDictionaryService Dictionary
        {
            get { return _dictionaryService; }
        }
        public IHistoryService History
        {
            get { return _historyService; }
        }
        public IFavoritesService Favorites
        {
            get { return _favoritesService; }
        }

        public override void OnNavigateTo()
        {
            base.OnNavigateTo();
            _dictionaryService.ItemClicked += OnItemClicked;
            _dictionaryService.HeaderClicked += OnListHeaderClicked;
            _historyService.ItemClicked += OnSavedItemClicked;
            _favoritesService.ItemClicked += OnSavedItemClicked;
        }
        public override void OnNavigateFrom()
        {
            base.OnNavigateFrom();
            _dictionaryService.ItemClicked -= OnItemClicked;
            _dictionaryService.HeaderClicked -= OnListHeaderClicked;
            _historyService.ItemClicked -= OnSavedItemClicked;
            _favoritesService.ItemClicked -= OnSavedItemClicked;
        }

        private void SearchQuerySubmittedCommandImpl(object param)
        {
            string query = param as string;
            if (!string.IsNullOrWhiteSpace(query))
            {
                _dictionaryService.SearchCommand.Execute(null);

                _parametersManager.Set<DictionaryList>(Parameters.SelectedSearchResultsList, null);
                ShowViewModel<SearchResultsViewModel>();
            }
        }

        private void OnItemClicked(DictionaryItem item)
        {
            OpenItem(new RenderingItem(item, true));
        }
        private void OnSavedItemClicked(DictionaryItem item)
        {
            OpenItem(new RenderingItem(item, false));
        }
        private void OnListHeaderClicked(DictionaryList sender)
        {
            _parametersManager.Set(Parameters.SelectedSearchResultsList, sender);
            ShowViewModel<SearchResultsViewModel>();
        }

        private void OpenItem(RenderingItem item)
        {
            _parametersManager.Set(Parameters.RenderingItem, item);
            ShowViewModel<ArticleViewModel>();
        }

        private readonly IDictionaryService _dictionaryService;
        private readonly IHistoryService _historyService;
        private readonly IFavoritesService _favoritesService;
        private readonly IParametersManager _parametersManager;
    }
}
