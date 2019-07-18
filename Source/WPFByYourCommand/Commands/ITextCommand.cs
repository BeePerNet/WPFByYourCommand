using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public interface ITextCommand : ICommand
    {
        string Text { get; }
    }
}
