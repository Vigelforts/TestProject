using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paragon.Container.Views
{
    class WordItemTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            Core.DictionaryItem wordItem = item as Core.DictionaryItem;
            if (wordItem != null)
            {
                if (!string.IsNullOrEmpty(wordItem.Subtitle))
                {
                    return Application.Current.Resources["WordItemWithSubtitleDataTemplate"] as DataTemplate;
                }

                if (!string.IsNullOrEmpty(wordItem.PartOfSpeech))
                {
                    return Application.Current.Resources["WordItemWithPartOfSpeechSDataTemplate"] as DataTemplate;
                }
            }

            return Application.Current.Resources["WordItemDataTemplate"] as DataTemplate;
        }
    }
}
