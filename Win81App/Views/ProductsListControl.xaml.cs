using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paragon.Container.Views
{
    public sealed partial class ProductsListControl : UserControl
    {
        public ProductsListControl()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Rect bounds = Window.Current.Bounds;
            this.Width = bounds.Width;
            this.Height = bounds.Height;
            
            Content.Width = this.Width;
        }
    }
}
