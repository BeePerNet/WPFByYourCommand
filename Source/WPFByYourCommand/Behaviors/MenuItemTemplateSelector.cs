using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFByYourCommand.Commands;

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
