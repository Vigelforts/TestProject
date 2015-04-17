using Paragon.Container.Core.ViewModels;
using System;

namespace Paragon.Container.Views
{
    public sealed partial class HistoryView : ViewBase
    {
        public HistoryView()
        {
            this.InitializeComponent();
        }

        public new HistoryViewModel ViewModel
        {
            get { return (HistoryViewModel)base.ViewModel; }
        }
    }
}
