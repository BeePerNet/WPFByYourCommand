using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using WPFByYourCommand.Expressions;

namespace WPFByYourCommand.Behaviors
{
    /// <summary>
    /// Exposes attached behaviors that can be
    /// applied to TreeViewItem objects.
    /// </summary>
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public static class SelectorBehavior
    {
        #region IsBroughtIntoViewWhenSelected

        public static bool GetIsBroughtIntoViewWhenSelected(Selector selector)
        {
            return (bool)selector.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        }

        public static void SetIsBroughtIntoViewWhenSelected(Selector selector, bool value)
        {
            selector.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached(
            "IsBroughtIntoViewWhenSelected",
            typeof(bool),
            typeof(SelectorBehavior),
            new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectionChanged));

        private static void OnIsBroughtIntoViewWhenSelectionChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is Selector item))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                item.SelectionChanged += OnSelectorSelectionChangedForFocus;
            }
            else
            {
                item.SelectionChanged -= OnSelectorSelectionChangedForFocus;
            }
        }

        private static void OnSelectorSelectionChangedForFocus(object sender, SelectionChangedEventArgs e)
        {

            // Only react to the Selected event raised by the TreeViewItem
            // whose IsSelected property was modified. Ignore all ancestors
            // who are merely reporting that a descendant's Selected fired.
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
            {
                return;
            }

            if (e.OriginalSource is Selector item && item.SelectedItem != null)
            {
                if (item is ListBox)
                {
                    ((ListBox)item).ScrollIntoView(item.SelectedItem);
                }

                if (item is DataGrid)
                {
                    ((DataGrid)item).ScrollIntoView(item.SelectedItem);
                }
            }
        }

        #endregion // IsBroughtIntoViewWhenSelected




        #region SyncIsSelectedWhenSelected

        public static string GetSyncIsSelectedWhenSelected(Selector selector)
        {
            return (string)selector.GetValue(SyncIsSelectedWhenSelectedProperty);
        }

        public static void SetSyncIsSelectedWhenSelected(Selector selector, string value)
        {
            selector.SetValue(SyncIsSelectedWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty SyncIsSelectedWhenSelectedProperty =
            DependencyProperty.RegisterAttached(
            "SyncIsSelectedWhenSelected",
            typeof(string),
            typeof(SelectorBehavior),
            new UIPropertyMetadata(null, OnSyncIsSelectedWhenSelectionChanged));

        private static void OnSyncIsSelectedWhenSelectionChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is Selector item))
            {
                return;
            }

            if (e.NewValue == null)
            {
                item.SelectionChanged -= OnSelectorSelectionChangedForIsSelected;
            }
            else
            {
                item.SelectionChanged += OnSelectorSelectionChangedForIsSelected;
            }
        }

        private static void OnSelectorSelectionChangedForIsSelected(object sender, SelectionChangedEventArgs e)
        {

            // Only react to the Selected event raised by the TreeViewItem
            // whose IsSelected property was modified. Ignore all ancestors
            // who are merely reporting that a descendant's Selected fired.
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
            {
                return;
            }

            if (e.OriginalSource is Selector list)
            {
                PassItems(list, e.RemovedItems, false);
                PassItems(list, e.AddedItems, true);
            }
        }

        private static Dictionary<object, FastProperty> compiledAccessors = new Dictionary<object, FastProperty>();
        private static void PassItems(Selector selector, IList list, bool selected)
        {
            FastProperty accessor = null;
            if (list != null)
            {
                foreach (object obj in list)
                {
                    if (accessor == null)
                    {
                        if (!compiledAccessors.ContainsKey(obj.GetType()))
                        {
                            string path = GetSyncIsSelectedWhenSelected(selector);
                            accessor = new FastProperty(obj, path);
                            compiledAccessors[obj.GetType()] = accessor;
                        }
                        else
                        {
                            accessor = compiledAccessors[obj.GetType()];
                        }
                    }
                    accessor.Set(obj, selected);
                }
            }
        }

        #endregion // SyncIsSelectedWhenSelected

    }
}

