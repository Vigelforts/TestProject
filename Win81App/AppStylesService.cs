using Paragon.Container.Core;
using Windows.UI;
using Windows.UI.Xaml.Media;

// todo: нельзя менять стиль, который используется
// https://social.msdn.microsoft.com/Forums/silverlight/en-US/46e0430b-873f-4ae2-b14e-def911ecbd5a/edit-or-add-setter-value

namespace Paragon.Container
{
    internal sealed class AppStylesService : IAppStylesService
    {
        public void SetDefaultColors()
        {
            SetPageHeaderBackgroundColor((Color)App.Current.Resources["CatalogPageHeaderBackgroundColor"]);
            SetPageHeaderTextColor((Color)App.Current.Resources["CatalogPageHeaderTextColor"]);
        }
        public void SetColor(string title, string value)
        {
            Color color = GetColorFromString(value);

            switch (title)
            {
                case "PageHeaderBackgroundColor":
                    SetPageHeaderBackgroundColor(color);
                    break;

                case "PageHeaderTextColor":
                    SetPageHeaderTextColor(color);
                    break;
            }
        }

        private void SetPageHeaderBackgroundColor(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            App.Current.Resources["PageHeaderBackgroundBrush"] = brush;
        }
        private void SetPageHeaderTextColor(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            App.Current.Resources["PageHeaderTextBrush"] = brush;
        }

        private Color GetColorFromString(string value)
        {
            string hex = value.Replace("#", "");

            byte pos = 0;

            byte alpha = System.Convert.ToByte("ff", 16);
            if (hex.Length == 8)
            {
                alpha = System.Convert.ToByte(hex.Substring(pos, 2), 16);
                pos = 2;
            }

            byte red = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            pos += 2;
            byte green = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            pos += 2;
            byte blue = System.Convert.ToByte(hex.Substring(pos, 2), 16);

            return Color.FromArgb(alpha, red, green, blue);
        }
    }
}
