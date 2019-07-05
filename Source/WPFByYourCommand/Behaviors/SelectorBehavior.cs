using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPFByYourCommand.Behaviors
{
    /// <summary>
    /// Exposes attached behaviors that can be
    /// applied to TreeViewItem objects.
    /// </summary>
    public static class SelectorBehavior
    {
        #region IsBroughtIntoViewWhenSelected

        public static bool GetIsBroughtIntoViewWhenSelected(Selector selector)
        {
            return (bool)selector.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        }

        public static void SetIsBroughtIntoViewWhenSelected(
          Selector selector, bool value)
        {
            selector.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached(
            "IsBroughtIntoViewWhenSelected",
            typeof(bool),
            typeof(SelectorBehavior),
            new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectionChanged));

        static void OnIsBroughtIntoViewWhenSelectionChanged(
          DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Selector item = depObj as Selector;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                item.SelectionChanged += OnSelectorSelectionChanged;
            else
                item.SelectionChanged -= OnSelectorSelectionChanged;
        }

        static void OnSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // Only react to the Selected event raised by the TreeViewItem
            // whose IsSelected property was modified. Ignore all ancestors
            // who are merely reporting that a descendant's Selected fired.
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
                return;

            Selector item = e.OriginalSource as Selector;
            if (item != null && item.SelectedItem != null)
            {
                if (item is ListBox)
                    ((ListBox)item).ScrollIntoView(item.SelectedItem);
                if (item is DataGrid)
                    ((DataGrid)item).ScrollIntoView(item.SelectedItem);
            }
        }

        #endregion // IsBroughtIntoViewWhenSelected
    }
}

