using Paragon.Container.Core.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace Paragon.Container.Views
{
    public sealed partial class EnterCodeFlyout : SettingsFlyout
    {
        public EnterCodeFlyout()
        {
            this.InitializeComponent();
        }

        public void SetViewModel(EnterCodeViewModel viewModel)
        {
            DataContext = viewModel;
        }
    }
}
