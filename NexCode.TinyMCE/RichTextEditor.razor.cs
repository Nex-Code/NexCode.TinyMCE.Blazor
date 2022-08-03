using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexCode.TinyMCEEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

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

        [Parameter] public IEnumerable<BlazorPlugin> DynamicPlugins { get; set; } = Array.Empty<BlazorPlugin>();


        [Parameter] public bool LoadOnRender { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await Js.Load();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && LoadOnRender)
            {
                await Load();
            }
        }

        public async Task Load()
        {
            Toolbar = TrimAndClean(Toolbar);
            Plugins = TrimAndClean(Plugins);
            MenuBar = TrimAndClean(MenuBar);

            DynamicPlugins = DynamicPlugins.Where(i => i != null).ToArray();

            await Js.Init(Id, Plugins, MenuBar, Toolbar, DynamicPlugins);
        }

        public async Task Destroy()
        {
            await Js.Destroy(Id);
        }

        public async Task Refresh()
        {
            await Destroy();
            await Load();
        }


        private string? TrimAndClean(string? str)
        {
            if (str.IsNullOrWhiteSpace())
                return null;

            return str.Trim();
        }




        public async ValueTask<string> GetHtml()
        {
            var html =  await Js.GetContent(Id);
            if (html.StartsWith("<!--!-->"))
                html = html.Replace("<!--!-->", "").Trim();

            return html;
        }

        public async ValueTask SetHtml(string? html)
        {
            await Js.SetContent(Id, html);
            StateHasChanged();
        }


    }
}
