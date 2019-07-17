using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public interface IMenuCommand : ITextCommand
    {
        KeyGesture KeyGesture { get; }

        bool UseDisablingImage { get; }

        object Icon { get; }

        void FillCommandSource(ICommandSource commandSource);

        void UnFillCommandSource(ICommandSource commandSource);
    }
}
