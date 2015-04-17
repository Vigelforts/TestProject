using Cirrious.MvvmCross.WindowsStore.Views;
using Paragon.Container.Core.ViewModels;
using Windows.UI.Xaml;

namespace Paragon.Container.Views
{
    public sealed partial class DictionaryView : ViewBase
    {
        public DictionaryView()
        {
            this.InitializeComponent();
            this.KeyDown += DictionaryViewOnKeyDown;
        }

        public new DictionaryViewModel ViewModel
        {
            get { return (DictionaryViewModel)base.ViewModel; }
        }

        private void SearchResultsLists_Loaded(object sender, RoutedEventArgs e)
        {
            double listHeight = SearchResultsLists.ActualHeight - SearchResultsItemIndent;
            ItemsController.SetListHeight(listHeight);
        }
        private void HistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            double listHeight = HistoryView.ActualHeight - AdditionalItemIndent;
            ItemsController.SetHistoryListHeight(listHeight);

            ViewModel.History.UpdateTopItems();
        }
        private void FavoritesView_Loaded(object sender, RoutedEventArgs e)
        {
            double listHeight = FavoritesView.ActualHeight - AdditionalItemIndent;
            ItemsController.SetFavoritesListHeight(listHeight);

            ViewModel.Favorites.UpdateTopItems();
        }

        private void DictionaryViewOnKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Tab)
            {
                SearchBox.Focus(FocusState.Keyboard);
                e.Handled = true;
            }
        }
        private void DictionaryViewOnLoaded(object sender, RoutedEventArgs e)
        {
            SearchBox.Select(SearchBox.Text.Length, 0);
        }

        private const double SearchResultsItemIndent = 25;
        private const double AdditionalItemIndent = 50;
    }
}
