using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.WindowsStore.Views;
using Paragon.Container.Core.ViewModels;
using System;
using Windows.UI.Xaml.Navigation;

namespace Paragon.Container.Views
{
    public sealed partial class SearchResultsView : ViewBase
    {
        public SearchResultsView()
        {
            this.InitializeComponent();
        }

        public new SearchResultsViewModel ViewModel
        {
            get { return (SearchResultsViewModel)base.ViewModel; }
        }

        private void SearchResultsTabsOnSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            SearchResultsScroller.ScrollToHorizontalOffset(0);
        }
        private void SearchResultsTabsOnRightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
