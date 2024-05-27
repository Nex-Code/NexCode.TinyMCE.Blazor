using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Code
{
    public abstract class JsInteropBase : IAsyncDisposable, IDisposable
    {
        protected abstract string ScriptPath { get; }
        private Lazy<ValueTask<IJSObjectReference>> ModuleTask { get; }

        protected ValueTask<IJSObjectReference> Module => ModuleTask.Value;
        protected IJSRuntime JsRuntime { get; }

        protected JsInteropBase(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
            ModuleTask = new Lazy<ValueTask<IJSObjectReference>>(Load);
        }

        protected virtual async ValueTask<IJSObjectReference> Load()
        {
            var module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", ScriptPath);
            return module;
        }
        

        protected virtual async ValueTask<T> InvokeAsync<T>(string funcName, params object?[]? args)
        {
            var js = await Module;
            var result = await js.InvokeAsync<T>(funcName, args);
            return result;
        }

        protected virtual async ValueTask InvokeVoidAsync(string funcName, params object?[]? args)
        {
            var js = await Module;
            await js.InvokeVoidAsync(funcName, args);
        }



        #region Dispose

        protected bool Disposed { get; private set; }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!disposing)
                return;

            DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (Disposed)
                return;

            if (ModuleTask.IsValueCreated)
            {
                var module = await ModuleTask.Value;
                await module.DisposeAsync();
            }

            Disposed = true;
        }

        #endregion

    }
}
