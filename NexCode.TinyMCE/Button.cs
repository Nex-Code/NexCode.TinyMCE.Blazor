using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NexCode.TinyMCE.Blazor.Button;
using static NexCode.TinyMCE.Blazor.ToggleButton;

namespace NexCode.TinyMCE.Blazor
{
    public class Button : BaseMenuItem
    {
        public override string Type => "button";
    }

    public class ToggleButton : BaseMenuItem
    {
        public override string Type => "togglebutton";
    }

    public class MenuButton : BaseMenuItem
    {

        public Func<IEnumerable<ITinyItem>> Fetch { get; init; }

        public override string Type => "menubutton";
    }

    public class Seperator : BaseMenuItem
    {
        public override string Type => "button";
        public override string Name { 
            get => "|" ; 
            set => _ = value; }
    }
}
