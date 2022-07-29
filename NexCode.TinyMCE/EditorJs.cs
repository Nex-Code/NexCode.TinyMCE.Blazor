using System.Runtime.InteropServices;
using NexCode.TinyMCE.Blazor;
using Microsoft.JSInterop;

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



        public async ValueTask Init(string id, string? plugins = null, string? menubar = null, string? toolbar = null)
        {
            await Load(true);

            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("init", id, plugins, menubar, toolbar);
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
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("setContent",id, str);
        }

        public async ValueTask<string> GetContent(string id)
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<string>("getContent",id);
        }


    }
}
