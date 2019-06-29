using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
