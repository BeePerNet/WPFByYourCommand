using System.Windows.Input;

namespace WPFByYourCommand
{
    public interface IMenuCommand : ICommand
    {
        KeyGesture KeyGesture { get; }

        bool UseDisablingImage { get; }

        string IconSource { get; }

        string Text { get; }

    }
}
