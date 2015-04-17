using System;
using System.Collections.Generic;

namespace Paragon.Container.Core
{
    public interface IArticleRenderingService
    {
        event Action ShowDemoHint;
        event Action<HideBlockTypes> HideBlockOccurred;

        string RenderArticle(DictionaryItem item, List<string> highlightedWords, RenderParameters parameters);
        void PlaySound(Dictionary.ListItemIndex itemIndex, Dictionary.ISoundPlayer soundPlayer);
        void PlayArticleSound(uint soundIndex, Dictionary.ISoundPlayer soundPlayer);
        void Reset();

        bool IsEqualsStrings(string a, string b);
    }
}
