using System;

namespace Paragon.Container.Core
{
    public interface IItemsController
    {
        bool CanAddTopItem(bool hasSubtitle);
        void ResetTopItems();

        bool CanAddHint();
        void ResetHints();

        bool CanAddHistoryTopItem();
        void ResetHistoryTopItems();

        bool CanAddFavoriteTopItem();
        void ResetFavoritesTopItems();
    }
}
