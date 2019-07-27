using System;
using System.Diagnostics.CodeAnalysis;

namespace WPFByYourCommand.Commands
{
    [SuppressMessage("Design", "CA1008:Enums should have zero value")]
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
