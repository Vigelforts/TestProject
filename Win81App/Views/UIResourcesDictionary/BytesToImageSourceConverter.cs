using System;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Paragon.Common.UI
{
    public sealed class BytesToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte[] iconData = (byte[])value;
            if (iconData != null)
            {
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    Stream writer = stream.AsStreamForWrite();
                    writer.WriteAsync(iconData, 0, iconData.Length).Wait();
                    writer.FlushAsync().Wait();
                    stream.Seek(0L);

                    BitmapImage source = new BitmapImage();
                    source.SetSource(stream);

                    return source as ImageSource;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
