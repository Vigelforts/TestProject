using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Paragon.Container.Views
{
    internal sealed class ListsCountToWightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int)
            {
                int listsCount = (int)value;
                double wight = listsCount * WightСoefficient;
                double result = wight > MinWight ? wight : MinWight;

                return new GridLength(result);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        private const double MinWight = 650.0;
        private const double WightСoefficient = 332.0;
    }
}
