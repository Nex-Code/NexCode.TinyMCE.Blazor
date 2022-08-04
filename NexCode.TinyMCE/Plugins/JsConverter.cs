using Microsoft.JSInterop;
using NexCode.TinyMCE.Blazor.Plugins.MenuItem;
using NexCode.TinyMCE.Blazor.Plugins.Toolbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Plugins
{
    internal static class JsConverter
    {

        public static IEnumerable<Dictionary<string, object?>> Process(this IEnumerable<BasicToolbarButton> actions, string pluginName)
        {
            return actions.Select(i => i.Process(pluginName)).ToArray(); ;
        }

        public static IEnumerable<Dictionary<string, object?>> Process(this IEnumerable<MenuToolbarButtonBase> actions, bool processChildren = false)
        {
            return actions.Select(i=>Process(i, processChildren)).ToArray();
        }

        public static IEnumerable<Dictionary<string, object?>> Process(this IEnumerable<BasicMenuItem> actions, string pluginName)
        {
            return actions.Select(i=>i.Process(pluginName)).ToArray(); ;
        }


        public static Dictionary<string, object?> Process(this BasicToolbarButton action, string pluginName)
        {
            var dict = BaseProcess(action, pluginName, out bool requiresDotNetHelper);


            if (action is SplitToolbarButton splitbutton)
            {

                dict.Add(nameof(splitbutton.Columns), splitbutton.Columns);

                if (splitbutton.ItemAction != null)
                {
                    dict.Add("hasItemAction", true);
                    requiresDotNetHelper = true;
                }
            }

            if (action is SplitToolbarButton || action is MenuToolbarButtonGroup || action is ToggleToolbarButton)
            {
                requiresDotNetHelper = true;
            }

            if (action is MenuToolbarButtonGroup mtbg)
            {
                requiresDotNetHelper = true;
            }

            if (requiresDotNetHelper)
                dict.Add("dotNetHelper", DotNetObjectReference.Create(action));


            if (action is GroupToolbarButton gTb)
            {
                dict.Add(nameof(gTb.Items), gTb.Items);
            }


            return dict;
        }


        
        public static Dictionary<string, object?> Process(this MenuToolbarButtonBase action, bool processChildren = false)
        {
            var dict = BaseProcess(action, null, out bool requiresDotNetHelper);

            dict.Add("type", action.Type);

            if (action is MenuToolbarButtonGroup group)
            {
                if (!processChildren)
                {
                    requiresDotNetHelper = true;
                }
                else if(group.Items?.Any()??false)
                {
                    dict.Add("items", group.Items.Select(i=>Process(i,true)) );
                }
            }

            if(requiresDotNetHelper)
                dict.Add("dotNetHelper", DotNetObjectReference.Create(action));

            return dict;

        }

        public static Dictionary<string, object?> Process(this BasicMenuItem action, string pluginName)
        {
            var dict = BaseProcess(action, pluginName, out bool requiresDotNetHelper);

            dict.Add("value", action.Value);
            dict.Add("shortcut", action.Shortcut);


            if (action is NestedMenuItem)
            {
                requiresDotNetHelper = true;
            }

            if (action is ToggleMenuItem ttbutton)
            {
                dict.Add(nameof(ttbutton.Active), ttbutton.Active);
            }

            if (requiresDotNetHelper)
                dict.Add("dotNetHelper", DotNetObjectReference.Create(action));

            return dict;
        }

        private static Dictionary<string, object?> BaseProcess(BaseAction action, string? pluginName, out bool addDotnetHelper)
        {
            var dict = new Dictionary<string, object?>();
            dict.Add("text", action.Text);
            dict.Add("icon", action.Icon);
            dict.Add("tooltip", action.ToolTip);
            dict.Add("enabled", action.Enabled);
            dict.Add("id", action.Id ?? pluginName);
            dict.Add("funcName", action.FuncName);

            addDotnetHelper = false;

            if (action.Setup != null)
            {
                dict.Add("hasSetup", true);
                addDotnetHelper = true;
            }

            if (action.Action != null)
            {
                dict.Add("hasAction", true);
                addDotnetHelper = true;
            }

            return dict;
        }

    }
}
