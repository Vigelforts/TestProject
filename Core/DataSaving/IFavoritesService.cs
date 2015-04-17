﻿using System;
using System.Collections.ObjectModel;

namespace Paragon.Container.Core.DataSaving
{
    public interface IFavoritesService
    {
        event Action<DictionaryItem> ItemClicked;

        ObservableCollection<DictionaryItem> Items { get; }
        ObservableCollection<DictionaryItem> TopItems { get; }

        bool ShowHint { get; }

        Common.Mvvm.Command ItemClickCommand { get; }
        Common.Mvvm.Command ClearCommand { get; }

        void AddItem(DictionaryItem item);
        bool HasItem(DictionaryItem item);
        void RemoveItem(DictionaryItem item);

        void Reset();
        void UpdateTopItems();
    }
}