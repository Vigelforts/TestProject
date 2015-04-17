using Cirrious.CrossCore;
using Paragon.Container.Core.ViewModels;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paragon.Container.Views
{
    public sealed partial class ProductsCatalogView : ViewBase
    {
        public ProductsCatalogView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ClearBackStack();
            InformationView.Reset();
            SearchResultsView.Reset();
        }


        public new ProductsCatalogViewModel ViewModel
        {
            get { return (ProductsCatalogViewModel)base.ViewModel; }
        }

        protected override void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            ResourceLoader resources = ResourceLoader.GetForCurrentView();
            
            string aboutCommandTitle = resources.GetString("AboutTitle");
            SettingsCommand aboutCommand = new SettingsCommand(aboutCommandTitle, aboutCommandTitle,
                async (handler) =>
                {
                    try
                    {
                        Core.IDictionaryService dictionaryService = Mvx.Resolve<Core.IDictionaryService>();

                        string aboutText = await Utils.ReadResourceFile("ms-appx:///Resources/ContainerAbout.html");
                        string packageVersion = Utils.GetPackageVersion();
                        string engineVersion = dictionaryService.GetEngineViersion().ToString();

                        aboutText = aboutText.Replace("${version}", packageVersion);
                        aboutText = aboutText.Replace("${engine_version}", engineVersion);

                        AdditionalArticleViewModel viewModel = new AdditionalArticleViewModel();
                        viewModel.Header = aboutCommandTitle;
                        viewModel.Source = aboutText;

                        AdditionalArticleFlyout flyout = new AdditionalArticleFlyout();
                        flyout.SetViewModel(viewModel);
                        flyout.Show();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
            args.Request.ApplicationCommands.Add(aboutCommand);

            string enterCodeCommandTitle = resources.GetString("EnterCodeTitle");
            SettingsCommand enterCodeCommand = new SettingsCommand(enterCodeCommandTitle, enterCodeCommandTitle,
                (handler) =>
                {
                    Utils.ShowEnterCodeFlyout();
                });
            args.Request.ApplicationCommands.Add(enterCodeCommand);
        }

        private void EnterCodeButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Utils.ShowEnterCodeFlyout();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (LanguageFilterPanel.Visibility == Visibility.Visible)
            {
                ProductsScrollViewer.Margin = new Thickness(5, 50, 0, 0);
            }
            else
            {
                ProductsScrollViewer.Margin = new Thickness(5, 10, 0, 0);
            }
        }
    }
}
