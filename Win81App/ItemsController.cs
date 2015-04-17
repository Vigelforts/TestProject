using System;
using Windows.UI.Xaml;

namespace Paragon.Container
{
    internal class ItemsController : Core.IItemsController
    {
        public ItemsController()
        {
            _itemHeight = (double)Application.Current.Resources["WordItemHeight"];
            _itemWithSubtitleHeight = (double)Application.Current.Resources["WordItemWithSubtitleHeight"];
        }

        public bool CanAddTopItem(bool hasSubtitle)
        {
            if (_listHeiht <= 0)
            {
                return false;
            }

            double currentItemHeight = (hasSubtitle ? _itemWithSubtitleHeight : _itemHeight) + ItemIndent;

            if (_currentListHeiht + currentItemHeight <= _listHeiht)
            {
                _currentListHeiht += currentItemHeight;
                return true;
            }

            return false;
        }
        public void ResetTopItems()
        {
            _currentListHeiht = 0;
        }
        
        public bool CanAddHint()
        {
            return true;
        }
        public void ResetHints()
        {
        }

        public bool CanAddHistoryTopItem()
        {
            if (_historyListHeiht <= 0)
            {
                return false;
            }

            double currentItemHeight = _itemHeight + ItemIndent;

            if (_currentHistoryListHeiht + currentItemHeight <= _historyListHeiht)
            {
                _currentHistoryListHeiht += currentItemHeight;
                return true;
            }

            return false;
        }
        public void ResetHistoryTopItems()
        {
            _currentHistoryListHeiht = 0;
        }

        public bool CanAddFavoriteTopItem()
        {
            if (_favoritesListHeiht <= 0)
            {
                return false;
            }

            double currentItemHeight = _itemHeight + ItemIndent;

            if (_currentFavoritesListHeiht + currentItemHeight <= _favoritesListHeiht)
            {
                _currentFavoritesListHeiht += currentItemHeight;
                return true;
            }

            return false;
        }
        public void ResetFavoritesTopItems()
        {
            _currentFavoritesListHeiht = 0;
        }

        public static void SetListHeight(double value)
        {
            _listHeiht = value;
        }
        public static void SetHistoryListHeight(double value)
        {
            _historyListHeiht = value;
        }
        public static void SetFavoritesListHeight(double value)
        {
            _favoritesListHeiht = value;
        }

        private double _currentListHeiht = 0;
        private double _currentHistoryListHeiht = 0;
        private double _currentFavoritesListHeiht = 0;

        private readonly double _itemHeight;
        private readonly double _itemWithSubtitleHeight;

        private static double _listHeiht = 0;
        private static double _historyListHeiht = 0;
        private static double _favoritesListHeiht = 0;

        private const double ItemIndent = 10;
    }
}
