using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Paragon.Common.UI
{
    public sealed class BooleanToVisibilityInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                bool val = (bool)value;
                return val ? Visibility.Collapsed : Visibility.Visible;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
