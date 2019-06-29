using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WPFByYourCommand
{
    public abstract class CommandViewModel : INotifyPropertyChanged, ICommandContext
    {
        protected static void AddCommandInput(InputBindingCollection bindingCollection, RoutedCommand command) 
        {
            if (command.InputGestures != null)
                foreach (InputGesture gesture in command.InputGestures)
                    bindingCollection.Add(new InputBinding(command, gesture));
        }




        private static CommandBindingCollection _commandList;
        private static InputBindingCollection _inputList;
        private string _statusText = string.Empty;

        public CommandBindingCollection Commands
        {
            get
            {
                if (_commandList == null)
                {
                    _commandList = new CommandBindingCollection();
                    this.AddCommandModels(_commandList);
                }

                return _commandList;
            }
        }

        public abstract void AddCommandModels(CommandBindingCollection bindingCollection);


        public InputBindingCollection Inputs
        {
            get
            {
                if (_inputList == null)
                {
                    _inputList = new InputBindingCollection();
                    this.AddInputModels(_inputList);
                }

                return _inputList;
            }
        }

        public abstract void AddInputModels(InputBindingCollection bindingCollection);

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

        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }
    }
}
