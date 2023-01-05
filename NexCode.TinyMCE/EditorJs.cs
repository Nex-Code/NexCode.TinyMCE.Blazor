using System.Runtime.InteropServices;
using NexCode.TinyMCE.Blazor;
using Microsoft.JSInterop;
using NexCode.TinyMCE.Blazor.Plugins;
using NexCode.TinyMCE.Blazor.Plugins.MenuItem;
using NexCode.TinyMCE.Blazor.Plugins.Toolbar;
using System.Xml.Linq;

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

        #region boring bits

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
        
        public async ValueTask Init(string id, string? plugins = null, string? menubar = null, string? toolbar = null, IEnumerable<Plugin>? blazorPlugins = null, bool? branding=null)
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


        public async ValueTask RegisterPlugin(Plugin plugin)
        {

            var toolbar = plugin.Toolbar.Select(i => JsConverter.Process(i, plugin.Name));
            var menuItem = plugin.MenuItems.Select(i => JsConverter.Process(i, plugin.Name));
            var uiElements = toolbar.Concat(menuItem).ToArray();

            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("registerPlugin", plugin.Name.ToLower(), uiElements);
        }

        public async ValueTask RemovePlugin(string name)
        {
            await (await _moduleTask.Value).InvokeVoidAsync("removePlugin", name);
        }

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }

        #endregion

        #region Editor

        public async ValueTask<string?> GetContent(string id, object? args = null)
        {
            var content =  await (await _moduleTask.Value).InvokeAsync<string?>("getContent", id, args);
            if(!content.IsNullOrWhiteSpace())
                content = content.Replace("<!--!-->", "").Trim();
            return content;
        }

        public async ValueTask<string?> GetParam(string id, string name, string defaultValue, string type)
        {
            return await (await _moduleTask.Value).InvokeAsync<string?>("getParam", id, name, defaultValue, type);
        }

        public async ValueTask<bool> HasPlugin(string id, string name, bool loaded = false)
        {
            return await (await _moduleTask.Value).InvokeAsync<bool>("hasPlugin", id, name, loaded);
        }


        public async ValueTask Hide(string id)
        {
            await (await _moduleTask.Value).InvokeVoidAsync("hide", id);
        }
        public async ValueTask<string?> Load(string id)
        {
            return await (await _moduleTask.Value).InvokeAsync<string?>("load", id);
        }

        public async ValueTask Remove(string id)
        {
            await (await _moduleTask.Value).InvokeVoidAsync("remove", id);
        }

        public async ValueTask<string?> Save(string id)
        {
            return await (await _moduleTask.Value).InvokeAsync<string?>("save", id);
        }

        public async ValueTask<string?> SetContent(string id, string? content)
        {
            return await (await _moduleTask.Value).InvokeAsync<string?>("setContent", id, content);
        }

        public async ValueTask<string?> InsertContent(string id, string? content, object? args=null)
        {
            return await (await _moduleTask.Value).InvokeAsync<string?>("insertContent", id, content, args);
        }

        public async ValueTask<bool> SetProgressState(string id, bool state, int? time)
        {
            return await (await _moduleTask.Value).InvokeAsync<bool>("setProgressState", id,state, time);
        }
        public async ValueTask Show(string id)
        {
            await (await _moduleTask.Value).InvokeAsync<string?>("show", id);
        }


        #endregion





    }
}
