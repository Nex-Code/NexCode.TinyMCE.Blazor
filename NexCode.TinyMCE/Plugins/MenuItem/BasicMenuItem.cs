using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Plugins.MenuItem
{
    public class BasicMenuItem : BaseAction
    {

        public string? Value { get; set; }
        public string? Shortcut { get; set; }


        internal override string FuncName => "addMenuItem";
    }
}
