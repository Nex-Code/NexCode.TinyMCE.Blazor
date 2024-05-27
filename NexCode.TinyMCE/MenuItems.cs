using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor
{
 

    public class MenuItem : BaseMenuItem
    {
        public override string Type => "menuitem";
    }

    public class ToggleMenuItem : BaseMenuItem
    {
    
        public override string Type => "togglemenuitem";
    }
}
