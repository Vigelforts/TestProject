using Cirrious.CrossCore;
using Cirrious.MvvmCross.WindowsStore.Views;
using Paragon.Container.Core.ViewModels;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;

namespace Paragon.Container.Views
{
    public abstract class ViewBase : MvxStorePage
    {
        public ViewBase()
        {
            _instance = this;
        }

        public new ViewModelBase ViewModel
        {
            get { return (ViewModelBase)base.ViewModel; }
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e); // TODO: key not found exception
            ViewModel.OnNavigateTo();

            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }
        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.OnNavigateFrom();

            SettingsPane.GetForCurrentView().CommandsRequested -= OnCommandsRequested;
        }
        protected void ClearNavigationCache()
        {
            int cacheSize = Frame.CacheSize;
            Frame.CacheSize = 0;
            Frame.CacheSize = cacheSize;
        }

        protected new void ClearBackStack()
        {
            this.Frame.BackStack.Clear();
        }

        protected virtual void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            Core.IDictionaryService dictionaryService = Mvx.Resolve<Core.IDictionaryService>();
            args.Request.ApplicationCommands.Add(CreateAboutCommand(dictionaryService));
        }

        private SettingsCommand CreateAboutCommand(Core.IDictionaryService dictionaryService)
        {
            ResourceLoader resources = ResourceLoader.GetForCurrentView();
            string aboutCommandTitle = resources.GetString("AboutTitle");

            return new SettingsCommand(aboutCommandTitle, aboutCommandTitle,
                async (handler) =>
                {
                    try
                    {
                        Core.IParametersManager parametersManager = Mvx.Resolve<Core.IParametersManager>();
                        Core.DictionaryInfo info = parametersManager.Get<Core.DictionaryInfo>(Core.Parameters.LaunchedProduct);

                        string aboutText = await Utils.ReadResourceFile("ms-appx:///Resources/ProductAbout.html");
                        string packageVersion = Utils.GetPackageVersion();
                        string engineVersion = dictionaryService.GetEngineViersion().ToString();
                        string databaseVersion = dictionaryService.GetDictionaryVersion().ToString();
                        string productName = info.Name;

                        aboutText = aboutText.Replace("${version}", packageVersion);
                        aboutText = aboutText.Replace("${engine_version}", engineVersion);
                        aboutText = aboutText.Replace("${product_name}", productName);
                        aboutText = aboutText.Replace("${database_version}", databaseVersion);

                        ShowFlyout(aboutCommandTitle, aboutText);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
        }
        private void ShowFlyout(string header, string source)
        {
            AdditionalArticleViewModel viewModel = new AdditionalArticleViewModel();
            viewModel.Header = header;
            viewModel.Source = source;

            AdditionalArticleFlyout flyout = new AdditionalArticleFlyout();
            flyout.SetViewModel(viewModel);
            flyout.Show();
        }

        internal static void Reset()
        {
            if (_instance != null)
            {
                _instance.ClearNavigationCache();
            }
        }

        private static ViewBase _instance;
    }
}
