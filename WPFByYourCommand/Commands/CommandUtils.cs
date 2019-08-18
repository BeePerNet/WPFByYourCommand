using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFByYourCommand.Controls;
using WPFLocalizeExtension.Extensions;

namespace WPFByYourCommand.Commands
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    public static class CommandUtils
    {
        private const string locPrefix = "loc:";

        public static void FillCommandSource(IMenuCommand command, ICommandSource commandSource)
        {
            if (commandSource is MenuItem)
            {
                FillMenuItem(command, commandSource as MenuItem);
            }
            else if (commandSource is ButtonBase)
            {
                FillButton(command, commandSource as ButtonBase);
            }
        }

        public static void UnFillCommandSource(IMenuCommand command, ICommandSource commandSource)
        {
            if (commandSource is MenuItem)
            {
                UnFillMenuItem(command, commandSource as MenuItem);
            }
            else if (commandSource is ButtonBase)
            {
                UnFillButton(command, commandSource as ButtonBase);
            }
        }




        public static void FillMenuItem(IMenuCommand command, MenuItem menuItem)
        {
            if (command == null || menuItem == null)
            {
                return;
            }

            menuItem.Command = command;

            if (!string.IsNullOrWhiteSpace(command.Text))
            {
                if (command.Text.StartsWith(locPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    BindingOperations.SetBinding(menuItem, MenuItem.HeaderProperty, new BLoc(command.Text.Remove(0, 4)));
                }
                else
                {
                    menuItem.Header = command.Text;
                }
            }

            if (command.KeyGesture != null && !string.IsNullOrWhiteSpace(command.KeyGesture.DisplayString))
            {
                if (command.KeyGesture.DisplayString.StartsWith(locPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    BindingOperations.SetBinding(menuItem, MenuItem.InputGestureTextProperty, new BLoc(command.KeyGesture.DisplayString.Remove(0, 4)));
                }
                else
                {
                    menuItem.InputGestureText = command.KeyGesture.DisplayString;
                }
            }

            if (command.Icon != null)
            {
                if (command.Icon is string)
                {
                    menuItem.Icon = GetImage(command);
                }
                else
                {
                    menuItem.Icon = command.Icon;
                }
            }
        }

        public static void UnFillMenuItem(IMenuCommand command, MenuItem menuItem)
        {
            if (command == null || menuItem == null)
            {
                return;
            }

            menuItem.Command = null;

            if (!string.IsNullOrWhiteSpace(command.Text))
            {
                if (command.Text.StartsWith(locPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    BindingOperations.ClearBinding(menuItem, MenuItem.HeaderProperty);
                }
                else
                {
                    menuItem.Header = null;
                }
            }

            if (command.KeyGesture != null && !string.IsNullOrWhiteSpace(command.KeyGesture.DisplayString))
            {
                if (command.KeyGesture.DisplayString.StartsWith(locPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    BindingOperations.ClearBinding(menuItem, MenuItem.InputGestureTextProperty);
                }
                else
                {
                    menuItem.InputGestureText = null;
                }
            }

            if (command.Icon != null)
            {
                menuItem.Icon = null;
            }
        }

        public static void FillButton(IMenuCommand command, ButtonBase button)
        {
            if (command == null || button == null)
            {
                return;
            }

            button.Command = command;

            if (command.Icon == null)
            {
                if (!string.IsNullOrWhiteSpace(command.Text))
                {
                    if (command.Text.StartsWith(locPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        BindingOperations.SetBinding(button, Button.ContentProperty, new BLoc(command.Text.Remove(0, 4)));
                    }
                    else
                    {
                        button.Content = command.Text;
                    }
                }
            }
            else
            {
                if (command.Icon is string)
                {
                    button.Content = GetImage(command);
                }
                else
                {
                    button.Content = command.Icon;
                }

                if (!string.IsNullOrWhiteSpace(command.Text))
                {
                    if (command.Text.StartsWith(locPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        BindingOperations.SetBinding(button, Button.ToolTipProperty, new BLoc(command.Text.Remove(0, 4)));
                    }
                    else
                    {
                        button.ToolTip = command.Text;
                    }
                }
            }
        }

        public static void UnFillButton(IMenuCommand command, ButtonBase button)
        {
            if (command == null || button == null)
            {
                return;
            }

            button.Command = null;

            if (command.Icon == null)
            {
                if (!string.IsNullOrWhiteSpace(command.Text))
                {
                    if (command.Text.StartsWith(locPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        BindingOperations.ClearBinding(button, Button.ContentProperty);
                    }
                    else
                    {
                        button.Content = null;
                    }
                }
            }
            else
            {
                button.Content = null;

                if (!string.IsNullOrWhiteSpace(command.Text))
                {
                    if (command.Text.StartsWith(locPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        BindingOperations.ClearBinding(button, Button.ToolTipProperty);
                    }
                    else
                    {
                        button.ToolTip = null;
                    }
                }
            }
        }



        public static Image GetImage(IMenuCommand command)
        {
            if (command == null)
            {
                return null;
            }

            BitmapImage bitmap = new BitmapImage(new Uri(command.Icon.ToString()));
            if (command.UseDisablingImage)
            {
                return new AutoDisablingImage
                {
                    Source = bitmap,
                    Width = bitmap.PixelWidth,
                    Height = bitmap.PixelWidth
                };
            }
            else
            {
                return new Image
                {
                    Source = bitmap,
                    Width = bitmap.PixelWidth,
                    Height = bitmap.PixelWidth
                };
            }

        }







    }
}
