using System.Runtime.InteropServices;
using NexCode.TinyMCE.Blazor;
using Microsoft.JSInterop;
using NexCode.TinyMCE.Blazor.Plugins;
using NexCode.TinyMCE.Blazor.Plugins.MenuItem;
using NexCode.TinyMCE.Blazor.Plugins.Toolbar;

namespace NexCode.TinyMCEEditor
{
    internal class EditorJs : IAsyncDisposable
    {

        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private JsLoader JsLoader { get; }


        public EditorJs(IJSRuntime jsRuntime, JsLoader jsLoader)
        {
            JsLoader = jsLoader;
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NexCode.TinyMCE.Blazor/editorJs.js").AsTask());
        }



        public async ValueTask Load(bool waitForLoad = false)
        {

            if (JsLoader.Loaded)
                return;

            var url = "./_content/NexCode.TinyMCE.Blazor/lib/tinymce/tinymce.min.js";

            if (waitForLoad)
                await JsLoader.LoadAndWait(url);
            else
                JsLoader.Load(url);
        }

        public async ValueTask Destroy(string id)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("destroy", id);
        }

        public async ValueTask Init(string id, string? plugins = null, string? menubar = null, string? toolbar = null, IEnumerable<Plugin>? blazorPlugins = null)
        {
            await Load(true);


            var externalPlugins = new Dictionary<string,string>();
           

            if (blazorPlugins?.Any() ?? false)
            {
                foreach (var plugin in blazorPlugins)
                {
                    await RegisterPlugin(plugin);
                }
            }


            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("init", id, plugins, menubar, toolbar, externalPlugins);
        }


        private async ValueTask RegisterPlugin(Plugin plugin)
        {

            var toolbar = plugin.Toolbar.Select(i => ProcessButton(i, plugin.Name));
            var menuItem = plugin.MenuItems.Select(i => ProcessMenuItem(i, plugin.Name));
            var uiElements = toolbar.Concat(menuItem).ToArray();

            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("registerPlugin", plugin.Name.ToLower(), uiElements);
        }


        private Dictionary<string, object?> ProcessButton(BasicToolbarButton action, string pluginName)
        {
            var dict = BaseProcess(action, pluginName);

            var requiresDotNetHelper = dict.Any(i=>(true == i.Value as bool?) && i.Key.StartsWith("has") );


            if (action is SplitToolbarButton splitbutton)
            {

                dict.Add(nameof(splitbutton.Columns), splitbutton.Columns);

                if (splitbutton.ItemAction != null)
                {
                    dict.Add("HasItemAction",true);
                    requiresDotNetHelper = true;
                }
            }

            if (action is SplitToolbarButton || action is MenuToolbarButtonGroup || action is ToggleToolbarButton)
            {
                requiresDotNetHelper = true;
            }



            if(requiresDotNetHelper)
                dict.Add("DotNetHelper", DotNetObjectReference.Create(action));


            if (action is GroupToolbarButton gTb)
            {
                dict.Add(nameof(gTb.Items), gTb.Items);
            }


            return dict;
        }

        private Dictionary<string, object?> ProcessMenuItem(BasicMenuItem action, string pluginName)
        {
            var dict = BaseProcess(action, pluginName);

            dict.Add(nameof(action.Value),action.Value);
            dict.Add(nameof(action.Shortcut), action.Shortcut);


            var requiresDotNetHelper = dict.Any(i => (true == i.Value as bool?) && i.Key.StartsWith("has"));
            if (action is NestedMenuItem)
            {
                requiresDotNetHelper = true;
            }

            if (action is ToggleMenuItem ttbutton)
            {
                dict.Add(nameof(ttbutton.Active), ttbutton.Active);
            }

            if (requiresDotNetHelper)
                dict.Add("DotNetHelper", DotNetObjectReference.Create(action));

            return dict;
        }

        private Dictionary<string, object?> BaseProcess(BaseAction action, string pluginName)
        {
            var dict = new Dictionary<string, object?>();
            dict.Add(nameof(action.Text), action.Text);
            dict.Add(nameof(action.Icon), action.Icon);
            dict.Add(nameof(action.ToolTip), action.ToolTip);
            dict.Add(nameof(action.Enabled), action.Enabled);
            dict.Add(nameof(action.Id), action.Id ?? pluginName);
            dict.Add(nameof(action.FuncName), action.FuncName);

            if (action.Setup != null)
            {
                dict.Add("HasSetup", true);
            }

            if (action.Action != null)
            {
                dict.Add("HasAction", true);
            }

            return dict;
        }



        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }

        public async ValueTask SetContent(string id, string? str)
        {
            await Load(true);
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("setContent", id, str);
        }

        public async ValueTask<string> GetContent(string id)
        {
            await Load(true);
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<string>("getContent", id);
        }


    }
}
