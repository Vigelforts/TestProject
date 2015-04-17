using Paragon.Container.Core.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace Paragon.Container.Views
{
    public sealed partial class AdditionalArticleFlyout : SettingsFlyout
    {
        public AdditionalArticleFlyout()
        {
            this.InitializeComponent();
        }

        public void SetViewModel(AdditionalArticleViewModel viewModel)
        {
            DataContext = viewModel;
        }
    }
}
