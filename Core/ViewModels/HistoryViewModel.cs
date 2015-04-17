using Cirrious.CrossCore;
using System;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class HistoryViewModel : ViewModelBase
    {
        public HistoryViewModel()
        {
            GotoHistoryCommand.IsExecutable = false;

            _historyService = Mvx.Resolve<IHistoryService>();
            _parametersManager = Mvx.Resolve<IParametersManager>();
        }
        public override void OnNavigateTo()
        {
            base.OnNavigateTo();
            _historyService.ItemClicked += OnItemClicked;
        }
        public override void OnNavigateFrom()
        {
            base.OnNavigateTo();
            _historyService.ItemClicked -= OnItemClicked;
        }

        public IHistoryService History
        {
            get { return _historyService; }
        }

        private void OnItemClicked(DictionaryItem item)
        {
            RenderingItem renderingItem = new RenderingItem(item, false);
            _parametersManager.Set(Parameters.RenderingItem, renderingItem);
            ShowViewModel<ArticleViewModel>();
        }

        private readonly IHistoryService _historyService;
        private readonly IParametersManager _parametersManager;
    }
}
