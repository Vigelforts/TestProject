using Paragon.Container.Core.ViewModels;
using System;
using Windows.UI.Xaml.Navigation;

namespace Paragon.Container.Views
{
    public sealed partial class InformationView : ViewBase
    {
        public InformationView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.SourcePageType != typeof(ArticleView))
            {
                ClearNavigationCache();
            }
        }

        public new InformationViewModel ViewModel
        {
            get { return (InformationViewModel)base.ViewModel; }
        }

        private void BackButtonOnClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.ClearNavigationCache();
        }
    }
}
