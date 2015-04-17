using Paragon.Container.Core.ViewModels;
using System;

namespace Paragon.Container.Views
{
    public sealed partial class FavoritesView : ViewBase
    {
        public FavoritesView()
        {
            this.InitializeComponent();
        }

        public new FavoritesViewModel ViewModel
        {
            get { return (FavoritesViewModel)base.ViewModel; }
        }

        private void ItemsView_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                BottomAppBar.IsOpen = true;
            }
            else
            {
                BottomAppBar.IsOpen = false;
            }
        }

        private void ClearButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BottomAppBar.IsOpen = false;
        }
    }
}
