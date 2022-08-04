using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Plugins.MenuItem
{
    public class ToggleMenuItem : BasicMenuItem
    {
        public bool Active { get; set; }

        internal override string FuncName => "addToggleMenuItem";

    }
}
