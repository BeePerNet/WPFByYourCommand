using System.Windows;
using System.Windows.Input;
using WPFByYourCommand.Observables;

namespace WPFByYourCommand.Commands
{
    public abstract class CommandViewModel : ObservableObject, ICommandContext
    {
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


        public static T GetViewModelObject<T>(object originalSource) where T : CommandViewModel
        {
            FrameworkElement element = originalSource as FrameworkElement;
            if (element == null)
                return null;
            //if (element is Selector)
            //  return (element as Selector)?.SelectedItem as T;

            return element.DataContext as T;
        }

    }
}
