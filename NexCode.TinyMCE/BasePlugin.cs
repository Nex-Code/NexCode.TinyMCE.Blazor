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


    }


}
