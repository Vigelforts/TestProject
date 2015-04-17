using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paragon.Common.UI
{
    public static class QuerySubmittedCommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand),
            typeof(QuerySubmittedCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SearchBox;
            if (control != null)
            {
                control.QuerySubmitted += OnQuerySubmitted;
            }
        }

        private static void OnQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            var command = GetCommand(sender);

            if (command != null && command.CanExecute(sender.QueryText))
            {
                command.Execute(sender.QueryText);
            }
        }
    }
}
