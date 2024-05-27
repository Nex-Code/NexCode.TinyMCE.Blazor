using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor.Code
{
    public class TinyEditor : JsInteropBase
    {
        public TinyEditor(IJSRuntime jsRuntime) : base(jsRuntime)
        {
        }

        protected override string ScriptPath => "./_content/NexCode.TinyMCE.Blazor/TinyEditor.cs.js";

        protected override async ValueTask<IJSObjectReference> Load()
        {
            var loader = new ScriptLoader(JsRuntime,
                "./_content/NexCode.TinyMCE.Blazor/lib/tinymce/tinymce.min.js");
            await loader.LoadAndWait();

            return await base.Load();
        }


        public async ValueTask Init(string selector, IEnumerable<Plugin> plugins)
        {
            var plugs = new List<string>();
            var toolbar = new List<string>();
            var menu = new List<string>();
            var contextToolbar = new List<string>();
            var contextMenu = new List<string>();
            var contextForms = new List<string>();

            var options = new Dictionary<string, object>();
            var pluginRegisters = new List<ValueTask>();

            options.Add("selector","#"+selector);

            foreach (var plugin in plugins)
            {
                plugs.Add(plugin.Name);

                toolbar.AddIf(plugin.Toolbar, s => s.IsNotNullOrWhiteSpace());
                contextMenu.AddIf(plugin.ContextMenu, s => s.IsNotNullOrWhiteSpace());
                contextToolbar.AddIf(plugin.ContextToolbar, s => s.IsNotNullOrWhiteSpace());
                contextForms.AddIf(plugin.ContextForms, s => s.IsNotNullOrWhiteSpace());
               

                if (plugin.OnEditorLoad != null)
                    plugin.OnEditorLoad(options);


                menu.AddIf(plugin.Menubar, s => s.IsNotNullOrWhiteSpace());

                if (plugin is CustomPlugin custom)
                {
                    await  RegisterPlugin(custom);
                    //pluginRegisters.Add(vt);
                }
            }

            await pluginRegisters.WhenAll();

            if(plugs.Any())
                options.Add("plugins", string.Join(" ",plugs));
            if (toolbar.Any())
                options.Add("toolbar", string.Join(" ", toolbar));
            if (menu.Any())
                options.Add("menubar", string.Join(" ", menu));
            if (contextMenu.Any())
                options.Add("contextmenu", string.Join(" ", contextMenu));

            if (options.Remove("menu", out object menuItems))
            {
                var items = (IEnumerable<Menu>)menuItems;
                options.Add("menu", items.ToDictionary(i=>i.Title));
            }

            await InvokeVoidAsync("init", options);
        }

        public async ValueTask RegisterPlugin(CustomPlugin plugin)
        {
            var registerableItems = plugin.GetRegisterableItems();
            if(registerableItems.Any())
                await InvokeVoidAsync("registerPlugin", plugin.Name, registerableItems);
        }


    }


}
