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
                element.MouseRightButtonDown += FocusElementMouseRightButtonDown;
            }
            else
            {
                element.MouseRightButtonDown -= FocusElementMouseRightButtonDown;
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

    }
}
