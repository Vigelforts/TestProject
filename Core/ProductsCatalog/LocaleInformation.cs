using System;
using System.Globalization;

namespace Paragon.Container.Core
{
    internal static class LocaleInformation
    {
        public static string GetLocale()
        {
            string letter = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var test = CultureInfo.CurrentCulture;
            switch (letter)
            {
                case "tr":
                    return "Turkish";
                case "de":
                    return "German";

                case "fr":
                    return "French";

                case "es":
                    return "Spanish";

                case "it":
                    return "Italian";

                case "ru":
                    return "Russian";

                default:
                    return "English";
            }
        }
    }
}
