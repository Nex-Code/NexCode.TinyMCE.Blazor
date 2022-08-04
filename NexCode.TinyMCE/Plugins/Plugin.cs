using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexCode.TinyMCE.Blazor.Plugins.MenuItem;
using NexCode.TinyMCE.Blazor.Plugins.Toolbar;

namespace NexCode.TinyMCE.Blazor.Plugins
{
    public class Plugin
    {

        public string Name { get; set; }

        public IEnumerable<BasicToolbarButton> Toolbar { get; set; } = new List<BasicToolbarButton>();

        public IEnumerable<BasicMenuItem> MenuItems { get; set; } = new List<BasicMenuItem>();

    }
}
