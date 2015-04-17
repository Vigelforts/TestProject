using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Paragon.Container.Core.DataSaving.DataAccessors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Paragon.Container.Core.DataSaving
{
    internal sealed class FavoritesService : MvxNotifyPropertyChanged, IFavoritesService
    {
        internal FavoritesService()
        {
            _dictionaryService = Mvx.Resolve<IDictionaryService>();
            _userInteraction = Mvx.Resolve<IUserInteraction>();
            _itemsController = Mvx.Resolve<IItemsController>();
            _parametersManager = Mvx.Resolve<IParametersManager>();
            _uiIDispatcher = Mvx.Resolve<Common.IUIDispatcher>();

            ItemClickCommand = new Common.Mvvm.Command(ItemClickCommandImpl);
            ClearCommand = new Common.Mvvm.Command(ClearCommandImpl, false);

            Initialize();
        }

        public event Action<DictionaryItem> ItemClicked;

        public ObservableCollection<DictionaryItem> Items
        {
            get
            {
                _locker.WaitOne();
                return _items;
            }
            private set
            {
                _items = value;
            }
        }
        public ObservableCollection<DictionaryItem> TopItems
        {
            get
            {
                _locker.WaitOne();
                return _topItems;
            }
        }

        public bool ShowHint
        {
            get
            {
                return _showHint;
            }
            private set
            {
                _showHint = value;
                RaisePropertyChanged(() => ShowHint);
            }
        }

        public Common.Mvvm.Command ItemClickCommand { get; private set; }
        public Common.Mvvm.Command ClearCommand { get; private set; }

        public void AddItem(DictionaryItem item)
        {
            _locker.WaitOne();
            lock (_locker)
            {
                try
                {
                    DictionaryItem realItem = item.GetRealItem();
                    if (realItem == null || Items.Contains(realItem))
                    {
                        return;
                    }

                    if (Items.Count == _maxItemsCount)
                    {
                        Items.RemoveAt(_maxItemsCount - 1);
                    }

                    Items.Add(realItem);
                    SortItems();

                    WriteData();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        public bool HasItem(DictionaryItem item)
        {
            _locker.WaitOne();
            lock (_locker)
            {
                DictionaryItem realItem = item.GetRealItem();
                return Items.Contains(realItem);
            }
        }
        public void RemoveItem(DictionaryItem item)
        {
            _locker.WaitOne();
            lock (_locker)
            {
                try
                {
                    DictionaryItem realItem = item.GetRealItem();
                    if (realItem == null || !Items.Contains(realItem))
                    {
                        return;
                    }

                    Items.Remove(realItem);
                    WriteData();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void Reset()
        {
            _locker.WaitOne();
            lock (_locker)
            {
                _dataAccessorV2 = null;

                Items = null;
                TopItems.Clear();

                ShowHint = true;

                Initialize();
            }
        }
        public void UpdateTopItems()
        {
            _locker.WaitOne();
            lock (_locker)
            {
                FillTopItems();
            }
        }

        public const string FileNameFormat = "favorites{0}_{1}.dat";

        private void Initialize()
        {
            DictionaryInfo info = _parametersManager.Get<DictionaryInfo>(Parameters.LaunchedProduct);
            Uri fileUri = new Uri(string.Format(FileNameFormat, Data.FormatVersion, info.Id), UriKind.Relative);
            _dataAccessorV2 = new DataAccessorV2(fileUri);

            ReadData();
        }

        private void FillTopItems()
        {
            TopItems.Clear();
            _itemsController.ResetFavoritesTopItems();

            foreach (DictionaryItem item in _items)
            {
                if (_itemsController.CanAddFavoriteTopItem())
                {
                    TopItems.Add(item);
                }
                else
                {
                    break;
                }
            }
        }

        private async void ReadData()
        {
            _locker.Reset();
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    IReadOnlyList<DictionaryItem> data = await _dataAccessorV2.ReadData();
                    _items = new ObservableCollection<DictionaryItem>(data);
                    _items.CollectionChanged += ItemsOnCollectionChanged;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    _locker.Set();
                }

                UpdateState();
            });
        }
        private async void WriteData()
        {
            _locker.Reset();
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _dataAccessorV2.WriteData(_items);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    _locker.Set();
                }
            });
        }

        private void SortItems()
        {
            Items.CollectionChanged -= ItemsOnCollectionChanged;

            var sortedItems = this.Items.OrderBy(i => i.Title).Select(i => i).ToList();
            Items.Clear();
            foreach (DictionaryItem item in sortedItems)
            {
                Items.Add(item);
            }

            FillTopItems();

            Items.CollectionChanged += ItemsOnCollectionChanged;
        }

        private void UpdateState()
        {
            if (_items != null && _items.Count > 0)
            {
                ShowHint = false;
                ClearCommand.IsExecutable = true;
            }
            else
            {
                ShowHint = true;
                ClearCommand.IsExecutable = false;
            }
        }

        private void ItemsOnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FillTopItems();
            UpdateState();
        }

        private void ItemClickCommandImpl(object parameter)
        {
            _locker.WaitOne();

            DictionaryItem item = parameter as DictionaryItem;
            if (item != null && _items.Contains(item))
            {
                Common.Delegate.Call(ItemClicked, item);
            }
        }
        private async void ClearCommandImpl()
        {
            _locker.WaitOne();

            bool clearRequestResult = await _userInteraction.ClearFavoritesRequest();
            if (clearRequestResult)
            {
                Items.Clear();
                WriteData();
            }
        }

        private bool _showHint = true;
        private ObservableCollection<DictionaryItem> _items;
        private readonly ObservableCollection<DictionaryItem> _topItems = new ObservableCollection<DictionaryItem>();
        private DataAccessorV2 _dataAccessorV2;
        private readonly IDictionaryService _dictionaryService;
        private readonly IUserInteraction _userInteraction;
        private readonly IParametersManager _parametersManager;
        private readonly IItemsController _itemsController;
        private readonly Common.IUIDispatcher _uiIDispatcher;
        private readonly ManualResetEvent _locker = new ManualResetEvent(true);

        private const int _maxItemsCount = 200;
    }
}
