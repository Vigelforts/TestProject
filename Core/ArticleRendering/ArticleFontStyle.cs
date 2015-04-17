using EngineWrapper;
using System;

namespace Paragon.Container.Core
{
    internal struct ArticleFontStyle
    {
        public uint Color;
        public uint BackgroundColor;
        public WSldStyleLevelEnum VerticalAlign;
        public WSldStyleFontFamilyEnum FontFamily;
        public double FontSize;
        public WSldStyleSizeEnum FontSizeModificator;
        public bool IsBold;
        public bool IsItalic;
        public bool IsUnderline;
        public bool IsStrikethrough;
    }
}
