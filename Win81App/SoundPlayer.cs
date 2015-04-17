using Cirrious.CrossCore;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Paragon.Container
{
    internal sealed class SoundPlayer : Dictionary.ISoundPlayer
    {
        public void Play()
        {
            try
            {
                _mediaElement.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public async Task SetSource(byte[] data, uint frequency)
        {
            try
            {
                InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(data.AsBuffer());
                stream.Seek(0);

                _mediaElement.SetSource(stream, string.Format("audio/wav; rate={0}; channels=1", frequency));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public void Stop()
        {
            try
            {
                _mediaElement.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private readonly MediaElement _mediaElement = new MediaElement();
    }
}
