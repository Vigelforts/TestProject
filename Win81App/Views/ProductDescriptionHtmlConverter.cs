using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Paragon.Container.Views
{
    internal sealed class ProductDescriptionHtmlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string html = value as string;
            if (html != null)
            {
                SolidColorBrush brush = (SolidColorBrush)Application.Current.Resources["CatalogBrush"];
                string color = brush.Color.ToString();
                color = color.Remove(1, 2);

                return string.Format(HtmlFormat, color, html);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        private const string HtmlFormat =
            @"<!DOCTYPE html>
                <html>
                    <head>
                        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/>
                        <title>Description</title>
                        <style type=""text/css"" media=""screen"">
                            html {{cursor: default; font-family: Segoe UI; font-size: 16pt; font-weight: 300}}
                        </style>
                        <script type=""text/javascript"">
                            window.onload = function() {{
                                document.onselectstart = function() {{ return false; }}
                            }}
                        </script>
                    </head>
                    <body bgcolor=""{0}"">
                        {1}
                    </body>
                </html>";
    }
}
