using Cirrious.CrossCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class InformationViewModel : ViewModelBase
    {
        public InformationViewModel()
        {
            GotoInformationCommand.IsExecutable = false;

            _dictionaryService = Mvx.Resolve<Core.IDictionaryService>();
            _parametersManager = Mvx.Resolve<IParametersManager>();
        }

        public override void OnNavigateTo()
        {
            base.OnNavigateTo();

            Lists = _dictionaryService.AdditionalArticles;
            foreach (AdditionalArticlesList list in Lists)
            {
                list.ItemClicked += OnItemClicked;
            }
        }
        public override void OnNavigateFrom()
        {
            base.OnNavigateFrom();

            foreach (AdditionalArticlesList list in Lists)
            {
                list.ItemClicked += OnItemClicked;
            }
        }

        public ObservableCollection<AdditionalArticlesList> Lists { get; private set; }

        private void OnItemClicked(DictionaryItem item)
        {
            _parametersManager.Set(Parameters.RenderingItem, new RenderingItem(item, false));
            ShowViewModel<ArticleViewModel>();
        }

        private readonly Core.IDictionaryService _dictionaryService;
        private readonly IParametersManager _parametersManager;
    }
}
