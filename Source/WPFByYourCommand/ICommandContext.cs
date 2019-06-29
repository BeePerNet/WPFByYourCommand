using System.Windows.Input;

namespace WPFByYourCommand
{
    public interface ICommandContext
    {
        CommandBindingCollection Commands { get; }
        InputBindingCollection Inputs { get; }
    }
}
