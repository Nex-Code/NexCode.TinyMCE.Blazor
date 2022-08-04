using Microsoft.JSInterop;
using NexCode.TinyMCE.Blazor.Plugins.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Plugins
{
    public abstract class BaseAction 
    {

        internal abstract string FuncName { get; }

        public string? Id { get; set; }
        public string? Text { get; set; }
        public string? Icon { get; set; }

        public string? ToolTip { get; set; }

        public bool Enabled { get; set; } = true;

        public Action<string>? Setup { get; set; } 
        public Action<string>? Action { get; set; }



        [JSInvokable]
        public virtual void OnSetup(string editorId) => Setup?.Invoke(editorId);

        [JSInvokable]
        public virtual void OnAction(string editorId) => Action?.Invoke(editorId);




    }

  

}
