using Paragon.Container.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

namespace Paragon.Container
{
    internal sealed class UserInteraction : IUserInteraction
    {
        public async Task<bool> ClearHistoryRequest()
        {
            ResourceLoader resources = ResourceLoader.GetForCurrentView();
            return await ShowDialog(resources.GetString("ClearHistoryRequest"), resources.GetString("ClearConfirm"), resources.GetString("Cancel"));
        }
        public async Task<bool> ClearFavoritesRequest()
        {
            ResourceLoader resources = ResourceLoader.GetForCurrentView();
            return await ShowDialog(resources.GetString("ClearFavoritesRequest"), resources.GetString("ClearConfirm"), resources.GetString("Cancel"));
        }
        public async Task RestorePurchaseMessage(List<string> products)
        {
            ResourceLoader resources = ResourceLoader.GetForCurrentView();

            string message;
            if (products.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (string productName in products)
                {
                    builder.Append(productName);
                    builder.Append("\n");
                }

                message = resources.GetString("RestorePurchasesSuccessMessage") + "\n\n" + builder.ToString();
            }
            else
            {
                message = resources.GetString("RestorePurchasesEmptyMessage");
            }

            MessageDialog dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand(resources.GetString("Ok")));
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }
        public async Task ErrorMessage(string message)
        {
            await _uiDispatcher.Execute(async () =>
                {
                    try
                    {
                        ResourceLoader resources = ResourceLoader.GetForCurrentView();

                        string error = message;
                        if (string.IsNullOrEmpty(error))
                        {
                            error = resources.GetString("SomeErrorMessage");
                        }

                        MessageDialog dialog = new MessageDialog(error);
                        dialog.Commands.Add(new UICommand(resources.GetString("Ok")));
                        dialog.DefaultCommandIndex = 0;
                        await dialog.ShowAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
        }
        public async Task InformationMessage(string message)
        {
            await _uiDispatcher.Execute(async () =>
            {
                try
                {
                    ResourceLoader resources = ResourceLoader.GetForCurrentView();

                    MessageDialog dialog = new MessageDialog(message);
                    dialog.Commands.Add(new UICommand(resources.GetString("Ok")));
                    dialog.DefaultCommandIndex = 0;
                    await dialog.ShowAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        private async Task<bool> ShowDialog(string message, string okText, string cancelText)
        {
            bool result = false;

            MessageDialog dialog = new MessageDialog(message);

            dialog.Commands.Add(new UICommand(okText,
                (IUICommand c) =>
                {
                    result = true;
                }));

            dialog.Commands.Add(new UICommand(cancelText,
                (IUICommand c) =>
                {
                    result = false;
                }));

            dialog.DefaultCommandIndex = 1;
            dialog.CancelCommandIndex = 1;

            await dialog.ShowAsync();

            return result;
        }

        private Common.UIDispatcher _uiDispatcher = new Common.UIDispatcher();
    }
}
