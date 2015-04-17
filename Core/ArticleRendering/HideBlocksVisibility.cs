using System;

namespace Paragon.Container.Core
{
    public static class HideBlockFactory
    {
        public static HideBlockTypes Create(string name)
        {
            switch (name)
            {
                case "phon":
                    return HideBlockTypes.Phon;

                case "morph":
                    return HideBlockTypes.Morph;

                case "example":
                    return HideBlockTypes.Example;

                case "phrase":
                    return HideBlockTypes.Phrase;

                default:
                    return HideBlockTypes.Other;
            }
        }
    }

    public enum HideBlockTypes
    {
        Other, Phon, Morph, Example, Phrase
    }

    public struct HideBlocksVisibility
    {
        public bool IsPhonVisible { get; set; }
        public bool IsMorphVisible { get; set; }
        public bool IsExampleVisible { get; set; }
        public bool IsPhraseVisible { get; set; }
    }
}
