using Cirrious.CrossCore;
using System;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class EnterCodeViewModel : ViewModelBase
    {
        public EnterCodeViewModel()
        {
            _productsCatalog = Mvx.Resolve<IProductsCatalog>();

            BackCommand.IsExecutable = false;
            GotoCatalogCommand.IsExecutable = false;
            GotoDictionaryCommand.IsExecutable = false;
            GotoHistoryCommand.IsExecutable = false;
            GotoFavoritesCommand.IsExecutable = false;
            GotoInformationCommand.IsExecutable = false;

            OkCommand = new Common.Mvvm.Command(OkCommandImpl);

            SetHeader();
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }

        public Common.Mvvm.Command OkCommand { get; private set; }

        private void SetHeader()
        {
            Common.IResourcesProvider resourcesProvider = Mvx.Resolve<Common.IResourcesProvider>();
            Header = resourcesProvider.GetResource("EnterCodeTitle");
        }

        private async void OkCommandImpl()
        {
            OkCommand.IsExecutable = false;
            bool isCodeValid = await _productsCatalog.EnterCode(Code);
            OkCommand.IsExecutable = true;

            if (isCodeValid)
            {
                Code = string.Empty;
            }
        }

        private readonly IProductsCatalog _productsCatalog;

        private static string _code;
    }
}
