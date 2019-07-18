using System;

namespace WPFByYourCommand.Commands
{
    [Flags]
    public enum CommandUIBehaviorType
    {
        Default = 0,
        Icon = 1,
        Text = 2,
        IconAndText = 3,
        Tooltip = 4,
        IconAndTooltip = 5
    }
}
