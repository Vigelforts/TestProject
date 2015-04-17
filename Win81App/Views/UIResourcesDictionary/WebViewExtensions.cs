using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paragon.Common.UI
{
    public sealed class WebViewExtensions
    {
        public static string GetHTML(DependencyObject obj)
        {
            return (string)obj.GetValue(HTMLProperty);
        }
        public static void SetHTML(DependencyObject obj, string value)
        {
            obj.SetValue(HTMLProperty, value);
        }

        public static readonly DependencyProperty HTMLProperty =
            DependencyProperty.RegisterAttached("HTML", typeof(string), typeof(WebViewExtensions), new PropertyMetadata(0, new PropertyChangedCallback(OnHTMLChanged)));

        private static void OnHTMLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView view = d as WebView;
            string value = e.NewValue as string;
            if (view != null && value != null)
            {
                view.NavigateToString(value);
            }
        } 
    }
}
