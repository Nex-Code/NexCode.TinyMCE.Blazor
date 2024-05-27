using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Code
{
    internal sealed class JsLoaderFactory
    {

        private IJSRuntime JsRuntime { get; }
        public JsLoaderFactory(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }


        public ScriptLoader Build(string url, Action? onSuccess = null, Action? onError = null)
        {
            var loader = new ScriptLoader(JsRuntime,
                onSuccess ?? (() => { }),
                onError ?? (() => { }),
                url);

            return loader;
        }

        public async ValueTask LoadAsync(string url, Action? onSuccess = null, Action? onError = null, CancellationToken cancellationToken = default)
        {
            var loader = Build(url, onSuccess, onError);
            await loader.Load(cancellationToken);
        }

        public async void Load(string url, Action? onSuccess = null, Action? onError = null, CancellationToken cancellationToken = default)
        { 
            await LoadAsync(url, onSuccess, onError, cancellationToken).ConfigureAwait(false);
        }

    }

    internal sealed class ScriptLoader : JsInteropBase
    {

        private static readonly ConcurrentDictionary<string, SemaphoreSlim?> Semaphores = new ();


        private Action OnSuccess { get; }
        private Action OnError { get; }
        private string Url { get; }


        internal ScriptLoader(IJSRuntime jsRuntime, Action onSuccess, Action onError, string url) : base(jsRuntime)
        {
            OnSuccess = onSuccess;
            OnError = onError;
            Url = url;
        }

        internal ScriptLoader(IJSRuntime jsRuntime, string url) : this(jsRuntime, () => { }, () => { }, url)
        {
            
        }


        protected override string ScriptPath => "./_content/NexCode.TinyMCE.Blazor/JsLoaderFactory.cs.js";



        public async ValueTask Load(CancellationToken cancellationToken = default)
        {
            _ = await InternalLoad(cancellationToken);
        }


        public async ValueTask LoadAndWait(CancellationToken cancellationToken = default)
        {
            var sem = await InternalLoad(cancellationToken);

            if(sem is { CurrentCount: 0 })
                try {
                    await sem.WaitAsync(cancellationToken);
                }catch(ObjectDisposedException){}
                
        }


        private async ValueTask<SemaphoreSlim?> InternalLoad(CancellationToken cancellationToken)
        {
            if (Semaphores.TryGetValue(Url.ToLower(), out var existingSemaphore))
            {
                if (existingSemaphore!=null)
                    await existingSemaphore.WaitAsync(cancellationToken);

                JsOnSuccess();
                return null;
            }

            var semaphore = new SemaphoreSlim(1,1);
            if (!Semaphores.TryAdd(Url.ToLower(), semaphore))
                throw new Exception("Unable to create semaphore lock");

            await semaphore.WaitAsync(cancellationToken);

            var dotNetHelper = DotNetObjectReference.Create(this);
            var added = await InvokeAsync<bool>("importScript", Url, dotNetHelper, nameof(JsOnSuccess), nameof(JsOnError));

            if (!added)
            {
                JsOnSuccess();
                return null;
            }

            return semaphore;
        }


        [JSInvokable]
        public void JsOnSuccess()
        {
            OnSuccess.Invoke();
            ReleaseLocks();
        }

        [JSInvokable]
        public void JsOnError()
        {
            OnError.Invoke();
            ReleaseLocks();
        }


        private void ReleaseLocks()
        {
            if (!Semaphores.TryGetValue(Url.ToLower(), out var semaphore) || semaphore==null)
                return;

            Semaphores[Url.ToLower()] = null;
            semaphore.ReleaseAll();
            semaphore.Dispose();
        }


    }

    
}
