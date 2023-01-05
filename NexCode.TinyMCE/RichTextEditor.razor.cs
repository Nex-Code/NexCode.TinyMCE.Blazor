using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexCode.TinyMCEEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NexCode.TinyMCE.Blazor.Plugins;

namespace NexCode.TinyMCE.Blazor
{
    public partial class RichTextEditor : ComponentBase
    {
        public string Id { get; } = "editor_"+ Guid.NewGuid().ToString().Replace("-","");

        [Inject] private EditorJs Js { get; set; } = default!;

        [Parameter] 
        public RichTextEditorOptions Options { get; set; } = new();

        [Inject]
        private RichTextDefaultEditorOptions? DefaultOptions { get; set; }

        [Parameter]
        public string? Toolbar
        {
            get => Options.Toolbar ?? DefaultOptions?.Toolbar;
            set => Options.Toolbar = value;
        }
        [Parameter]
        public string? Plugins
        {
            get => Options.Plugins ?? DefaultOptions?.Plugins;
            set => Options.Plugins = value;
        }
        [Parameter]
        public string? MenuBar
        {
            get => Options.MenuBar ?? DefaultOptions?.MenuBar;
            set => Options.MenuBar = value;
        }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter] public IEnumerable<Plugin> DynamicPlugins { get; set; } = Array.Empty<Plugin>();


        [Parameter] public bool LoadOnRender { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await Js.Load();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && LoadOnRender)
            {
                await Init();
            }
        }

        public async Task Init()
        {
            Toolbar = TrimAndClean(Toolbar);
            Plugins = TrimAndClean(Plugins);
            MenuBar = TrimAndClean(MenuBar);

            DynamicPlugins = DynamicPlugins.Where(i => i != null).ToArray();

            await Js.Init(Id, Plugins, MenuBar, Toolbar, DynamicPlugins, Options?.Branding);
        }




        private string? TrimAndClean(string? str)
        {
            if (str.IsNullOrWhiteSpace())
                return null;

            return str.Trim();
        }




        public async ValueTask<string?> GetContent()
        {
            return await Js.GetContent(Id);
            
        }

        public async ValueTask SetContent(string? html)
        {
            html ??= string.Empty;

            await Js.SetContent(Id, html);
        }

        public async ValueTask<string?> InsertContent(string content, object? args = null)
        {
            return await Js.InsertContent(Id, content, args);
        }

        public async ValueTask Remove()
        {
            await Js.Remove(Id);
        }

        
        public async ValueTask Load()
        {
            await Js.Load(Id);
        }

        public async ValueTask<bool> SetProgressState(bool state, int? time = null)
        {
            return await Js.SetProgressState(Id, state, time);
        }

        public async ValueTask<bool> HasPlugin(string name, bool loaded = true)
        {
            return await Js.HasPlugin(Id, name, loaded);
        }


    }
}
