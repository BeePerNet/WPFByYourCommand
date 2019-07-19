using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFByYourCommand.Behaviors
{
    public static class ItemsControlBehavior
        {
        public static bool GetFocusPreviewMouseRightButtonDown(ItemsControl element)
        {
            return (bool)element.GetValue(FocusPreviewMouseRightButtonDownProperty);
        }

        public static void SetFocusPreviewMouseRightButtonDown(ItemsControl element, bool value)
        {
            element.SetValue(FocusPreviewMouseRightButtonDownProperty, value);
        }


        public static readonly DependencyProperty FocusPreviewMouseRightButtonDownProperty =
            DependencyProperty.RegisterAttached(
            "FocusPreviewMouseRightButtonDown",
            typeof(bool),
            typeof(ItemsControlBehavior),
            new FrameworkPropertyMetadata(false, OnFocusPreviewMouseRightButtonDownChanged));

        static void OnFocusPreviewMouseRightButtonDownChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = depObj as UIElement;
            if (element == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                element.PreviewMouseRightButtonDown += FocusElementPreviewMouseRightButtonDown;
            }
            else
            {
                element.PreviewMouseRightButtonDown -= FocusElementPreviewMouseRightButtonDown;
            }
        }

        static void FocusElementPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Control control = Helper.FindParentControl<Control>(e.OriginalSource as DependencyObject);

            if (control != null)
            {
                control.Focus();
                e.Handled = true;
            }
        }












        public static bool GetRollbackOnUnfocused(ItemsControl datagrid)
        {
            return (bool)datagrid.GetValue(RollbackOnUnfocusedProperty);
        }

        public static void SetRollbackOnUnfocused(ItemsControl datagrid, bool value)
        {
            datagrid.SetValue(RollbackOnUnfocusedProperty, value);
        }

        public static readonly DependencyProperty RollbackOnUnfocusedProperty =
            DependencyProperty.RegisterAttached(
            "RollbackOnUnfocused",
            typeof(bool),
            typeof(ItemsControlBehavior),
            new FrameworkPropertyMetadata(false, OnRollbackOnUnfocusedChanged));

        static void OnRollbackOnUnfocusedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement datagrid = depObj as FrameworkElement;
            if (datagrid == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                datagrid.LostKeyboardFocus += RollbackDataGridOnLostFocus;
                datagrid.DataContextChanged += RollbackDataGridOnDataContextChanged;
            }
            else
            {
                datagrid.LostKeyboardFocus -= RollbackDataGridOnLostFocus;
                datagrid.DataContextChanged -= RollbackDataGridOnDataContextChanged;
            }
        }

        static void RollbackDataGridOnLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ItemsControl senderItemsControl = sender as ItemsControl;
            if (senderItemsControl == null)
                return;

            UIElement focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement == null)
                return;

            ItemsControl focusedDatagrid = Helper.FindParentWithItemPresenter<ItemsControl>(focusedElement); //let's see if the new focused element is inside a datagrid
            if (focusedDatagrid == senderItemsControl)
            {
                return;
                //if the new focused element is inside the same datagrid, then we don't need to do anything;
                //this happens, for instance, when we enter in edit-mode: the DataGrid element loses keyboard-focus, which passes to the selected DataGridCell child
            }

            //otherwise, the focus went outside the datagrid; in order to avoid exceptions like ("DeferRefresh' is not allowed during an AddNew or EditItem transaction")
            //or ("CommitNew is not allowed for this view"), we undo the possible pending changes, if any
            IEditableCollectionView collection = senderItemsControl.Items as IEditableCollectionView;

            if (collection.IsEditingItem)
            {
                collection.CancelEdit();
            }
            else if (collection.IsAddingNew)
            {
                collection.CancelNew();
            }
        }

        static void RollbackDataGridOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl senderItemsControl = sender as ItemsControl;

            if (senderItemsControl == null)
                return;

            IEditableCollectionView collection = senderItemsControl.Items as IEditableCollectionView;

            if (collection.IsEditingItem)
            {
                collection.CancelEdit();
            }
            else if (collection.IsAddingNew)
            {
                collection.CancelNew();
            }
        }




    }
}
