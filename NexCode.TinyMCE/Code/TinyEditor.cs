using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor.Code
{



    public class TinyEditorIntaliser : JsInteropBase
    {
        public TinyEditorIntaliser(IJSRuntime jsRuntime) : base(jsRuntime)
        {
        }

        protected override string ScriptPath => "./_content/NexCode.TinyMCE.Blazor/TinyEditor.cs.js";

        protected override async ValueTask<IJSObjectReference> Load()
        {
            var loader = new ScriptLoader(JsRuntime,
                "./_content/NexCode.TinyMCE.Blazor/lib/tinymce/tinymce.min.js");
            await loader.LoadAndWait();

            return await base.Load();
        }


        public async ValueTask Init(string selector, IEnumerable<Plugin> plugins, Action? afterLoad = null,
            Action<EditorOptions>? onInitalise = null)
        {
            await Init(selector, plugins, afterLoad, objects => CallOnIntalise(onInitalise, objects));
            return;
                
            Task CallOnIntalise(Action<EditorOptions>? func, EditorOptions objects)
            {
                if (func != null)
                    func(objects);

                return Task.CompletedTask;
            }
            
        }


        public async ValueTask Init(IEditorScope editor, IEnumerable<Plugin> plugins, Action<IEditor>? afterLoad = null,
            Func<EditorOptions, Task>? onInitalise = null)
        {

            var orgAfterLoad = afterLoad;

            await Init(editor.EditorId, plugins, NewAfterLoad, onInitalise);

            void NewAfterLoad()
            {
                var tinyEditor = new TinyEditor(JsRuntime, editor);
                if (orgAfterLoad != null) 
                    orgAfterLoad.Invoke(tinyEditor);
            }


        }


        public async ValueTask Init(string selector, IEnumerable<Plugin> plugins, Action? afterLoad =null, Func<EditorOptions, Task>? onInitalise = null)
        {
            var plugs = new List<string>();
            var toolbar = new List<string>();
            var menu = new List<string>();
            var contextToolbar = new List<string>();
            var contextMenu = new List<string>();
            var contextForms = new List<string>();

            var options = new EditorOptions();
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

            if (options.Remove("menu", out object menuItems))
            {
                var items = (IEnumerable<Menu>)menuItems;
                options.Add("menu", items.ToDictionary(i=>i.Title));
            }


            if(onInitalise!=null)
                await onInitalise.Invoke(options);

            await InvokeVoidAsync("init", options);

            if(afterLoad!=null)
                afterLoad.Invoke();
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

    }

    public interface IJsEditor : IEditor
    {
        public ValueTask<T> InvokeAsync<T>(string funcName, params object?[]? args);
        public ValueTask InvokeVoidAsync(string funcName, params object?[]? args);
    }



    internal class TinyEditor : IJsEditor
    {

        #region Intalisation and JS calling

        [CascadingParameter]
        private IEditorScope? EditorScope { get; set; }


        public TinyEditor(IJSRuntime js) : this(js, null)
        {
        }

        public TinyEditor(IJSRuntime js, IEditorScope? scope)
        {
            Js = js;
            EditorScope = scope;
        }

        private IJSRuntime Js { get; }

      
        

        private async ValueTask<IJSObjectReference> GetEditor()
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

    }

}
