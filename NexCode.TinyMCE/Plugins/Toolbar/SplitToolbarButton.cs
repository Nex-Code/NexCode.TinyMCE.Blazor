using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor.Plugins.Toolbar
{
    public class SplitToolbarButton : BasicToolbarButton
    {


        public IEnumerable<SplitMenuItems> Items { get; set; } = Array.Empty<SplitMenuItems>();

        public int Columns { get; set; } = 1;

        internal override string FuncName => "addSplitButton";

        public Action<string>? ItemAction { get; set; }

        [JSInvokable]
        public virtual async ValueTask<IEnumerable<SplitMenuItems>> Fetch()
        {
            return Items;
        }



        [JSInvokable]
        public virtual void OnItemAction(string value) => ItemAction?.Invoke(value);

    }

    public class SplitMenuItems
    {
        public string Value { get; set; }
        public string? Text { get; set; }
        public string? Icon { get; set; }
        public bool Enabled { get; set; } = true;
        public string? Shortcut { get; set; }

        public string Type => "choiceitem";
    }
}
