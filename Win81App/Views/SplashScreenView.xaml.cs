using Paragon.Container.Core.ViewModels;
using System;
using Windows.UI.Xaml.Navigation;

namespace Paragon.Container.Views
{
    public sealed partial class SplashScreenView : ViewBase
    {
        public SplashScreenView()
        {
            this.InitializeComponent();
        }

        public new SplashScreenViewModel ViewModel
        {
            get { return (SplashScreenViewModel)base.ViewModel; }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Progress.IsActive = false;
        }
    }
}
