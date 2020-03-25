using System.Windows;
using System.Windows.Controls;

namespace WPFByYourCommand.Themes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<En attente>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "<En attente>")]
    public partial class Expander: ResourceDictionary
    {
        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            ((ContentPresenter)((FrameworkElement)sender).TemplatedParent).HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}
