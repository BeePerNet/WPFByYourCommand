﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public interface IParentMenuCommand
    {
        IList<ICommand> Childs { get; set; }
    }
}
