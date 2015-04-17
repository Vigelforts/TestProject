using Cirrious.MvvmCross.WindowsStore.Views;
using Paragon.Container.Core.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Paragon.Container.Views
{
    public sealed partial class ArticleView : ViewBase
    {
        public ArticleView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.HideBlocksOpened += ViewModelOnHideBlocksOpened;
            ViewModel.HideBlocksClosed += ViewModelOnHideBlocksClosed;
            ViewModel.Scrolled += ViewModelOnScrolled;

            RemovePreviouseInstanse();
        }
        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.HideBlocksOpened -= ViewModelOnHideBlocksOpened;
            ViewModel.HideBlocksClosed -= ViewModelOnHideBlocksClosed;
        }

        public new ArticleViewModel ViewModel
        {
            get { return (ArticleViewModel)base.ViewModel; }
        }

        private void ArticleHtmlView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            ViewModel.OnScriptNotify(e.Value);
        }

        private async void ViewModelOnHideBlocksOpened()
        {
            await ArticleHtmlView.InvokeScriptAsync("OpenHideBlocks", null);
        }
        private async void ViewModelOnHideBlocksClosed()
        {
            await ArticleHtmlView.InvokeScriptAsync("CloseHideBlocks", null);
        }
        private async void ViewModelOnScrolled(string label)
        {
            if (!string.IsNullOrEmpty(label))
            {
                List<string> arguments = new List<string>();
                arguments.Add(label);

                await ArticleHtmlView.InvokeScriptAsync("ScrollTo", arguments);
            }
        }

        private void RemovePreviouseInstanse()
        {
            PageStackEntry entryToRemove = null;
            foreach (PageStackEntry entry in this.Frame.BackStack)
            {
                if (entry.SourcePageType == typeof(ArticleView))
                {
                    entryToRemove = entry;
                    break;
                }
            }

            if (entryToRemove != null)
            {
                this.Frame.BackStack.Remove(entryToRemove);
            }
        }
    }
}
