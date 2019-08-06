using System;
using System.Windows;

namespace WPFByYourCommand
{
    public static class StylesHelper
    {
        public static void LoadWPFStyles()
        {
            Uri foo = new Uri("pack://application:,,,/WPFByYourCommand;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = foo });
        }

    }
}
