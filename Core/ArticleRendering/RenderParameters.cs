using System;

namespace Paragon.Container.Core
{
    public struct RenderParameters
    {
        public bool Columns { get; set; }
        public bool Crossrefs { get; set; }
        public bool HideBlocksState { get; set; }
        public int ArticleHeight { get; set; }
    }
}
