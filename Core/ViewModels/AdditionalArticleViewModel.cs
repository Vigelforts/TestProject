using Cirrious.MvvmCross.ViewModels;
using System;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class AdditionalArticleViewModel : ViewModelBase
    {
        public AdditionalArticleViewModel()
        {
            BackCommand.IsExecutable = false;
            GotoCatalogCommand.IsExecutable = false;
            GotoDictionaryCommand.IsExecutable = false;
            GotoHistoryCommand.IsExecutable = false;
            GotoFavoritesCommand.IsExecutable = false;
            GotoInformationCommand.IsExecutable = false;
        }
        public new string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged(() => Header);
            }
        }
        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                RaisePropertyChanged(() => Source);
            }
        }

        private string _source;
        private string _header;
    }
}
