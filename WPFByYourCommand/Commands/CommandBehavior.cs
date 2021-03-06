﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
    public static class CommandBehavior
    {
        public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached("Context",
          typeof(ICommandContext), typeof(CommandBehavior),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCommandInvalidated)));

        public static ICommandContext GetContext(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetValue(ContextProperty) as ICommandContext;
        }

        public static void SetContext(DependencyObject element, ICommandContext commandContext)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(ContextProperty, commandContext);
        }

        /// <summary>  
        /// Callback when the Command property is set or changed.  
        /// </summary>  
        private static void OnCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.  
            UIElement element = (UIElement)dependencyObject;
            element.CommandBindings.Clear();

            // If we're given a command model, set up a binding  
            ICommandContext commandContext = e.NewValue as ICommandContext;
            if (commandContext != null)
            {
                foreach (CommandBinding commandBinding in commandContext.Commands)
                {
                    element.CommandBindings.Add(commandBinding);
                }
            }

            if (commandContext != null)
            {
                foreach (InputBinding inputBinding in commandContext.Inputs)
                {
                    element.InputBindings.Add(inputBinding);
                }
            }

            // Suggest to WPF to refresh commands  
            CommandManager.InvalidateRequerySuggested();
        }






        public static readonly DependencyProperty UITypeProperty = DependencyProperty.RegisterAttached(
            "UIType", typeof(CommandUIBehaviorType), typeof(CommandBehavior), new FrameworkPropertyMetadata(CommandUIBehaviorType.Default, UITypePropertyChanged));

        private static void UITypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
            //TODO
        }

        public static void SetUIType(Control target, CommandUIBehaviorType command)
        {
            target.SetValue(UITypeProperty, command);
        }
        public static CommandUIBehaviorType GetUIType(Control target)
        {
            return (CommandUIBehaviorType)target.GetValue(UITypeProperty);
        }






















        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(CommandBehavior), new FrameworkPropertyMetadata(null, CommandPropertyChanged));

        private static void CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ICommandSource commandSource = d as ICommandSource;

            if (e.OldValue != null && e.OldValue != DependencyProperty.UnsetValue && e.OldValue is ICommand)
            {
                if (e.OldValue is IMenuCommand)
                {
                    (e.OldValue as IMenuCommand).UnFillCommandSource(commandSource);
                }
                else
                {
                    string text = string.Empty;
                    if (e.OldValue is RoutedUICommand)
                    {
                        text = (e.OldValue as RoutedUICommand).Text;
                    }
                    else if (e.OldValue is ITextCommand)
                    {
                        text = (e.OldValue as ITextCommand).Text;
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        if (commandSource is MenuItem menuItem)
                        {
                            menuItem.Command = null;
                            menuItem.Header = null;
                        }
                        else if (commandSource is ButtonBase button)
                        {
                            button.Command = null;
                            button.Content = null;
                        }
                        else
                        {
                            //TODO
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        //TODO
                        throw new NotImplementedException();
                    }
                }
            }
            if (e.NewValue != null && e.NewValue != DependencyProperty.UnsetValue && e.NewValue is ICommand)
            {
                if (e.NewValue is IMenuCommand)
                {
                    (e.NewValue as IMenuCommand).FillCommandSource(commandSource);
                }
                else
                {
                    string text = string.Empty;
                    if (e.NewValue is RoutedUICommand)
                    {
                        text = (e.NewValue as RoutedUICommand).Text;
                    }
                    else if (e.NewValue is ITextCommand)
                    {
                        text = (e.NewValue as ITextCommand).Text;
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        if (commandSource is MenuItem menuItem)
                        {
                            menuItem.Command = e.NewValue as ICommand;
                            menuItem.Header = text;
                        }
                        else if (commandSource is ButtonBase button)
                        {
                            button.Command = e.NewValue as ICommand;
                            button.Content = text;
                        }
                        else
                        {
                            //TODO
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        //TODO
                        throw new NotImplementedException();
                    }
                }
            }
        }

        public static void SetCommand(Control target, ICommand command)
        {
            target.SetValue(CommandProperty, command);
        }
        public static ICommand GetCommand(Control target)
        {
            return (ICommand)target.GetValue(CommandProperty);
        }


    }
}
