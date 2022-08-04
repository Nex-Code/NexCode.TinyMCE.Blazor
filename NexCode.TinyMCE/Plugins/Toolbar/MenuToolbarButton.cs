using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor.Plugins.Toolbar
{
    public abstract class MenuToolbarButtonBase : BasicToolbarButton
    {
        public abstract string Type { get; }

        internal override string FuncName => "addMenuButton";

    }

    public class MenuToolbarButtonGroup : MenuToolbarButtonBase
    {
        public override string Type => "nestedmenuitem";

        public IEnumerable<MenuToolbarButtonBase> Items { get; set; } = Array.Empty<MenuToolbarButtonBase>();

        [JSInvokable]
        public virtual async ValueTask<IEnumerable<object>> Fetch()
        {
            return Items.Process(true);
        }

    }

    public class MenuToolbarButtonItem : MenuToolbarButtonBase
    {

        public override string Type => "menuitem";

    }
}
