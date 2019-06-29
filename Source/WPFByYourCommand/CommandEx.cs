using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace WPFByYourCommand
{
    public class CommandEx : RoutedCommand
    {
        private KeyGesture _keyGesture;
        public KeyGesture KeyGesture { get => _keyGesture; set => SetProperty(ref _keyGesture, value); }

        private bool _useDisablingImage = true;
        public bool UseDisablingImage { get => _useDisablingImage; set => SetProperty(ref _useDisablingImage, value); }

        private string _iconSource;
        public string IconSource { get => _iconSource; set => SetProperty(ref _iconSource, value); }

        private string _text;
        public string Text { get => _text; set => SetProperty(ref _text, value); }



        public CommandEx(string name, string text, string iconSource, Type ownerType, InputGesture gesture) : base(name, ownerType, new InputGestureCollection(new[] { gesture }))
        {
            this._text = text;
            this._iconSource = iconSource;
        }







        //The interface only includes this evennt
        public event PropertyChangedEventHandler PropertyChanged;

        //Common implementations of SetProperty
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName]string name = null)
        {
            bool propertyChanged = false;

            //If we have a different value, do stuff
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(name);
                propertyChanged = true;
            }

            return propertyChanged;
        }

        //The C#6 version of the common implementation
        protected void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }




























        public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached("Context",
          typeof(ICommandContext), typeof(CommandEx),
          new PropertyMetadata(new PropertyChangedCallback(OnCommandInvalidated)));

        public static ICommandContext GetContext(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element", "is null");
            return element.GetValue(ContextProperty) as ICommandContext;
        }

        public static void SetContext(DependencyObject element, ICommandContext commandContext)
        {
            if (element == null)
                throw new ArgumentNullException("element", "is null");
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














        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(CommandEx), new PropertyMetadata(_CommandPropertyChanged));

        private static void _CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            /*MenuItem target = d as MenuItem;
            if (target != null)
            {
                IMenuCommand command = (IMenuCommand)e.NewValue;
                if (command != null)
                {
                    target.Command = command;
                    target.Header = command.Header;
                }
                else
                {
                    target.Command = null;
                    target.Header = null;
                }
            }*/
        }
        public static void SetCommand(Control target, CommandEx command)
        {
            target.SetValue(CommandProperty, command);
        }
        public static CommandEx GetCommand(Control target)
        {
            return (CommandEx)target.GetValue(CommandProperty);
        }










    }
}
