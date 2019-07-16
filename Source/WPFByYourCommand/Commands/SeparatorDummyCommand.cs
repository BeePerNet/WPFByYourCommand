using System;
using System.Diagnostics;

namespace WPFByYourCommand.Commands
{
    [DebuggerDisplay("---")]
    public class SeparatorDummyCommand : ITextCommand
    {
        private SeparatorDummyCommand()
        {
        }

        public static SeparatorDummyCommand Instance { get; } = new SeparatorDummyCommand();

        public string Text => "-";

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }
    }
}
