using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public interface ICommandContext
    {
        CommandBindingCollection Commands { get; }
        InputBindingCollection Inputs { get; }
    }
}
