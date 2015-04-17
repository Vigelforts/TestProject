using Paragon.Container.Core.ViewModels;
using Paragon.Container.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Networking.Connectivity;
using Windows.Storage;

namespace Paragon.Container
{
    internal static class Utils
    {
        public static string GetPackageVersion()
        {
            PackageVersion version = Package.Current.Id.Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public static async Task<string> ReadResourceFile(string path)
        {
            try
            {
                Uri uri = new Uri(path);
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public static void ShowEnterCodeFlyout()
        {
            EnterCodeFlyout flyout = new EnterCodeFlyout();
            flyout.SetViewModel(new EnterCodeViewModel());
            flyout.Show();
        }
    }
}
