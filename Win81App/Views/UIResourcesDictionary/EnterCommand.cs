using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Paragon.Common.UI
{
    public static class EnterCommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand),
            typeof(EnterCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

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
            var control = d as TextBox;
            if (control != null)
            {
                control.KeyDown += OnKeyDown;
            }
        }

        private static void OnKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var control = sender as TextBox;
                if (control != null)
                {
                    string text = control.Text;

                    var command = GetCommand(control);
                    if (command != null && command.CanExecute(text))
                    {
                        command.Execute(text);
                    }
                }
            }
        }
    }
}
