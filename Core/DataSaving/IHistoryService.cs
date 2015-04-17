using System;
using System.Collections.ObjectModel;

namespace Paragon.Container.Core
{
    public interface IHistoryService
    {
        event Action<DictionaryItem> ItemClicked;

        ObservableCollection<DictionaryItem> Items { get; }
        ObservableCollection<DictionaryItem> TopItems { get; }

        bool ShowHint { get; }

        Common.Mvvm.Command ItemClickCommand { get; }
        Common.Mvvm.Command ClearCommand { get; }

        void AddItem(DictionaryItem item);

        void Reset();
        void UpdateTopItems();
    }
}
