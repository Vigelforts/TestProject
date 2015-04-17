using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paragon.Container.Core
{
    public sealed class AdditionalArticlesList : MvxNotifyPropertyChanged
    {
        public AdditionalArticlesList(Dictionary.WordList model)
        {
            _model = model;
            ItemClickCommand = new Common.Mvvm.Command(ItemClickCommandImpl);
            HeaderClickCommand = new Common.Mvvm.Command(HeaderClickCommandImpl);
            Items = new ObservableCollection<DictionaryItem>();

            FillItems();
        }

        public event Action<DictionaryItem> ItemClicked;

        public int Index
        {
            get
            {
                return _model.Index;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }
        public bool IsRoot
        {
            get
            {
                return _isRoot;
            }
            private set
            {
                _isRoot = value;
                RaisePropertyChanged(() => IsRoot);
            }
        }

        public ObservableCollection<DictionaryItem> Items { get; private set; }

        public Common.Mvvm.Command ItemClickCommand { get; private set; }
        public Common.Mvvm.Command HeaderClickCommand { get; private set; }

        private void FillItems()
        {
            Items.Clear();
            foreach (Dictionary.ListItem listItem in _model.Items)
            {
                DictionaryItem item = new DictionaryItem(listItem);
                item.HasSound = false;

                Items.Add(item);
            }

            IsRoot = _model.IsRoot;
            SetName();
        }
        private void SetName()
        {
            if (_model.IsRoot)
            {
                if (Items.Count > 1)
                {
                    Name = Utils.GetWordListName(_model);
                }
            }
            else
            {
                DictionaryItem lastItem = new DictionaryItem(_model.Hierarchy.Last());
                Name = lastItem.Title;
            }
        }

        private void ItemClickCommandImpl(object parameter)
        {
            DictionaryItem item = parameter as DictionaryItem;
            if (item != null)
            {
                if (item.HasHierarchy)
                {
                    _model.GotoSubdirectory(item.GetModel());
                    FillItems();
                }
                else
                {
                    Common.Delegate.Call(ItemClicked, item);
                }
            }
        }
        private void HeaderClickCommandImpl()
        {
            if (!_model.IsRoot)
            {
                _model.GoUp();
                FillItems();
            }
        }

        private string _name;
        private bool _isRoot = true;
        private readonly Dictionary.WordList _model;
    }
}
