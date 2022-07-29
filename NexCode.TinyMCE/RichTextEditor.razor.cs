using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexCode.TinyMCEEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace NexCode.TinyMCE.Blazor
{
    public partial class RichTextEditor : ComponentBase
    {
        private string Id { get; } = "editor_"+ Guid.NewGuid().ToString().Replace("-","");

        [Inject] private EditorJs Js { get; set; } = default!;

        [Parameter] public RichTextDefaultEditorOptions Options { get; set; } = new();

        private RichTextDefaultEditorOptions? DefaultOptions { get; set; }

        public RichTextEditor() { }

        public RichTextEditor(RichTextDefaultEditorOptions? defaultOptions) : this()
        {
            DefaultOptions = defaultOptions;
        }


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


        protected override async Task OnInitializedAsync()
        {
            await Js.Load();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadEditor();
            }
        }

        private async Task LoadEditor()
        {
            Toolbar = TrimAndClean(Toolbar);
            Plugins = TrimAndClean(Plugins);
            MenuBar = TrimAndClean(MenuBar);

            await Js.Init(Id, Plugins, MenuBar, Toolbar);
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
        


    }
}
