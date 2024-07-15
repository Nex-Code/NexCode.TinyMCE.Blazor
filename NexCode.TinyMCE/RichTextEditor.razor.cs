﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using NexCode.TinyMCE.Blazor.Code;

namespace NexCode.TinyMCE.Blazor
{
    public partial class RichTextEditor : ComponentBase, IEditorScope
    {
        public string Id { get; } = "editor_"+ Guid.NewGuid().ToString().Replace("-", "");

        public string EditorId => Id;
        public bool Intalised { get; private set; }

        [Parameter]
        public EventCallback<EditorOptions> OnInitalise { get; set; }


        [Parameter] public bool DisableLoadOnRender { get; set; }
        [Parameter] public bool ContextMenuNeverUseNative { get; set; }

        [Parameter]
        public RenderFragment? Plugins { get; set; }

        [Parameter]
        public bool ExcludeDefaultPlugins { get; set; }


        [Inject] private TinyEditorIntaliser? Intaliser { get; set; } = default!;


        public IEditor Editor { get; private set; } = default!;

        private bool _loaded;

        private string IntalisedHtml { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            IntalisedHtml = _html;
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !DisableLoadOnRender)
            {
                await Init();
            }
        }

        public async Task Init()
        {
            if (_loaded)
                return;
            _loaded = true;

            await Intaliser!.Init(this, PluginList, CallOnInitalise, AfterLoad);

            return;


            async ValueTask AfterLoad(IEditor editor)
            {
                Intalised = true;
                Editor = editor;
                
                StateHasChanged();
                if(Intaliser!=null)
                    await Intaliser.DisposeAsync();

                await Editor.SetContent(Html);
            }

            async ValueTask CallOnInitalise(EditorOptions options)
            {

                options.Add("DotNetHelper", DotNetObjectReference.Create(this));
                options.Add("onchange", nameof(OnChange));
                options.Add("onblur", nameof(OnChange));

                if (Inline)
                    options.Add("inline", true);

                if (OnInitalise.HasDelegate)
                    await OnInitalise.InvokeAsync(options);
            }


        }

        private List<Plugin> PluginList { get; } = new List<Plugin>();

        internal void Register(Plugin plugin)
        {
            if (PluginList.Any(i => i.Name.Equals(plugin.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new ArgumentException($"Plugin with name ({plugin.Name}) already exists", nameof(plugin));

            PluginList.Add(plugin);
        }


        private async Task UpdateHtml()
        {
            var r = await Editor.GetContent();
            Html = r??string.Empty;
            if (HtmlChanged.HasDelegate)
                await HtmlChanged.InvokeAsync(Html);
        }

    }


    public interface IEditorScope
    {
        public string EditorId { get; }
        public bool Intalised { get; }
    }

}
