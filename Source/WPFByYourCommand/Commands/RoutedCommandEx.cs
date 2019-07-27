using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
    public class RoutedCommandEx : RoutedCommand, INotifyPropertyChanged, IMenuCommand
    {
        public RoutedCommandEx() { }

        private KeyGesture _keyGesture;
        public KeyGesture KeyGesture { get => _keyGesture; set => SetProperty(ref _keyGesture, value); }

        private bool _useDisablingImage = true;
        public bool UseDisablingImage { get => _useDisablingImage; set => SetProperty(ref _useDisablingImage, value); }

        private object _Icon;
        public object Icon { get => _Icon; set => SetProperty(ref _Icon, value); }

        private string _text;
        public string Text { get => _text; set => SetProperty(ref _text, value); }

        private string _tag;
        public string Tag { get => _tag; set => SetProperty(ref _tag, value); }



        public RoutedCommandEx(string name, string text, object icon, Type ownerType, params InputGesture[] gestures) : base(name, ownerType, new InputGestureCollection(gestures))
        {
            this._text = text;
            this._Icon = icon;
            this.KeyGesture = gestures.OfType<KeyGesture>().FirstOrDefault();
        }

        public RoutedCommandEx(string name, string text, Type ownerType, params InputGesture[] gestures) : this(name, text, null, ownerType, gestures) { }




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


        public void FillCommandSource(ICommandSource commandSource)
        {
            CommandUtils.FillCommandSource(this, commandSource);
        }

        public void UnFillCommandSource(ICommandSource commandSource)
        {
            CommandUtils.UnFillCommandSource(this, commandSource);
        }





    }
}
