using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFByYourCommand.Controls;

namespace WPFByYourCommand.Behaviors
{
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public static class ItemsControlBehavior
    {

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

        private static void OnRollbackOnUnfocusedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is ItemsControl control))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                control.LostKeyboardFocus += RollbackOnLostFocus;
                control.DataContextChanged += RollbackOnDataContextChanged;
            }
            else
            {
                control.LostKeyboardFocus -= RollbackOnLostFocus;
                control.DataContextChanged -= RollbackOnDataContextChanged;
            }
        }

        private static void RollbackOnLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(sender is ItemsControl senderItemsControl))
            {
                return;
            }

            if (!(Keyboard.FocusedElement is UIElement focusedElement))
            {
                return;
            }

            ItemsControl focusedDatagrid = ControlsHelper.FindParentWithItemPresenter<ItemsControl>(focusedElement); //let's see if the new focused element is inside a datagrid
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

        private static void RollbackOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ItemsControl senderItemsControl))
            {
                return;
            }

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
