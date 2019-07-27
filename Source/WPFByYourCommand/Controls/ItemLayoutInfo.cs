using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFByYourCommand.Controls
{
    public class ItemLayoutInfo
    {
        public int FirstRealizedItemIndex { get; set; }
        public double FirstRealizedLineTop { get; set; }
        public double FirstRealizedItemLeft { get; set; }
        public int LastRealizedItemIndex { get; set; }
    }
}
