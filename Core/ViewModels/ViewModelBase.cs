using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using System.Windows.Input;

namespace Paragon.Container.Core.ViewModels
{
    public abstract class ViewModelBase : MvxViewModel
    {
        public ViewModelBase()
        {
            BackCommand = new Common.Mvvm.Command(BackCommandImpl);
            GotoCatalogCommand = new Common.Mvvm.Command(GotoCatalogCommandImpl);
            GotoDictionaryCommand = new Common.Mvvm.Command(GotoDictionaryCommandImpl);
            GotoHistoryCommand = new Common.Mvvm.Command(GotoHistoryCommandImpl);
            GotoFavoritesCommand = new Common.Mvvm.Command(GotoFavoritesCommandImpl);
            GotoInformationCommand = new Common.Mvvm.Command(GotoInformationCommandImpl);

            IDictionaryService dictionaryService = Mvx.Resolve<IDictionaryService>();
            if (dictionaryService.AdditionalArticles.Count == 0)
            {
                GotoInformationCommand.IsExecutable = false;
            }
        }

        public string Header
        {
            get { return _header; }
            protected set
            {
                _header = value;
                RaisePropertyChanged(() => Header);
            }
        }

        public Common.Mvvm.Command BackCommand { get; private set; }
        public Common.Mvvm.Command GotoCatalogCommand { get; private set; }
        public Common.Mvvm.Command GotoDictionaryCommand { get; private set; }
        public Common.Mvvm.Command GotoHistoryCommand { get; private set; }
        public Common.Mvvm.Command GotoFavoritesCommand { get; private set; }
        public Common.Mvvm.Command GotoInformationCommand { get; private set; }

        public virtual void OnNavigateTo()
        {

        }
        public virtual void OnNavigateFrom()
        {

        }

        protected virtual void BackCommandImpl()
        {
            Close(this);
        }

        private void GotoCatalogCommandImpl()
        {
            ShowViewModel<ProductsCatalogViewModel>();
        }
        private void GotoDictionaryCommandImpl()
        {
            ShowViewModel<DictionaryViewModel>();
        }
        private void GotoHistoryCommandImpl()
        {
            ShowViewModel<HistoryViewModel>();
        }
        private void GotoFavoritesCommandImpl()
        {
            ShowViewModel<FavoritesViewModel>();
        }
        private void GotoInformationCommandImpl()
        {
            ShowViewModel<InformationViewModel>();
        }

        private string _header;
    }
}
