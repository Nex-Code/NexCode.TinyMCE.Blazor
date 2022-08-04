using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Plugins.Toolbar
{
    public class GroupToolbarButton : BasicToolbarButton
    {
        public IEnumerable<LabelledToolbarButton> Items { get; set; } = Array.Empty<LabelledToolbarButton>();

        internal override string FuncName => "addGroupToolbarButton";

    }

    public class LabelledToolbarButton
    {
        public string Name { get; set; }
        public IEnumerable<string> Items { get; set; } = Array.Empty<string>();
    }
}
