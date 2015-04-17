using Cirrious.CrossCore;
using EngineWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Paragon.Container.Core
{
    internal class ArticleHtmlRenderer : IArticleRenderer, IArticleRenderingService
    {
        public ArticleHtmlRenderer()
        {
            ReadHtmlTemplate();
        }

        public event Action ShowDemoHint;
        public event Action<HideBlockTypes> HideBlockOccurred;

        public string RenderArticle(DictionaryItem item, List<string> highlightedWords, RenderParameters parameters)
        {
            _highlightedWords = highlightedWords;
            _renderParameters = parameters;

            _articleService.RenderArticle(item.Index, item.Label, this);
            return GetArticle();
        }
        public void PlaySound(Dictionary.ListItemIndex itemIndex, Dictionary.ISoundPlayer soundPlayer)
        {
            _articleService.PlaySound(itemIndex, soundPlayer);
        }
        public void PlayArticleSound(uint soundIndex, Dictionary.ISoundPlayer soundPlayer)
        {
            _articleService.PlayInternalSound(soundIndex, soundPlayer);
        }
        public void Reset()
        {
            _articleService = Dictionary.DictionaryFactory.GetArticleService();
            UpdateStyles();
        }
        public bool IsEqualsStrings(string a, string b)
        {
            return _articleService.IsEqualsStrings(a, b);
        }

        public void AddHide(string aText, int aStyle, uint aCloseFlag, string aLabel, uint aHasControl)
        {
            RenderBufferText();

            if (aCloseFlag != 0)
            {
                _article.Append(HtmlBuilder.CloseBlock());
            }
            else
            {
                if (aHasControl != 0)
                {
                    _article.Append(HtmlBuilder.OpenHideBlockWithControl());
                }
                else
                {
                    _article.Append(HtmlBuilder.OpenHideBlock(_renderParameters.HideBlocksState));
                    Common.Delegate.Call(HideBlockOccurred, HideBlockFactory.Create(aLabel));
                }
            }
        }
        public void AddHideControl(string aText, int aStyle, uint aCloseFlag)
        {
            RenderBufferText();

            if (aCloseFlag != 0)
            {
                _article.Append(HtmlBuilder.CloseBlock());
                _isHideControl = false;
            }
            else
            {
                _article.Append(HtmlBuilder.OpenHideControl());
                _isHideControl = true;
            }
        }
        public void AddImage(uint aPictureIndex, uint aFullPictureIndex, int aStyle, uint aCloseFlag, int imageWidth, int imageHeight)
        {
            RenderBufferText();

            if (aCloseFlag != 0)
            {
                return;
            }

            int width;
            int height;
            byte[] buffer = _articleService.GetImage((int)aPictureIndex, out width, out height);
            string base64image = Convert.ToBase64String(buffer);

            _article.Append(HtmlBuilder.CreateImage(base64image, width, height));
        }
        public void AddInfoBlock(string blockType)
        {
            RenderBufferText();

            if (blockType == "InApp")
            {
                Common.Delegate.Call(ShowDemoHint);
                _renderParameters.Columns = false;
            }
        }
        public void AddLabel(string aText, int aStyle, uint aCloseFlag, string aData)
        {
            if (aCloseFlag == 0)
            {
                _article.Append(HtmlBuilder.OpenAnchor(aData));
            }
            else
            {
                _article.Append(HtmlBuilder.CloseAnchor());
            }
        }
        public void AddLink(string aText, int aStyle, uint aCloseFlag, uint aListIndex, uint aEntryIndex, string aTitle, uint aLinkType, string aLabel, uint aSelf)
        {
            RenderBufferText();
            if (aCloseFlag == 0)
            {
                _linkHandler.OpenLink(aListIndex, aEntryIndex, aLabel, false);
            }
            else
            {
                _linkHandler.Reset();
            }
        }
        public void AddParagraph(string aText, int aStyle, uint aCloseFlag, uint aDepth, int aIndent, int aAlign)
        {
            RenderBufferText();

            if (aCloseFlag == 0)
            {
                _article.Append(HtmlBuilder.OpenParagraph((int)aDepth, (WSldTextAlignEnum)aAlign));
            }
            else
            {
                _article.Append(HtmlBuilder.CloseParagraph());
            }
        }
        public void AddPhonetic(string text, int style)
        {
            RenderBufferText();

            _articleStyles.Add(style);
            RenderText(text, style);
        }
        public void AddPopupArticle(string aStr, uint aCloseFlag, uint aListIndex, uint aEntryIndex, string aTitle, string aLabel, uint aExtDictId, int aExtListIdx, string aExtKey)
        {
            RenderBufferText();

            if (aCloseFlag == 0)
            {
                _linkHandler.OpenLink(aListIndex, aEntryIndex, aLabel, true);
            }
            else
            {
                _linkHandler.Reset();
            }
        }
        public void AddPopupImage(string text, int style, uint closeFlag, uint pictureIndex, int imageWidth, int imageHeight)
        {
            Debug.WriteLine("AddPopupImage is called");
        }
        public void AddSound(uint aSoundIndex, int aStyle, uint aCloseFlag, string aLanguage, uint aExtDictId, int aExtListIdx, string aExtKey)
        {
            RenderBufferText();

            if (aCloseFlag == 0)
            {
                _article.Append(HtmlBuilder.CreateSound(new Metadata(MetadataTypes.Sound, aSoundIndex)));
            }
        }
        public void AddTable(string aText, int aStyle, uint aCloseFlag, string aWidth)
        {
            RenderBufferText();

            if (aCloseFlag == 0)
            {
                _article.Append(HtmlBuilder.OpenTable(aWidth));
            }
            else
            {
                _article.Append(HtmlBuilder.CloseTable());
            }
        }
        public void AddTableCol(string aText, int aStyle, uint aCloseFlag, uint aRowSpan, uint aColSpan, string aBgColor, string aBorderStyle, string aBorderSize, string aBorderColor, string aWidth, uint aTextAlign, uint aVerticalTextAlign)
        {
            RenderBufferText();

            if (aCloseFlag == 0)
            {
                int borderWidth = 0;
                int.TryParse(aBorderSize, out borderWidth);

                _article.Append(HtmlBuilder.OpenTableColumn((WSldTextAlignEnum)aTextAlign, (WSldVerticalTextAlignEnum)aVerticalTextAlign, aBgColor,
                    aBorderStyle, aBorderSize, aBorderColor, borderWidth, aWidth));
            }
            else
            {
                _article.Append(HtmlBuilder.CloseTableColumn());
            }
        }
        public void AddTableRow(string aText, int aStyle, uint aCloseFlag)
        {
            RenderBufferText();

            if (aCloseFlag == 0)
            {
                _article.Append(HtmlBuilder.OpenTableRow());
            }
            else
            {
                _article.Append(HtmlBuilder.CloseTableRow());
            }
        }
        public void AddTest(string aText, int aStyle, uint testType, uint closeFlag)
        {
            Debug.WriteLine("AddTest is called");
        }
        public void AddTestInput(string aText, int aStyle, uint inputType, string group, IList<string> answers, string initValue, uint closeFlag)
        {
            Debug.WriteLine("AddTestInput is called");
        }
        public void AddTestToken(int aStyle, uint order, string group, string title, uint closeFlag)
        {
            Debug.WriteLine("AddTestToken is called");
        }
        public void AddText(string text, int style, uint language)
        {
            _articleStyles.Add(style);

            if (IsNewLineSymbol(text))
            {
                RenderBufferText();
                _article.Append(HtmlBuilder.CreateNewLine());
                return;
            }

            TextPart part = new TextPart();
            part.StyleId = style;
            part.Language = language;

            foreach (char ch in text)
            {
                if (_articleService.IsDelimiter(ch))
                {
                    if (part.Text.Length > 0)
                    {
                        _textBuffer.Add(part);
                    }

                    RenderBufferText();
                    RenderText(new string(ch, 1), style);

                    part = new TextPart();
                    part.StyleId = style;
                    part.Language = language;
                }
                else
                {
                    part.Text += ch;
                }
            }

            if (part.Text.Length > 0)
            {
                if (!CanAddPartToBuffer(part))
                {
                    RenderBufferText();
                }

                _textBuffer.Add(part);
            }
        }
        public void AddUrl(string aText, int aStyle, uint closeFlag, string src)
        {
            RenderBufferText();
            if (closeFlag == 0)
            {
                _isHyperlink = true;
                _article.Append(HtmlBuilder.OpenHyperlink(src));
            }
            else
            {
                _isHyperlink = false;
                _article.Append(HtmlBuilder.CloseHyperlink());
            }
        }
        public void EndBuilding(IList<string> aHighlightedWords, bool wordClickable)
        {
            RenderBufferText();
        }
        public string GetArticle()
        {
            _locker.WaitOne();

            StringBuilder articleStyles = new StringBuilder();
            foreach (int styleId in _articleStyles)
            {
                articleStyles.Append(HtmlBuilder.CreateStyle(styleId, _allStyles[styleId]));
            }

            string bodyStyle = "columns";
            if (!_renderParameters.Columns || Hacks.NeedDisableColumns())
            {
                bodyStyle = "simpl";
            }

            string html = _htmlTemplate;
            html = html.Replace("[[styles]]", articleStyles.ToString());
            html = html.Replace("[[label]]", _label);
            html = html.Replace("[[body_style]]", bodyStyle);
            html = html.Replace("[[article_height]]", _renderParameters.ArticleHeight.ToString());
            html = html.Replace("[[body]]", _article.ToString());
            return html;
        }
        public void MarkNewTranslation()
        {
            Debug.WriteLine("MarkNewTranslation is called");
        }
        public void SetCompare(EngineWrapper.WSldCompare compare)
        {
            Debug.WriteLine("SetCompare is called");
        }
        public void SetStyle(uint aIndex, EngineWrapper.WSldStyleInfo aCurrentStyle, double fontSize)
        {
            ArticleFontStyle style = new ArticleFontStyle();
            style.Color = aCurrentStyle.GetColor();
            style.BackgroundColor = aCurrentStyle.GetBackgroundColor();
            style.VerticalAlign = aCurrentStyle.GetLevel();
            style.FontFamily = aCurrentStyle.GetStyleFontFamily();
            style.FontSize = fontSize * _scale;
            style.FontSizeModificator = (WSldStyleSizeEnum)aCurrentStyle.GetTextSize();
            style.IsBold = aCurrentStyle.IsBold() == 0 ? false : true;
            style.IsItalic = aCurrentStyle.IsItalic() == 0 ? false : true;
            style.IsUnderline = aCurrentStyle.IsUnderline() == 0 ? false : true;
            style.IsStrikethrough = aCurrentStyle.IsStrikethrough() == 0 ? false : true;

            _allStyles.Add((int)aIndex, style);
        }
        public void StartBuilding(bool aAllowHeader, bool aNewLineAfterHeader, int entry, int list, string aHighlightText, string aLabel)
        {
            _article.Clear();
            _articleStyles.Clear();
            _linkHandler.Reset();
            _textBuffer.Clear();
            _isHideControl = false;
            _isHyperlink = false;
            _label = aLabel;
        }

        public void SetScale(double scale)
        {
            if (scale > 0)
            {
                _scale = scale;
                UpdateStyles();
            }
        }

        private async void ReadHtmlTemplate()
        {
            _locker.Reset();
            await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        Common.IFileAccessorFactory fileAccessorFactory = Mvx.Resolve<Common.IFileAccessorFactory>();
                        Common.IFileAccessor fileAccessor = fileAccessorFactory.GetResourceFileAccessor();
                        _htmlTemplate = await fileAccessor.ReadFile(new Uri("Assets\\ArticleHtmlTemplate.html", UriKind.Relative));
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

        private bool IsNewLineSymbol(string c)
        {
            return c == "\\%0A" ? true : false;
        }
        private bool CanAddPartToBuffer(TextPart part)
        {
            if (_textBuffer.Count == 0)
            {
                return true;
            }

            ArticleFontStyle a = _allStyles[_textBuffer[0].StyleId];
            ArticleFontStyle b = _allStyles[part.StyleId];
            if (a.FontSize == b.FontSize && a.FontSizeModificator == b.FontSizeModificator && a.VerticalAlign == b.VerticalAlign)
            {
                return true;
            }

            return false;
        }
        private void RenderBufferText()
        {
            StringBuilder fullTextBuilder = new StringBuilder();
            foreach (TextPart part in _textBuffer)
            {
                fullTextBuilder.Append(part.Text);
            }

            string fullText = fullTextBuilder.ToString();

            foreach (TextPart part in _textBuffer)
            {
                RenderText(part.Text, part.StyleId, fullText, part.Language);
            }

            _textBuffer.Clear();
        }
        private void RenderText(string text, int styleId, string fullText = null, uint lang = 0)
        {
            text = ReplaceSymbolsInText(text);

            bool needHighlight = false;
            if (fullText != null)
            {
                fullText = ReplaceSymbolsInText(fullText);
                needHighlight = NeedHighlight(fullText, lang);
            }
            else
            {
                needHighlight = NeedHighlight(text, lang);
            }

            bool isClickable = _renderParameters.Crossrefs && !_isHideControl && !_isHyperlink;

            if (_linkHandler.IsLink)
            {
                MetadataTypes metaType = MetadataTypes.Link;
                if (_linkHandler.IsPopup)
                {
                    metaType = MetadataTypes.PopupArticle;
                }

                Metadata meta = new Metadata(metaType, _linkHandler.ListIndex, _linkHandler.EntryIndex, _linkHandler.Label);
                _article.Append(HtmlBuilder.CreateText(text, styleId, isClickable, needHighlight, meta));
                return;
            }

            if (!string.IsNullOrEmpty(fullText))
            {
                Metadata meta = new Metadata(MetadataTypes.Text, fullText, lang);
                _article.Append(HtmlBuilder.CreateText(text, styleId, isClickable, needHighlight, meta));
                return;
            }

            _article.Append(HtmlBuilder.CreateText(text, styleId, false, needHighlight));
        }
        private string ReplaceSymbolsInText(string text)
        {
            return text.
                Replace((char)834, (char)771);
        }
        private bool NeedHighlight(string text, uint languageCode)
        {
            if (_highlightedWords != null && languageCode != 0)
            {
                foreach (string word in _highlightedWords)
                {
                    if (_articleService.IsEqualsStrings(text, word, languageCode))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void UpdateStyles()
        {
            _allStyles.Clear();
            _articleService.PrepareStyles(this);
        }

        private bool _isHideControl = false;
        private bool _isHyperlink = false;
        private string _label = string.Empty;
        private List<string> _highlightedWords;
        private double _scale = 1;
        private LinkHandler _linkHandler = new LinkHandler();
        private string _htmlTemplate;
        private Dictionary.IArticleService _articleService;
        private RenderParameters _renderParameters;
        private readonly List<TextPart> _textBuffer = new List<TextPart>();
        private readonly StringBuilder _article = new StringBuilder();
        private readonly SortedSet<int> _articleStyles = new SortedSet<int>();
        private readonly Dictionary<int, ArticleFontStyle> _allStyles = new Dictionary<int, ArticleFontStyle>();
        private readonly ManualResetEvent _locker = new ManualResetEvent(true);
    }

    internal class LinkHandler
    {
        public LinkHandler()
        {
            IsLink = false;
        }

        public void OpenLink(uint listIndex, uint entryIndex, string label, bool isPopup)
        {
            IsLink = true;
            ListIndex = listIndex;
            EntryIndex = entryIndex;
            Label = label;
            IsPopup = isPopup;
        }
        public void Reset()
        {
            IsLink = false;
            ListIndex = 0;
            EntryIndex = 0;
            Label = string.Empty;
            IsPopup = false;
        }

        public bool IsLink { get; private set; }
        public uint ListIndex { get; private set; }
        public uint EntryIndex { get; private set; }
        public string Label { get; private set; }
        public bool IsPopup { get; private set; }
    }

    internal class TextPart
    {
        public TextPart()
        {
            Text = string.Empty;
        }

        public string Text { get; set; }
        public int StyleId { get; set; }
        public uint Language { get; set; }
    }
}
