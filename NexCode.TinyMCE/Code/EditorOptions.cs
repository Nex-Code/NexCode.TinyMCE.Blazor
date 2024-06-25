using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Code
{
    public class EditorOptions : Dictionary<string, object>
    {


        public string? Selector
        {
            get => this.GetValueOrDefault("selector")?.ToString();
            set => Replace("selector", value);
        }

        public string? Plugins
        {
            get => this.GetValueOrDefault("plugins")?.ToString();
            set => Replace("plugins", value);
        }


        public string? Toolbar
        {
            get => this.GetValueOrDefault("toolbar")?.ToString();
            set => Replace("toolbar", value);
        }

        public string? Menubar
        {
            get => this.GetValueOrDefault("menubar")?.ToString();
            set => Replace("menubar", value);
        }

        public string? Contextmenu
        {
            get => this.GetValueOrDefault("contextmenu")?.ToString();
            set => Replace("contextmenu", value);
        }

        public string? CustomElements
        {
            get => this.GetValueOrDefault("custom_elements")?.ToString();
            set => Replace("custom_elements", value);
        }

        public string? CustomCss
        {
            get => this.GetValueOrDefault("content_css")?.ToString();
            set => Replace("content_css", value);
        }

        public string? ContentStyle
        {
            get => this.GetValueOrDefault("content_style")?.ToString();
            set => Replace("content_style", value);
        }


        public string? ExtendedValidElements
        {
            get => this.GetValueOrDefault("extended_valid_elements")?.ToString();
            set => Replace("extended_valid_elements", value);
        }

        public void Replace(string key, object? value)
        {
            this.Remove(key);
            if(value!=null)
                this.Add(key, value);
        }
      
    }
}
