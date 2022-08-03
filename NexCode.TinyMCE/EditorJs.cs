using System.Runtime.InteropServices;
using NexCode.TinyMCE.Blazor;
using Microsoft.JSInterop;

namespace NexCode.TinyMCEEditor
{
    internal class EditorJs : IAsyncDisposable
    {

        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private JsLoader JsLoader { get; }

        private IEnumerable<JsPlugin> Plugins { get; }

        public EditorJs(IJSRuntime jsRuntime, JsLoader jsLoader, IEnumerable<JsPlugin> plugins)
        {
            JsLoader = jsLoader;
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NexCode.TinyMCE.Blazor/editorJs.js").AsTask());
            Plugins = plugins;
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

        public async ValueTask Init(string id, string? plugins = null, string? menubar = null, string? toolbar = null, IEnumerable<BlazorPlugin>? blazorPlugins = null)
        {
            await Load(true);


            var externalPlugins = new Dictionary<string,string>();
            if (!plugins.IsNullOrWhiteSpace())
            {
                var pList = plugins.Split(" ").ToHashSet();

                foreach (var plugin in Plugins)
                {
                    if (pList.Contains(plugin.Name))
                    {
                        pList.Remove(plugin.Name);
                        externalPlugins.Add(plugin.Name, plugin.JsUrl);
                    }
                }

                plugins = string.Join(" ", pList);

            }

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


        private async ValueTask RegisterPlugin(BlazorPlugin plugin)
        {
            var module = await _moduleTask.Value;

            var buttons = plugin.Buttons.Select(i=>new {i.Text, dotNethelper = DotNetObjectReference.Create(i), });
            var menuItems = plugin.MenuItems.Select(i => new { i.Text, dotNethelper = DotNetObjectReference.Create(i) });
            var tooldropDownItems = plugin.ToolbarButtons.Select(i => new
                { Text = i.Text, i.Scope, i.Position, Items = i.Items.Select(j => new ProcessedItem(j)) });
            await module.InvokeVoidAsync("registerPlugin", plugin.Name.ToLower(), buttons, menuItems, tooldropDownItems);
        }

        private class ProcessedItem
        {
            public string Text { get; set; }
            public IEnumerable<ProcessedItem>? Items { get; set; }
            public DotNetObjectReference<BlazorPlugin.ToolbarButton.Item> DotNethelper { get; set; }

            public ProcessedItem(BlazorPlugin.ToolbarButton.Item item)
            {
                Text = item.Text;
                Items = item.SubItems?.Select(i=>new ProcessedItem(i));
                DotNethelper = DotNetObjectReference.Create(item);
            }
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
