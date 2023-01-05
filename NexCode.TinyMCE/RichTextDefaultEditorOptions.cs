using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor
{
    public class RichTextDefaultEditorOptions : RichTextEditorOptions
    {
    
    }

    public class RichTextEditorOptions
    {
        public string? Toolbar { get; set; }
        public string? Plugins { get; set; }
        public string? MenuBar { get; set; }
        public bool? Branding { get; set; }
    }

}
