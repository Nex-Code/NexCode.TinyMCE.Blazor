using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using NexCode.TinyMCE.Blazor.Code;

namespace NexCode.TinyMCE.Blazor
{
    public partial class RichTextEditor : ComponentBase, IEditorScope
    {


        public string Id { get; } = "editor_"+ Guid.NewGuid().ToString().Replace("-", "");

        public string EditorId => Id;
        public bool Intalised { get; private set; }

        [Parameter] public string Html { get; set; } = string.Empty;
        [Parameter] public EventCallback<string> HtmlChanged { get; set; }

        [Parameter] public bool DisableLoadOnRender { get; set; }
        [Parameter] public bool ContextMenuNeverUseNative { get; set; }

        [Parameter]
        public RenderFragment? Plugins { get; set; }


        [Inject] private TinyEditorIntaliser TinyEditor { get; set; } = default!;

        private bool _loaded;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !DisableLoadOnRender)
            {
                await Init();
            }
        }

        public async Task Init()
        {
            if (_loaded)
                return;
            _loaded = true;

            await TinyEditor.Init(Id, PluginList, () => Intalised = true);
        }

        private List<Plugin> PluginList { get; } = new List<Plugin>();

        internal void Register(Plugin plugin)
        {
            if (PluginList.Any(i => i.Name.Equals(plugin.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new ArgumentException($"Plugin with name ({plugin.Name}) already exists", nameof(plugin));

            PluginList.Add(plugin);
        }

        
    }


    public interface IEditorScope
    {
        public string EditorId { get; }
        public bool Intalised { get; }
    }

}
