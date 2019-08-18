using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public interface IMenuCommand : ITextCommand
    {
        KeyGesture KeyGesture { get; }

        bool UseDisablingImage { get; }

        string Name { get; }

        object Icon { get; }

        string Tag { get; }

        void FillCommandSource(ICommandSource commandSource);

        void UnFillCommandSource(ICommandSource commandSource);
    }
}
