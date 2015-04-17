using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Paragon.Common;
using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Paragon.Container
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                Setup setup = new Setup(rootFrame);
                setup.Initialize();

                Core.IParametersManager parametersManager = Mvx.Resolve<Core.IParametersManager>();
                parametersManager.Set(Core.Parameters.LaunchedTileId, e.TileId);

                IMvxAppStart app = Mvx.Resolve<IMvxAppStart>();
                app.Start();
            }

            Window.Current.Activate();
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private string GetLocale()
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            while (!string.IsNullOrEmpty(culture.Parent.Name))
            {
                culture = culture.Parent;
            }

            return culture.EnglishName;
        }
    }
}
