using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.WindowsStore.Platform;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Paragon.Container
{
    public class Setup : MvxStoreSetup
    {
        public Setup(Frame rootFrame)
            : base(rootFrame)
        {
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();

            Common.FileDownloader fileDownloader = new Common.FileDownloader();
            fileDownloader.SetRequestHeader("user-agent", string.Format("Windows Store Container app/v{0}", Utils.GetPackageVersion()));

            Mvx.RegisterSingleton<Common.IFileAccessorFactory>(new Common.FileAccessorFactory());
            Mvx.RegisterSingleton<Common.IResourcesProvider>(new Common.ResourcesProvider());
            Mvx.RegisterSingleton<Common.IUIDispatcher>(new Common.UIDispatcher());
            Mvx.RegisterSingleton<Common.IFileDownloader>(fileDownloader);
            Mvx.RegisterSingleton<Common.IInAppService>(new Common.InAppService());
            Mvx.RegisterSingleton<Common.ISettingsManager>(new Common.SettingsManager());
            Mvx.RegisterSingleton<Common.IShortcutManager>(new Common.ShortcutManager());
            Mvx.RegisterSingleton<Common.IDeviceInformation>(new Common.DeviceInformation());
            Mvx.RegisterSingleton<Common.IHttpClient>(new Common.HttpClient());
            Mvx.RegisterSingleton<Core.IItemsController>(new ItemsController());
            Mvx.RegisterSingleton<Core.IUserInteraction>(new UserInteraction());
            Mvx.RegisterSingleton<Core.IAppStylesService>(new AppStylesService());
            Mvx.RegisterSingleton<Dictionary.ISoundPlayer>(new SoundPlayer());
            Mvx.RegisterType<Common.ITimer, Common.Timer>();
        }

        protected override IMvxApplication CreateApp()
        {
            return new Core.App();
        }
    }
}
