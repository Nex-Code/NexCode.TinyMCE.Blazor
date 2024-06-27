using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexCode.TinyMCE.Blazor.Code;
using Microsoft.AspNetCore.Components.Rendering;

namespace NexCode.TinyMCE.Blazor
{
    public class CustomPlugin : Plugin
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }


        public IReadOnlyList<BaseMenuItem> Items => ChildItems.AsReadOnly();

        private List<BaseMenuItem> ChildItems { get; } = new List<BaseMenuItem>();

        internal void Register(BaseMenuItem item)
        {
            ChildItems.Add(item);
        }



        private IEnumerable<string> _toolbar = Array.Empty<string>();
        private IEnumerable<string> _menubar = Array.Empty<string>();
        private IEnumerable<string> _contextMenu = Array.Empty<string>();


        [Parameter]
        public override string? Toolbar
        {
            get => Combine(_toolbar, MenuItemLocation.Toolbar);
            set => _toolbar = value?.Split(" ")??Array.Empty<string>();
        }


        [Parameter]
        public override string? Menubar
        {
            get => Combine(_menubar, MenuItemLocation.Menubar);
            set => _menubar = value?.Split(" ")??Array.Empty<string>();
        }

        [Parameter]
        public override string? ContextMenu
        {
            get => Combine(_contextMenu, MenuItemLocation.Contextmenu);
            set => _contextMenu = value?.Split(" ")??Array.Empty<string>();
        }

        private string? Combine(IEnumerable<string> overrides, MenuItemLocation location)
        {
            var other = Items.Where(i => i.Show && i.Location.HasFlag(location) && !overrides.Contains(i.Name, StringComparer.CurrentCultureIgnoreCase)).Select(i=>i.Name).ToArray();
            var all = overrides.Concat(other).ToArray();

            if (!all.Any())
                return null;

            var r = string.Join(" ", all);
            return r;
        }


        protected override void BuildRenderTree(RenderTreeBuilder __builder)
        {
            if (ChildContent != null)
            {
                __builder.AddCascadingValue<CustomPlugin>(this, ChildContent);
            }
            base.BuildRenderTree(__builder);
        }
    }

    public record RegisterableItem(string Func, object Details);
}
