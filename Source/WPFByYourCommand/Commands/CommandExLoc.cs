using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension.Extensions;

namespace WPFByYourCommand.Commands
{
    public class CommandExLoc : CommandEx
    {
        protected CommandExLoc() { }


        public static readonly new DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(CommandExLoc), new PropertyMetadata(_CommandPropertyChanged));

        private static void _CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IMenuCommand command = e.NewValue as IMenuCommand;
            ICommandSource commandSource = d as ICommandSource;
            if (commandSource != null && command != null)
            {
                KeyValuePair<Type, Action<IMenuCommand, ICommandSource>>? dest = destinationDictionary.FirstOrDefault(T => T.Key.IsAssignableFrom(d.GetType()));
                if (dest.HasValue)
                {
                    dest.Value.Value(command, commandSource);
                }
            }
        }

        private static Dictionary<Type, Action<IMenuCommand, ICommandSource>> destinationDictionary = new Dictionary<Type, Action<IMenuCommand, ICommandSource>>();

        static CommandExLoc()
        {
            destinationDictionary.Add(typeof(MenuItem), FillMenuItem);
            destinationDictionary.Add(typeof(ButtonBase), FillButton);
        }

        private static void FillMenuItem(IMenuCommand command, ICommandSource control)
        {
            MenuItem menuItem = control as MenuItem;
            if (command != null)
            {
                menuItem.Command = command;

                if (!string.IsNullOrWhiteSpace(command.Text))
                    BindingOperations.SetBinding(menuItem, MenuItem.HeaderProperty, new BLoc(command.Text));

                if (command.KeyGesture != null && !string.IsNullOrWhiteSpace(command.KeyGesture.DisplayString))
                    BindingOperations.SetBinding(menuItem, MenuItem.InputGestureTextProperty, new BLoc(command.KeyGesture.DisplayString));

                if (!string.IsNullOrWhiteSpace(command.IconSource))
                {
                    Image image = GetImage(command);
                    menuItem.Icon = image;
 //                   BindingOperations.SetBinding(image, AutoDisablingImage.IsEnabledProperty, new Binding("IsEnabled") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MenuItem), 1) });
                }
            }
            else
            {
                menuItem.Command = null;

                if (!string.IsNullOrWhiteSpace(command.Text))
                    BindingOperations.ClearBinding(menuItem, MenuItem.HeaderProperty);

                if (command.KeyGesture != null && !string.IsNullOrWhiteSpace(command.KeyGesture.DisplayString))
                    BindingOperations.ClearBinding(menuItem, MenuItem.InputGestureTextProperty);

                if (!string.IsNullOrWhiteSpace(command.IconSource))
                    menuItem.Icon = null;
            }
        }

        private static void FillButton(IMenuCommand command, ICommandSource control)
        {
            ButtonBase button = control as ButtonBase;
            if (command != null)
            {
                button.Command = command;

                if (string.IsNullOrWhiteSpace(command.IconSource))
                {
                    if (!string.IsNullOrWhiteSpace(command.Text))
                        BindingOperations.SetBinding(button, Button.ContentProperty, new BLoc(command.Text));
                }
                else
                {
                    button.Content = GetImage(command);

                    if (!string.IsNullOrWhiteSpace(command.Text))
                        BindingOperations.SetBinding(button, Button.ToolTipProperty, new BLoc(command.Text));
                }
            }
            else
            {
                button.Command = null;
                if (string.IsNullOrWhiteSpace(command.IconSource))
                {
                    button.Content = null;
                    if (!string.IsNullOrWhiteSpace(command.Text))
                        BindingOperations.ClearBinding(button, Button.ContentProperty);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(command.Text))
                        BindingOperations.ClearBinding(button, Button.ToolTipProperty);
                }
            }
        }


        /// <summary>
        /// Load a resource WPF-BitmapImage (png, bmp, ...) from embedded resource defined as 'Resource' not as 'Embedded resource'.
        /// </summary>
        /// <param name="pathInApplication">Path without starting slash</param>
        /// <param name="assembly">Usually 'Assembly.GetExecutingAssembly()'. If not mentionned, I will use the calling assembly</param>
        /// <returns></returns>
        public static BitmapImage LoadBitmapFromResource(string pathInApplication, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }

            if (pathInApplication[0] == '/')
            {
                pathInApplication = pathInApplication.Substring(1);
            }
            return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/" + pathInApplication, UriKind.Absolute));
        }




        new public static void SetCommand(Control target, CommandEx command)
        {
            target.SetValue(CommandProperty, command);
        }
        new public static CommandEx GetCommand(Control target)
        {
            return (CommandEx)target.GetValue(CommandProperty);
        }





    }
}
