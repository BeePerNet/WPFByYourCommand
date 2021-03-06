﻿using System.Windows;
using System.Windows.Input;
using WPFByYourCommand.Observables;

namespace WPFByYourCommand.Commands
{
    public abstract class CommandViewModel : ObservableObject, ICommandContext
    {
        private CommandBindingCollection _commandList;
        private InputBindingCollection _inputList;

        public CommandBindingCollection Commands
        {
            get
            {
                if (_commandList == null)
                {
                    _commandList = new CommandBindingCollection();
                    AddCommandModels(_commandList);
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
                    AddInputModels(_inputList);
                }

                return _inputList;
            }
        }



        public abstract void AddInputModels(InputBindingCollection bindingCollection);


        public static T GetViewModelObject<T>(object originalSource) where T : class
        {
            return GetViewModelObject(originalSource) as T;
        }

        public static object GetViewModelObject(object originalSource)
        {
            if (!(originalSource is FrameworkElement element))
            {
                return null;
            }

            return element.DataContext;
        }




    }
}
