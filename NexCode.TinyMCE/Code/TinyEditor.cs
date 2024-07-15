using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor.Code
{



    public class TinyEditorIntaliser : JsInteropBase
    {
        private TinyMCESettings Settings { get; }

        public TinyEditorIntaliser(IJSRuntime jsRuntime, IOptions<TinyMCESettings> config) : base(jsRuntime)
        {
            Settings = config.Value;
        }

        protected override string ScriptPath => "./_content/NexCode.TinyMCE.Blazor/TinyEditor.cs.js";

        protected override async ValueTask<IJSObjectReference> Load()
        {
            var loader = new ScriptLoader(JsRuntime,
                "./_content/NexCode.TinyMCE.Blazor/lib/tinymce/tinymce.min.js");
            await loader.LoadAndWait();

            return await base.Load();
        }




        public async ValueTask Init(IEditorScope editor, IEnumerable<Plugin> plugins) =>
            await Init(editor, plugins, options => ValueTask.CompletedTask, editor1 => ValueTask.CompletedTask);

        public async ValueTask Init(IEditorScope editor, IEnumerable<Plugin> plugins,
            Func<EditorOptions, ValueTask> onInitalise) =>
            await Init(editor, plugins, onInitalise, editor1 => ValueTask.CompletedTask);

        public async ValueTask Init(IEditorScope editor, IEnumerable<Plugin> plugins,
            Func<IEditor, ValueTask> afterLoad) =>
            await Init(editor, plugins, options => ValueTask.CompletedTask, afterLoad);

        public async ValueTask Init(IEditorScope editor, IEnumerable<Plugin> plugins, Func<EditorOptions, ValueTask> onInitalise, Func<IEditor, ValueTask> afterLoad)
        {
            await Init(editor.EditorId, plugins, onInitalise, NewAfterLoad);

            async ValueTask NewAfterLoad()
            {
                var tinyEditor = new TinyEditor(JsRuntime, editor);
                await afterLoad.Invoke(tinyEditor);
            }

        }


        public async ValueTask Init(string selector, IEnumerable<Plugin> plugins) =>
            await Init(selector, plugins, options => ValueTask.CompletedTask, () => ValueTask.CompletedTask);

        public async ValueTask Init(string selector, IEnumerable<Plugin> plugins,
            Func<EditorOptions, ValueTask> onInitalise) =>
            await Init(selector, plugins, onInitalise, () => ValueTask.CompletedTask);

        public async ValueTask Init(string selector, IEnumerable<Plugin> plugins,
            Func<ValueTask> afterLoad) =>
            await Init(selector, plugins, options => ValueTask.CompletedTask, afterLoad);



        public async ValueTask Init(string selector, IEnumerable<Plugin> plugins, Func<EditorOptions, ValueTask> onInitalise, Func<ValueTask> afterLoad)
        {
            var plugs = new List<string>();
            var toolbar = new List<string>();
            var menu = new List<string>();
            var contextToolbar = new List<string>();
            var contextMenu = new List<string>();
            var contextForms = new List<string>();

            var options = new EditorOptions()
            {
                Promotion = Settings.ShowPromotion,
                Branding = Settings.ShowBranding,
                LicenseKey = Settings.LicenseKey
            };

            var pluginRegisters = new List<ValueTask>();

            options.Selector = "#"+selector;

            foreach (var plugin in plugins)
            {
                plugs.Add(plugin.Name);

                toolbar.AddIf(plugin.Toolbar, s => s.IsNotNullOrWhiteSpace());
                contextMenu.AddIf(plugin.ContextMenu, s => s.IsNotNullOrWhiteSpace());
                contextToolbar.AddIf(plugin.ContextToolbar, s => s.IsNotNullOrWhiteSpace());
                contextForms.AddIf(plugin.ContextForms, s => s.IsNotNullOrWhiteSpace());
               

                if (plugin.AdditionalConfig != null)
                    plugin.AdditionalConfig(options);


                menu.AddIf(plugin.Menubar, s => s.IsNotNullOrWhiteSpace());

                if (plugin is CustomPlugin custom)
                {
                    var vt = RegisterPlugin(custom);
                    pluginRegisters.Add(vt);
                }
            }

            await pluginRegisters.WhenAll();

            if(plugs.Any())
                options.Plugins = string.Join(" ",plugs);
            if (toolbar.Any())
                options.Toolbar = string.Join(" ", toolbar);
            if (menu.Any())
                options.Menubar = string.Join(" ", menu);
            if (contextMenu.Any())
                options.Contextmenu = string.Join(" ", contextMenu);

            if (options.Remove("menu", out object? menuItems))
            {
                if (menuItems is IEnumerable<Menu> items)
                    options.Add("menu", items.ToDictionary(i=>i.Title));
            }

            await onInitalise.Invoke(options);
            await InvokeVoidAsync("init", options);
            await afterLoad.Invoke();
        }

        public async ValueTask RegisterPlugin(CustomPlugin plugin)
        {
            var registerableItems = plugin.GetRegisterableItems();
            if(registerableItems.Any())
                await InvokeVoidAsync("registerPlugin", plugin.Name, registerableItems);
        }


    }


    public interface IEditor
    {
        public ValueTask<string?> GetContent();
        public ValueTask SetContent(string value);
        public ValueTask InsertContent(string value, object? args=null);


        public EditorSelection Selections { get; }




        public void On(string eventName, EventCallback callback);



    }

    public interface IJsEditor : IEditor
    {
        public ValueTask<T> InvokeAsync<T>(string funcName, params object?[]? args);
        public ValueTask InvokeVoidAsync(string funcName, params object?[]? args);
    }

    public abstract class EditorBase
    {

        #region Intalisation and JS calling

        [CascadingParameter]
        protected IEditorScope? EditorScope { get; set; }

        protected EditorBase(IJSRuntime js) : this(js, null)
        {
        }

        protected EditorBase(IJSRuntime js, IEditorScope? scope)
        {
            Js = js;
            EditorScope = scope;
        }

        protected IJSRuntime Js { get; }

        protected virtual async ValueTask<IJSObjectReference> GetEditor()
        {
            if (EditorScope == null || !EditorScope.Intalised)
                return await Js.InvokeAsync<IJSObjectReference>("tinymce.activeEditor");

            return await Js.InvokeAsync<IJSObjectReference>("tinymce.EditorManager.get", EditorScope.EditorId);
        }

        public virtual async ValueTask<T> InvokeAsync<T>(string funcName, params object?[]? args)
        {
            var editor = await GetEditor();
            var result = await editor.InvokeAsync<T>(funcName, args);
            return result;
        }

        public virtual async ValueTask InvokeVoidAsync(string funcName, params object?[]? args)
        {
            var editor = await GetEditor();
            await editor.InvokeVoidAsync(funcName, args);
        }

        #endregion
    }

    internal class TinyEditor : EditorBase, IJsEditor
    {

        public TinyEditor(IJSRuntime js) : base(js)
        {
        }

        public TinyEditor(IJSRuntime js, IEditorScope? scope) : base(js, scope)
        {
        }

        public async ValueTask<string?> GetContent()
        {
            var r = await InvokeAsync<string>("getContent");
            return r;
        }

        public async ValueTask SetContent(string value)
        {
            await InvokeVoidAsync("setContent", value);
        }

        public async ValueTask InsertContent(string value, object? args = null)
        {
            await InvokeVoidAsync("insertContent", value, args);
        }

        private EditorSelection _selection;
        public EditorSelection Selections => new EditorSelection(Js, EditorScope);


    }



    public class EditorSelection : EditorBase
    {

        public EditorSelection(IJSRuntime js) : base(js)
        {
        }

        public EditorSelection(IJSRuntime js, IEditorScope? scope) : base(js, scope)
        {
        }

        /*protected override async ValueTask<IJSObjectReference> GetEditor()
        {
            var js = await base.GetEditor();
            var selectJs = await js.InvokeAsync<IJSObjectReference>("selection");
            return selectJs;
        }*/

        public override async ValueTask<T> InvokeAsync<T>(string funcName, params object?[]? args) =>
            await base.InvokeAsync<T>($"selection.{funcName}", args);
      

        public virtual async ValueTask InvokeVoidAsync(string funcName, params object?[]? args) =>
            await base.InvokeVoidAsync($"selection.{funcName}", args);



        /*public ValueTask<IJSObjectReference> GetNode() => InvokeAsync<IJSObjectReference>("selection.getNode");
        public ValueTask<string> GetSel() => InvokeAsync<string>("getSel");*/



        public async ValueTask<HtmlNode?> GetNode()
        {
            var r = await InvokeAsync<IJSObjectReference>("getNode");
            var node = await Js.InvokeAsync<HtmlNode?>("convertElementToObject", r);
            return node;
        } 


        public ValueTask<string> GetContent() => InvokeAsync<string>("getContent");
        public ValueTask SetContent(string html) => InvokeVoidAsync("setContent", html);



    }


    public sealed class HtmlNode
    {

        [JsonPropertyName("nodeName")]
        public string? Name { get; set; }
        [JsonPropertyName("nodeValue")]
        public string? Value { get; set; }
        [JsonPropertyName("content")]
        public string? Content { get; set; }
        [JsonPropertyName("children")]
        public IEnumerable<HtmlNode> Children { get; set; } = Array.Empty<HtmlNode>();

        [JsonPropertyName("attributes")]
        public IDictionary<string, string?> Attributes { get; set; } = new Dictionary<string, string?>();

    }

}
