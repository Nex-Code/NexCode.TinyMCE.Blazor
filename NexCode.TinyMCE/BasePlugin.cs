using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor
{
    public class JsPlugin
    {

        public string Name { get; }

        public string JsUrl { get; }
      
    }

    public class BlazorPlugin
    {

        public string Name { get; init; }
        
        public List<BaseAction> Buttons { get; set; } = new List<BaseAction>();
        public List<BaseAction> MenuItems { get; set; } = new List<BaseAction>();

        public List<ToolbarButton> ToolbarButtons { get; set; } = new List<ToolbarButton>();

        public class BaseAction
        {
            public string Text { get; set; }
            public Action OnAction { get; set; }

            [JSInvokable("TriggerAction")]
            public void TriggerAction()
            {
                OnAction.Invoke();
            }
        }


        public class ToolbarButton
        {
            public string Text { get; set; }
            public List<Item> Items { get; set; }
            public string Position { get; set; }
            public string Scope { get; set; }

            public class Item
            {
                public IEnumerable<Item>? SubItems { get; set; }
                public string Text { get; set; }
                public Action? OnAction { get; set; }

                [JSInvokable("TriggerAction")]
                public void TriggerAction()
                {
                    if(OnAction!=null)
                        OnAction.Invoke();
                }
            }
        }

    }


}
