using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paragon.Container.Core
{
    public enum DictionaryListType
    {
        Headword, Other
    }

    public sealed class DictionaryList : MvxNotifyPropertyChanged
    {
        internal DictionaryList(string name, DictionaryListType type, ICollection<Dictionary.ListItem> items)
        {
            _itemsController = Mvx.Resolve<IItemsController>();

            Name = name;
            Type = type;
            Items = new ObservableCollection<DictionaryItem>();
            TopItems = new ObservableCollection<DictionaryItem>();

            ItemClickCommand = new Common.Mvvm.Command(ItemClickCommandImpl);
            HeaderClickCommand = new Common.Mvvm.Command(HeaderClickCommandImpl);

            Initialize(items);
        }

        public event Action<DictionaryItem> ItemClicked;
        public event Action<DictionaryList> HeaderClicked;

        public string Name { get; private set; }
        public DictionaryListType Type { get; private set; }
        public int ItemsCount
        {
            get { return Items.Count; }
        }
        public bool IsCurrent
        {
            get { return _isCurrent; }
            internal set
            {
                _isCurrent = value;
                RaisePropertyChanged(() => IsCurrent);
            }
        }
        public ObservableCollection<DictionaryItem> Items { get; private set; }
        public ObservableCollection<DictionaryItem> TopItems { get; private set; }

        public Common.Mvvm.Command ItemClickCommand { get; private set; }
        public Common.Mvvm.Command HeaderClickCommand { get; private set; }

        private void Initialize(ICollection<Dictionary.ListItem> items)
        {
            _itemsController.ResetTopItems();
            foreach (Dictionary.ListItem word in items)
            {
                DictionaryItem item = new DictionaryItem(word);
                bool hasSubtitle = !string.IsNullOrEmpty(item.Subtitle);

                Items.Add(item);

                if (_itemsController.CanAddTopItem(hasSubtitle))
                {
                    TopItems.Add(item);
                }
            }
        }

        private void ItemClickCommandImpl(object parameter)
        {
            DictionaryItem item = parameter as DictionaryItem;
            if (item != null)
            {
                Common.Delegate.Call(ItemClicked, item);
            }
        }
        private void HeaderClickCommandImpl()
        {
            Common.Delegate.Call(HeaderClicked, this);
        }

        private bool _isCurrent = false;
        private readonly IItemsController _itemsController;
    }
}
