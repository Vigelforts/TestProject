using Cirrious.CrossCore;
using Paragon.Container.Core.DataSaving;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Paragon.Container.Core.ViewModels
{
    public sealed class ArticleViewModel : ViewModelBase
    {
        public ArticleViewModel()
        {
            _soundPlayer = Mvx.Resolve<Dictionary.ISoundPlayer>();
            _articleRenderingService = Mvx.Resolve<IArticleRenderingService>();
            _dictionaryService = Mvx.Resolve<IDictionaryService>();
            _parametersManager = Mvx.Resolve<IParametersManager>();
            _historyService = Mvx.Resolve<IHistoryService>();
            _favoritesService = Mvx.Resolve<IFavoritesService>();
            _deviceInformation = Mvx.Resolve<Common.IDeviceInformation>();
            _settingsManager = Mvx.Resolve<Common.ISettingsManager>();

            PlaySoundCommand = new Common.Mvvm.Command(PlaySoundCommandImpl, false);
            FavoriteCommand = new Common.Mvvm.Command(FavoriteCommandImpl, false);
            UnfavoriteCommand = new Common.Mvvm.Command(UnfavoriteCommandImpl, false);
            OpenHideBlocksCommand = new Common.Mvvm.Command(OpenHideBlocksCommandImpl, false);
            CloseHideBlocksCommand = new Common.Mvvm.Command(CloseHideBlocksCommandImpl, false);
            MorphoTableCommand = new Common.Mvvm.Command(MorphoTableCommandImpl, false);
        }

        public event Action HideBlocksOpened;
        public event Action HideBlocksClosed;
        public event Action<string> Scrolled;

        public override void Start()
        {
            _articleRenderingService.ShowDemoHint += ArticleRenderingServiceOnShowDemoHint;
            _articleRenderingService.HideBlockOccurred += ArticleRenderingServiceOnHideBlockOccurred;

            _renderingItem = _parametersManager.Get<RenderingItem>(Parameters.RenderingItem);
            Render(false);
        }
        public override void OnNavigateTo()
        {
            base.OnNavigateTo();

            DudenMorphoTableLogoVisible = false;
        }

        public string ArticleSource
        {
            get
            {
                return _articleSource;
            }
            set
            {
                _articleSource = value;
                RaisePropertyChanged(() => ArticleSource);
            }
        }

        public bool ShowDemoHint
        {
            get { return _showDemoHint; }
            set
            {
                _showDemoHint = value;
                RaisePropertyChanged(() => ShowDemoHint);
            }
        }

        public bool DudenMorphoTableLogoVisible
        {
            get
            {
                return _dudenMorphoTableLogoVisible;
            }
            set
            {
                _dudenMorphoTableLogoVisible = value;
                RaisePropertyChanged(() => DudenMorphoTableLogoVisible);
            }
        }

        public Common.Mvvm.Command PlaySoundCommand { get; private set; }
        public Common.Mvvm.Command FavoriteCommand { get; private set; }
        public Common.Mvvm.Command UnfavoriteCommand { get; private set; }
        public Common.Mvvm.Command OpenHideBlocksCommand { get; private set; }
        public Common.Mvvm.Command CloseHideBlocksCommand { get; private set; }
        public Common.Mvvm.Command MorphoTableCommand { get; private set; }

        public override void OnNavigateFrom()
        {
            base.OnNavigateFrom();
            _articleRenderingService.ShowDemoHint -= ArticleRenderingServiceOnShowDemoHint;
            _articleRenderingService.HideBlockOccurred -= ArticleRenderingServiceOnHideBlockOccurred;
        }

        public void OnScriptNotify(object param)
        {
            try
            {
                string value = param as string;
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                Metadata metadata = Metadata.Parse(value);
                WordOnClick(metadata);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected override void BackCommandImpl()
        {
            DudenMorphoTableLogoVisible = false;

            if (_previousItem == null)
            {
                Close(this);
            }
            else
            {
                _renderingItem = _previousItem;
                _previousItem = null;

                Render(false);
            }
        }

        private void Render(bool isSimpl)
        {
            ShowDemoHint = false;
            SetFavoriteState();
            SetMorphoTableState();
            SetHideBlocksState(HideBlocksStates.None, false);

            RenderParameters renderParameters = GenerateRenderParameters(isSimpl);
            List<string> highlightedWords = new List<string>();

            if (_renderingItem.Type == RenderingItemType.AddtionalItem)
            {
                renderParameters.Columns = false;
                renderParameters.ArticleHeight = 0;

                PlaySoundCommand.IsExecutable = false;
                MorphoTableCommand.IsExecutable = false;
            }
            else
            {
                if (!isSimpl)
                {
                    highlightedWords = GenerateHighlightedWords();
                }
                else
                {
                    MorphoTableCommand.IsExecutable = false;
                }

                PlaySoundCommand.IsExecutable = _renderingItem.Item.HasSound;
            }

            ArticleSource = _articleRenderingService.RenderArticle(_renderingItem.Item, highlightedWords, renderParameters);

            if (_renderingItem.Type != RenderingItemType.AddtionalItem)
            {
                _historyService.AddItem(_renderingItem.Item);
            }
        }
        private void RenderPopup(DictionaryItem item)
        {
            _previousItem = _renderingItem;

            _renderingItem = new RenderingItem(item, false);
            Render(true);

            FavoriteCommand.IsExecutable = false;
            UnfavoriteCommand.IsExecutable = false;
            MorphoTableCommand.IsExecutable = false;
            PlaySoundCommand.IsExecutable = false;
        }

        private RenderParameters GenerateRenderParameters(bool isSimpl)
        {
            RenderParameters renderParameters = new RenderParameters();
            renderParameters.Columns = !isSimpl;
            renderParameters.Crossrefs = !isSimpl;
            renderParameters.HideBlocksState = GetHideBlocksState();

            if (isSimpl)
            {
                renderParameters.ArticleHeight = 0;
            }
            else
            {
                renderParameters.ArticleHeight = _deviceInformation.GetScreenHeight() - 90;
            }

            return renderParameters;
        }
        private List<string> GenerateHighlightedWords()
        {
            if (!_renderingItem.NeedHighlight)
            {
                return null;
            }

            var headwordLists = _dictionaryService.SearchResults.Where(l => l.Type == DictionaryListType.Headword);
            foreach (DictionaryList headwordList in headwordLists)
            {
                if (headwordList.Items.Contains(_renderingItem.Item))
                {
                    return null;
                }
            }

            if (string.IsNullOrEmpty(_dictionaryService.SearchQuery))
            {
                return null;
            }

            List<string> highlightedWords = _dictionaryService.GetAllWordForms(_dictionaryService.SearchQuery);
            highlightedWords.Remove(string.Empty);
            foreach (string word in highlightedWords)
            {
                if (_articleRenderingService.IsEqualsStrings(word, _renderingItem.Item.Title))
                {
                    return null;
                }
            }

            return highlightedWords;
        }
        private bool GetHideBlocksState()
        {
            if (_renderingItem.Type == RenderingItemType.SearchResultItem)
            {
                return true;
            }

            object value = _settingsManager.Get(Settings.HideBlocksState);
            if (value is bool && (bool)value)
            {
                return true;
            }

            return false;
        }

        private void SetFavoriteState()
        {
            if (_renderingItem.Type == RenderingItemType.AddtionalItem || _renderingItem.Item.GetRealItem() == null)
            {
                FavoriteCommand.IsExecutable = false;
                UnfavoriteCommand.IsExecutable = false;
                return;
            }

            if (_favoritesService.HasItem(_renderingItem.Item))
            {
                FavoriteCommand.IsExecutable = false;
                UnfavoriteCommand.IsExecutable = true;
            }
            else
            {
                UnfavoriteCommand.IsExecutable = false;
                FavoriteCommand.IsExecutable = true;
            }
        }
        private void SetMorphoTableState()
        {
            if (_renderingItem.Item.MorphoTableIndex.HasValue)
            {
                MorphoTableCommand.IsExecutable = true;
            }
            else
            {
                MorphoTableCommand.IsExecutable = false;
            }
        }
        private void SetHideBlocksState(HideBlocksStates state, bool needSave)
        {
            _hideBlocksState = state;
            switch (_hideBlocksState)
            {
                case HideBlocksStates.None:
                    {
                        OpenHideBlocksCommand.IsExecutable = false;
                        CloseHideBlocksCommand.IsExecutable = false;
                        break;
                    }

                case HideBlocksStates.Close:
                    {
                        OpenHideBlocksCommand.IsExecutable = true;
                        CloseHideBlocksCommand.IsExecutable = false;
                        
                        if (needSave)
                        {
                            _settingsManager.Set(Settings.HideBlocksState, false);
                        }

                        break;
                    }

                case HideBlocksStates.Open:
                    {
                        OpenHideBlocksCommand.IsExecutable = false;
                        CloseHideBlocksCommand.IsExecutable = true;

                        if (needSave)
                        {
                            _settingsManager.Set(Settings.HideBlocksState, true);
                        }

                        break;
                    }
            }
        }

        private void PlaySoundCommandImpl()
        {
            _articleRenderingService.PlaySound(_renderingItem.Item.Index, _soundPlayer);
        }
        private void FavoriteCommandImpl()
        {
            _favoritesService.AddItem(_renderingItem.Item);
            SetFavoriteState();
        }
        private void UnfavoriteCommandImpl()
        {
            _favoritesService.RemoveItem(_renderingItem.Item);
            SetFavoriteState();
        }
        private void OpenHideBlocksCommandImpl()
        {
            SetHideBlocksState(HideBlocksStates.Open, true);            
            Common.Delegate.Call(HideBlocksOpened);
        }
        private void CloseHideBlocksCommandImpl()
        {
            SetHideBlocksState(HideBlocksStates.Close, true);
            Common.Delegate.Call(HideBlocksClosed);
        }
        private void MorphoTableCommandImpl()
        {
            DictionaryItem morphoTable = _dictionaryService.GetItemByIndex(_renderingItem.Item.MorphoTableIndex.Value);

            if (Hacks.IsBaseWithDudenMorphomagic(morphoTable.Index.BaseId))
            {
                DudenMorphoTableLogoVisible = true;
            }

            RenderPopup(morphoTable);
        }

        private void ArticleRenderingServiceOnShowDemoHint()
        {
            ShowDemoHint = true;
        }
        private void ArticleRenderingServiceOnHideBlockOccurred(HideBlockTypes obj)
        {
            if (_hideBlocksState == HideBlocksStates.None)
            {
                if (GetHideBlocksState())
                {
                    SetHideBlocksState(HideBlocksStates.Open, false);
                }
                else
                {
                    SetHideBlocksState(HideBlocksStates.Close, false);
                }
            }
        }
        private void WordOnClick(Metadata metadata)
        {
            DictionaryItem item = null;
            switch (metadata.Type)
            {
                case MetadataTypes.Text:
                    item = _dictionaryService.GetItemByWord(metadata.Word, metadata.LanguageCode);
                    break;

                case MetadataTypes.Link:
                    Dictionary.ListItemIndex itemIndex = new Dictionary.ListItemIndex(string.Empty, (int)metadata.ListIndex, (int)metadata.EntryIndex, (int)metadata.EntryIndex);
                    item = _dictionaryService.GetItemByIndex(itemIndex);
                    break;

                case MetadataTypes.Sound:
                    _articleRenderingService.PlayArticleSound(metadata.SoundIndex, _soundPlayer);
                    break;

                case MetadataTypes.PopupArticle:
                    Dictionary.ListItemIndex popupItemIndex = new Dictionary.ListItemIndex(string.Empty, (int)metadata.ListIndex, (int)metadata.EntryIndex, (int)metadata.EntryIndex);
                    DictionaryItem popupArticle = _dictionaryService.GetItemByIndex(popupItemIndex);
                    RenderPopup(popupArticle);
                    break;
            }

            if (item != null)
            {
                item.Label = metadata.Label;
                
                if (item.Index.Equals(_renderingItem.Item.Index))
                {
                    Common.Delegate.Call(Scrolled, item.Label);
                }
                else
                {
                    _renderingItem = new RenderingItem(item, false);
                    Render(false);
                }
            }
        }

        private string _articleSource;
        private bool _dudenMorphoTableLogoVisible = false;
        private bool _showDemoHint;
        private RenderingItem _renderingItem;
        private RenderingItem _previousItem;
        private HideBlocksStates _hideBlocksState = HideBlocksStates.None;
        private readonly Dictionary.ISoundPlayer _soundPlayer;
        private readonly IArticleRenderingService _articleRenderingService;
        private readonly IDictionaryService _dictionaryService;
        private readonly IParametersManager _parametersManager;
        private readonly IHistoryService _historyService;
        private readonly IFavoritesService _favoritesService;
        private readonly Common.IDeviceInformation _deviceInformation;
        private readonly Common.ISettingsManager _settingsManager;

        internal enum HideBlocksStates
        {
            None, Open, Close
        }
    }
}
