using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public class ParentCommand : DirectCommand
    {
        public ParentCommand(string name, string text, string iconSource, Type ownerType, params InputGesture[] gestures) :
            base(name, text, iconSource, ownerType, null, null, false, gestures)
        {
        }


        public ParentCommand(string name, string text, Type ownerType, params InputGesture[] gestures) : this(name, text, null, ownerType, gestures) { }


        public IList<ICommand> Childs { get; } = new List<ICommand>();


        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {

        }


    }
}
