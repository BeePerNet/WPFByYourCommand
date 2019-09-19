using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFByYourCommand.Controls;

namespace WPFByYourCommand.Behaviors
{
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<En attente>")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public static class ControlBehavior
    {
        public static bool GetFocusMouseRightButtonDown(Control element)
        {
            return (bool)element.GetValue(FocusMouseRightButtonDownProperty);
        }

        public static void SetFocusMouseRightButtonDown(Control element, bool value)
        {
            element.SetValue(FocusMouseRightButtonDownProperty, value);
        }


        public static readonly DependencyProperty FocusMouseRightButtonDownProperty =
            DependencyProperty.RegisterAttached(
            "FocusMouseRightButtonDown",
            typeof(bool),
            typeof(ControlBehavior),
            new FrameworkPropertyMetadata(false, OnFocusMouseRightButtonDownChanged));

        private static void OnFocusMouseRightButtonDownChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is UIElement element))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                element.PreviewMouseRightButtonDown += FocusElementMouseRightButtonDown;
            }
            else
            {
                element.PreviewMouseRightButtonDown -= FocusElementMouseRightButtonDown;
            }
        }

        private static void FocusElementMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Control control = ControlsHelper.FindParentControl<Control>(e.OriginalSource as DependencyObject);

            if (control != null)
            {
                control.Focus();
                e.Handled = true;
            }
        }



















        public static bool GetKeyboardFocusOnMouseButtonDown(Control element)
        {
            return (bool)element.GetValue(KeyboardFocusOnMouseButtonDownProperty);
        }

        public static void SetKeyboardFocusOnMouseButtonDown(Control element, bool value)
        {
            element.SetValue(KeyboardFocusOnMouseButtonDownProperty, value);
        }


        public static readonly DependencyProperty KeyboardFocusOnMouseButtonDownProperty =
            DependencyProperty.RegisterAttached(
            "KeyboardFocusOnMouseButtonDown",
            typeof(bool),
            typeof(ControlBehavior),
            new FrameworkPropertyMetadata(false, OnKeyboardFocusOnMouseButtonDownChanged));

        private static void OnKeyboardFocusOnMouseButtonDownChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is UIElement element))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                element.MouseDown += KeyboardFocusOnMouseButtonDown;
            }
            else
            {
                element.MouseDown -= KeyboardFocusOnMouseButtonDown;
            }
        }

        private static void KeyboardFocusOnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            Control control = e.Source as Control;

            if (control != null && !control.IsFocused)
            {
                control.Focus();
                e.Handled = true;
            }
        }


    }
}
