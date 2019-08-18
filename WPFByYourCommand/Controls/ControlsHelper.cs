using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFByYourCommand.Controls
{
    public static class ControlsHelper
    {
        /// <summary>
        /// Find a specific parent object type in the visual tree
        /// </summary>
        public static T FindParentControl<T>(DependencyObject outerDepObj) where T : UIElement
        {
            while ((outerDepObj = VisualTreeHelper.GetParent(outerDepObj)) != null)
            {
                if (outerDepObj is T)
                {
                    return outerDepObj as T;
                }
            }

            return null;
        }

        public static T FindParentWithItemPresenter<T>(UIElement element) where T : UIElement
        {
            UIElement childElement; //element from which to start the tree navigation, looking for a Datagrid parent

            //Maybe we have to add other components
            if (element is ComboBoxItem) //since ComboBoxItem.Parent is null, we must pass through ItemsPresenter in order to get the parent ComboBox
            {
                ItemsPresenter parentItemsPresenter = FindParentControl<ItemsPresenter>(element as ComboBoxItem);
                ComboBox combobox = parentItemsPresenter.TemplatedParent as ComboBox;
                childElement = combobox;
            }
            else
            {
                childElement = element;
            }

            return FindParentControl<T>(childElement); //let's see if the new focused element is inside a datagrid
        }

    }
}
