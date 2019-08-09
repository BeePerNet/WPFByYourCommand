using System.Windows;
using System.Windows.Controls;

namespace WPFByYourCommand.Themes
{
    public partial class Expander: ResourceDictionary
    {
        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            ((ContentPresenter)((FrameworkElement)sender).TemplatedParent).HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}
