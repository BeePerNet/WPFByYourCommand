using System;
using System.Diagnostics;
using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    [DebuggerDisplay("---")]
    public class SeparatorDummyCommand : IMenuCommand
    {
        private SeparatorDummyCommand()
        {
        }

        public static SeparatorDummyCommand Instance { get; } = new SeparatorDummyCommand();

        public string Text => "-";

        public KeyGesture KeyGesture => null;

        public bool UseDisablingImage => false;

        public object Icon => null;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }

        public void FillCommandSource(ICommandSource commandSource)
        {
        }

        public void UnFillCommandSource(ICommandSource commandSource)
        {
        }
    }
}
