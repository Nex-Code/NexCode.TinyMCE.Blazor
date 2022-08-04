using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Plugins.Toolbar
{
    public class ToggleToolbarButton : BasicToolbarButton
    {

        public bool Active { get; set; }

        internal override string FuncName => "addToggleButton";
    }
}
