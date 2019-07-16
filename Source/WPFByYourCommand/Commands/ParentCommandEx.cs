using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public class ParentCommandEx : CommandExLoc, IParentMenuCommand
    {
        public IList<ICommand> Childs { get; set; } = new List<ICommand>();
    }
}
