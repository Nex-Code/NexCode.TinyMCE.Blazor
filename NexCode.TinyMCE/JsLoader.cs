using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor
{
    internal class JsLoader
    {
        private static ConcurrentDictionary<string, SemaphoreSlim?> _semaphores =
            new ConcurrentDictionary<string, SemaphoreSlim?>();


        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        public JsLoader(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NexCode.TinyMCE.Blazor/jsLoader.js").AsTask());
        }


        private string Url { get; set; } = string.Empty;

        private SemaphoreSlim? Semaphore => _semaphores.GetValueOrDefault(Url);

        public bool Loaded => _semaphores.ContainsKey(Url) && _semaphores[Url] == null;

        public async void Load(string url)
        {
            await InternalLoad(url);
        }

        private async Task InternalLoad(string url)
        {
            if (!(string.IsNullOrWhiteSpace(Url) || Url.Equals(url)))
                throw new InvalidOperationException("Must create a new JsLoader for each url");

            Url = url;

            if (Loaded)
                return;

            _semaphores.TryAdd(Url, new SemaphoreSlim(1, 1));

            var semaphore = Semaphore;

            if (semaphore == null)
                return;

            await semaphore.WaitAsync();

            if (Loaded)
                return;
            try
            {
                var module = await _moduleTask.Value;
                var dotNetHelper = DotNetObjectReference.Create(this);
                await module.InvokeVoidAsync("loadJs", url, dotNetHelper, nameof(LoadComplete));
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }


        public async Task LoadAndWait(string url)
        {
            await InternalLoad(url);

            if (Loaded)
                return;

            await (Semaphore?.WaitAsync()?? Task.CompletedTask);
        }

        [JSInvokable]
        public void LoadComplete()
        {
            var semaphore = Semaphore;
            _semaphores[Url] = null;
            if (semaphore != null)
            {
                try
                {
                    while(true)
                        semaphore.Release();
                }
                catch (SemaphoreFullException ex)
                {
                    //ignore this.
                }
                finally
                {
                    semaphore.Dispose();
                }
            }

            
        }




    }
}
