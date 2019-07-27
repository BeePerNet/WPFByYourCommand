using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace WPFByYourCommand.Behaviors
{
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public class MenuItemTemplateSelector : ItemContainerTemplateSelector
    {
        public MenuItemTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
        {
            return (DataTemplate)parentItemsControl.FindResource(item.GetType());
        }
    }
}
