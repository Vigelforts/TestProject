using EngineWrapper;
using System;
using System.Text;

namespace Paragon.Container.Core
{
    internal static class HtmlBuilder
    {
        public static string CreateText(string text, int styleId, bool isClickable, bool isHighlighted, Metadata meta = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string onClickHandler = string.Empty;
            if (isClickable)
            {
                onClickHandler = string.Format(@"onclick=""textTapped('{0}')""",
                    meta.ToString());
            }

            string result = string.Format(@"<span {0} class=""s{1}"">{2}</span>",
                onClickHandler, styleId, text);

            if (isHighlighted)
            {
                result = string.Format(@"<span class=""highlighted"">{0}</span>", result);
            }

            return result;
        }
        public static string CreateNewLine()
        {
            return "<br/>";
        }

        public static string OpenAnchor(string id)
        {
            return string.Format(@"<a id=""{0}"">", id);
        }
        public static string CloseAnchor()
        {
            return "</a>";
        }

        public static string OpenParagraph(int marginLeft, WSldTextAlignEnum textAlign)
        {
            return string.Format(@"<div style=""margin-left: {0}em; text-align: {1};"">",
                marginLeft < 0 ? 0 : marginLeft, GetTextAlign(textAlign));
        }
        public static string CloseParagraph()
        {
            return "</div>";
        }

        public static string OpenTable(string width)
        {
            string tableWidth = "auto";
            if (width.ToLower() == "full")
            {
                tableWidth = "100%";
            }

            return string.Format(@"<table cellpadding=""2%"" style=""border-collapse: collapse; width: {0}"">", tableWidth);
        }
        public static string CloseTable()
        {
            return "</table>";
        }
        public static string OpenTableColumn(WSldTextAlignEnum textAlign, WSldVerticalTextAlignEnum verticalAlign, string bgColor, string borderStyle,
            string borderSize, string borderColor, int borderWidth, string width)
        {
            return string.Format(@"<td style=""text-align: {0}; vertical-align: {1}; border-style: {2}; border-width: {3}px; width: {4}; border-color: #{5}; background: #{6}; border-width: {7}"">",
                GetTextAlign(textAlign), GetVerticalAlign(verticalAlign), borderStyle, borderWidth, width, borderColor, bgColor, borderWidth);
        }
        public static string CloseTableColumn()
        {
            return "</td>";
        }
        public static string OpenTableRow()
        {
            return "<tr>";
        }
        public static string CloseTableRow()
        {
            return "</tr>";
        }

        public static string CreateImage(string base64image, int width, int height)
        {
            return string.Format(@"<img style=""max-width: 100%; height: auto;"" src=""data:image/png;base64,{0}""/>",
                base64image);
        }

        public static string CreateSound(Metadata meta)
        {
            return string.Format(@"<img onclick=""textTapped('{0}')"" width=28 height=28 src=""data:image/png;base64,{1}""/>",
                meta.ToString(), Images.SoundImage);
        }

        public static string OpenHideBlock(bool opened)
        {
            string state = opened ? "" : @"style=""display: none;""";

            return string.Format(@"<span class=""hideBlock"" {0}>", state);
        }
        public static string OpenHideBlockWithControl()
        {
            return @"<span class=""hideBlockWithControl"">";
        }
        public static string OpenHideControl()
        {
            return @"<span class=""hideControl"" onclick=""hideControlTapped(this)"">";
        }
        public static string CloseBlock()
        {
            return "</span>";
        }

        public static string OpenHyperlink(string url)
        {
            return string.Format(@"<a href=""{0}"" target=""_blank"">", url);
        }
        public static string CloseHyperlink()
        {
            return "</a>";
        }

        public static string CreateStyle(int id, ArticleFontStyle style)
        {
            string backgroundColor = GetColor(style.BackgroundColor);
            string backgroundColorStyle = string.Empty;
            if (backgroundColor != "#FFFFFF")
            {
                backgroundColorStyle = string.Format("background-color: {0};", backgroundColor);
            }

            return string.Format("span.s{0} {{ color: {1}; vertical-align: {2}; font-family: {3}; font-size: {4}pt; font-weight: {5}; font-style: {6}; text-decoration: {7}; {8} }}\n",
                id, GetColor(style.Color), GetVerticalAlign(style.VerticalAlign), GetFontFamily(style.FontFamily), GetFontSize(style.FontSize, style.FontSizeModificator),
                GetFontWeight(style.IsBold), GetFontStyle(style.IsItalic), GetTextDecoration(style.IsUnderline, style.IsStrikethrough), backgroundColorStyle);
        }

        private static string GetTextAlign(WSldTextAlignEnum src)
        {
            switch (src)
            {
                case WSldTextAlignEnum.eTextAlign_Center:
                    return "center";

                case WSldTextAlignEnum.eTextAlign_Justify:
                    return "justify";

                case WSldTextAlignEnum.eTextAlign_Right:
                    return "right";

                default:
                    return "left";
            }
        }
        private static string GetVerticalAlign(WSldVerticalTextAlignEnum src)
        {
            switch (src)
            {
                case WSldVerticalTextAlignEnum.eVerticalTextAlign_Bottom:
                    return "bottom";

                case WSldVerticalTextAlignEnum.eVerticalTextAlign_Center:
                    return "middle";

                default:
                    return "top";
            }
        }
        private static string GetVerticalAlign(WSldStyleLevelEnum src)
        {
            switch (src)
            {
                case WSldStyleLevelEnum.eLevelSup:
                    return "super";

                case WSldStyleLevelEnum.eLevelSub:
                    return "sub";

                default:
                    return "baseline";
            }
        }
        private static string GetColor(uint src)
        {
            return string.Format("#{0:X6}", src & 0xFFffFF);
        }
        private static string GetFontFamily(WSldStyleFontFamilyEnum src)
        {
            switch (src)
            {
                case WSldStyleFontFamilyEnum.eFontFamily_Fantasy:
                    return "Fantasy";

                case WSldStyleFontFamilyEnum.eFontFamily_Monospace:
                    return "Monospace";

                case WSldStyleFontFamilyEnum.eFontFamily_Serif:
                    return "Serif";

                default:
                    return "Segoe UI";
            }
        }
        private static int GetFontSize(double size, WSldStyleSizeEnum modificator)
        {
            double result = size;
            switch (modificator)
            {
                case WSldStyleSizeEnum.eSizeLarge:
                    result = size * 2;
                    break;

                case WSldStyleSizeEnum.eSizeSmall:
                    result = size * 0.75;
                    break;
            }

            return (int)result;
        }
        private static string GetFontWeight(bool src)
        {
            return src ? "bold" : "normal";
        }
        private static string GetFontStyle(bool src)
        {
            return src ? "italic" : "normal";
        }
        private static string GetTextDecoration(bool isUnderline, bool isStrikethrough)
        {
            if (isUnderline)
            {
                return "underline";
            }

            if (isStrikethrough)
            {
                return "line-through";
            }

            return "none";
        }
    }
}
