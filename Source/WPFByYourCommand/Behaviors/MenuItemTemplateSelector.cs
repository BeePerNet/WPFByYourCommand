using System.Windows;
using System.Windows.Controls;

namespace WPFByYourCommand.Behaviors
{
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
