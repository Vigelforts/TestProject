using Cirrious.CrossCore;
using Paragon.Container.Core.DataSaving;
using System;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class FavoritesViewModel : ViewModelBase
    {
        public FavoritesViewModel()
        {
            GotoFavoritesCommand.IsExecutable = false;

            _favoritesService = Mvx.Resolve<IFavoritesService>();
            _parametersManager = Mvx.Resolve<IParametersManager>();

            UnfavoriteCommand = new Common.Mvvm.Command(UnfavoriteCommandImpl, false);
        }
        public override void OnNavigateTo()
        {
            base.OnNavigateTo();
            _favoritesService.ItemClicked += OnItemClicked;
        }
        public override void OnNavigateFrom()
        {
            base.OnNavigateTo();
            _favoritesService.ItemClicked -= OnItemClicked;
        }

        public IFavoritesService Favorites
        {
            get { return _favoritesService; }
        }

        public DictionaryItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);

                if (_selectedItem != null)
                {
                    UnfavoriteCommand.IsExecutable = true;
                }
                else
                {
                    UnfavoriteCommand.IsExecutable = false;
                }
            }
        }

        public Common.Mvvm.Command UnfavoriteCommand { get; private set; }

        private void OnItemClicked(DictionaryItem item)
        {
            RenderingItem renderingItem = new RenderingItem(item, false);
            _parametersManager.Set(Parameters.RenderingItem, renderingItem);
            ShowViewModel<ArticleViewModel>();
        }

        private void UnfavoriteCommandImpl()
        {
            if (SelectedItem != null)
            {
                Favorites.RemoveItem(SelectedItem);
            }

            SelectedItem = null;
        }

        private DictionaryItem _selectedItem;
        private readonly IFavoritesService _favoritesService;
        private readonly IParametersManager _parametersManager;
    }
}
