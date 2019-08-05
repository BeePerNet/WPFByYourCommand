using System.Windows;
using System.Windows.Input;
using WPFByYourCommand.Controls;

namespace WPFByYourCommand.Behaviors
{
    public static class UIElementBehavior
    {
        public static bool GetFocusPreviewMouseRightButtonDown(UIElement element)
        {
            return (bool)element.GetValue(FocusPreviewMouseRightButtonDownProperty);
        }

        public static void SetFocusPreviewMouseRightButtonDown(UIElement element, bool value)
        {
            element.SetValue(FocusPreviewMouseRightButtonDownProperty, value);
        }


        public static readonly DependencyProperty FocusPreviewMouseRightButtonDownProperty =
            DependencyProperty.RegisterAttached(
            "FocusPreviewMouseRightButtonDown",
            typeof(bool),
            typeof(UIElementBehavior),
            new FrameworkPropertyMetadata(false, OnFocusPreviewMouseRightButtonDownChanged));

        static void OnFocusPreviewMouseRightButtonDownChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is UIElement element))
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
            UIElement control = ControlsHelper.FindParentControl<UIElement>(e.OriginalSource as DependencyObject);

            if (control != null)
            {
                control.Focus();
                e.Handled = true;
            }
        }

    }
}
