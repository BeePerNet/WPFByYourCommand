using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public interface IMenuCommand : ITextCommand
    {
        KeyGesture KeyGesture { get; }

        bool UseDisablingImage { get; }

        string IconSource { get; }
    }
}
