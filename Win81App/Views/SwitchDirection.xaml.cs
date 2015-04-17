using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Paragon.Container.Views
{
    public sealed partial class SwitchDirection : UserControl
    {
        public SwitchDirection()
        {
            InitializeComponent();

            LanguageFromProperty = DependencyProperty.Register("LanguageFrom", typeof(Dictionary.Language), typeof(SwitchDirection),
                new PropertyMetadata(0, LanguageFromPropertyChangedCallback));

            LanguageToProperty = DependencyProperty.Register("LanguageTo", typeof(Dictionary.Language), typeof(SwitchDirection),
                new PropertyMetadata(0, LanguageToPropertyChangedCallback));

            SwitchCommandProperty = DependencyProperty.Register("SwitchCommand", typeof(ICommand), typeof(SwitchDirection),
                new PropertyMetadata(0));
        }

        public event Action Click;

        public Dictionary.Language LanguageFrom
        {
            get { return (Dictionary.Language)GetValue(LanguageFromProperty); }
            set { SetValue(LanguageFromProperty, value); }
        }
        public Dictionary.Language LanguageTo
        {
            get { return (Dictionary.Language)GetValue(LanguageToProperty); }
            set { SetValue(LanguageToProperty, value); }
        }
        public ICommand SwitchCommand
        {
            get { return (ICommand)GetValue(SwitchCommandProperty); }
            set { SetValue(SwitchCommandProperty, value); }
        }

        public readonly DependencyProperty LanguageFromProperty;
        public readonly DependencyProperty LanguageToProperty;
        public readonly DependencyProperty SwitchCommandProperty;

        private void LanguageFromPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Dictionary.Language newValue = e.NewValue as Dictionary.Language;
            if (newValue != null)
            {
                image2.Source = GetSourceByLanguage(newValue);
            }
        }
        private void LanguageToPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Dictionary.Language newValue = e.NewValue as Dictionary.Language;
            if (newValue != null)
            {
                image1.Source = GetSourceByLanguage(newValue);
            }
        }

        private void Canvas_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (SwitchCommand != null && SwitchCommand.CanExecute(null))
            {
                SwitchCommand.Execute(null);
            }

            Common.Delegate.Call(Click);
        }

        private ImageSource GetSourceByLanguage(Dictionary.Language language)
        {
            if (!_flags.ContainsKey(language))
            {
                var source = new BitmapImage(new Uri("ms-appx:///Assets/Flags/" + language.Abbreviation + ".png"));
                _flags.Add(language, source);
            }
            return _flags[language];
        }

        private readonly Dictionary<Dictionary.Language, ImageSource> _flags = new Dictionary<Dictionary.Language, ImageSource>();
    }
}
