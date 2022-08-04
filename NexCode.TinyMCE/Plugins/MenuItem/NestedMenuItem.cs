using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor.Plugins.MenuItem
{
    public class NestedMenuItem : BasicMenuItem
    {
        public IEnumerable<BasicMenuItem> Items { get; set; } = Array.Empty<BasicMenuItem>();

        internal override string FuncName => "addNestedMenuItem";

        [JSInvokable]
        public virtual async ValueTask<IEnumerable<BasicMenuItem>> Fetch()
        {
            return Items;
        }


    }
}
