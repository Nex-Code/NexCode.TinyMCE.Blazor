using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using NexCode.TinyMCE.Blazor.Code;

namespace NexCode.TinyMCE.Blazor
{
    public class Plugin : ComponentBase
    {
        [Parameter]
        public virtual string Name { get; set; }

        [Parameter]
        public virtual string? Toolbar { get; set; }


        [Parameter]
        public virtual string? Menubar { get; set; }

        [Parameter]
        public virtual string? ContextMenu { get; set; }

        [Parameter]
        public virtual string? ContextToolbar { get; set; }

        [Parameter]
        public virtual string? ContextForms { get; set; }


        [CascadingParameter]
        private RichTextEditor? Parent { get; set; }


        public Action<IDictionary<string,object>>? AdditionalConfig { get; protected set; }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            if(Parent is not null)
                Parent.Register(this);
        }

        protected void AddMenu(IDictionary<string, object> obj, params Menu[] menu)
        {
            var existingList = new List<Menu>();
            if (obj.TryGetValue("menu", out var value))
                existingList = (List<Menu>)value;
            else
                obj.Add("menu", existingList);

            existingList.AddRange(menu);
        }
    }


    public class DefaultPlugins : Plugin
    {

        

        public IEnumerable<string> PluginNames { get; private set; }
        public IEnumerable<string> ToolbarNames { get; private set; }
        public IEnumerable<string> MenuName { get; private set; }



        [Parameter] public IEnumerable<string> ExcludeButtons { get; set; } = Array.Empty<string>();
        [Parameter] public IEnumerable<string> ExcludePlugins { get; set; } = Array.Empty<string>();
        [Parameter] public IEnumerable<string> ExcludeMenu { get; set; } = Array.Empty<string>();



        [Inject] private IOptions<TinyMCESettings> SettingsOptions { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var ops = SettingsOptions.Value;
            PluginNames = ops.DefaultPlugins.ToArray();
            ToolbarNames = ops.DefaultToolbar.ToArray();
            MenuName = ops.DefaultMenu.ToArray();
            AdditionalConfig = AddDefaultMenu;
        }

        private void AddDefaultMenu(IDictionary<string, object> obj)
        {
            var menu = new List<Menu>()
            {
                new Menu("File",
                    "newdocument restoredraft | preview | importword exportpdf exportword | print | deleteallconversations"),
                new Menu("Edit", "undo redo | cut copy paste pastetext | selectall | searchreplace"),
                new Menu("View",
                    "code revisionhistory | visualaid visualchars visualblocks | spellchecker | preview fullscreen | showcomments"),
                new Menu("Insert",
                    "image link media addcomment pageembed codesample inserttable | math | charmap emoticons hr | pagebreak nonbreaking anchor tableofcontents | insertdatetime"),
                new Menu("Format",
                    "bold italic underline strikethrough superscript subscript codeformat | styles blocks fontfamily fontsize align lineheight | forecolor backcolor | language | removeformat"),
                new Menu("Tools", "spellchecker spellcheckerlanguage | a11ycheck code wordcount"),
                new Menu("Table", "inserttable | cell row column | advtablesort | tableprops deletetable"),
                new Menu("Help", "help")
            };


            menu.RemoveAll(i => ExcludeMenu.Contains(i.Title));
            AddMenu(obj, menu.ToArray());
        }



        [Parameter]
        public override string? Name
        {
            get => string.Join(" ", PluginNames.Where(i => !ExcludePlugins.Contains(i)));
            set => _ = value;
        }

        [Parameter]
        public override string? Toolbar
        {
            get => string.Join(" ", ToolbarNames.Where(i => !ExcludeButtons.Contains(i)));
            set => _ = value;
        }


        [Parameter]
        public override string? Menubar
        {
            get => string.Join(" ", MenuName.Where(i => !ExcludeMenu.Contains(i)));
            set => _ = value;
        }

        [Parameter]
        public override string? ContextMenu
        {
            get => string.Join(" ", ToolbarNames.Where(i => !ExcludeButtons.Contains(i)));
            set => _ = value;
        }


 

    }

    public class Menu
    {
        public string Title { get; set; }

        public string Items => string.Join(" ", Buttons);

        public ICollection<string> Buttons { get;} = new List<string>();


        public Menu()
        {
            
        }

        public Menu(string title, IEnumerable<string> items)
        {
            Title = title;
            Buttons = items.ToList();
        }

        public Menu(string title, string items) : this(title, items.Split(" ").ToArray())
        {
        }
    }


    public static class Shortcut
    {
        public sealed class ShortcutBuilder
        {
            public List<string> Parts { get; } = new List<string>();


            public static implicit operator string(ShortcutBuilder shortcutBuilder)
            {
                return string.Join("+", shortcutBuilder.Parts);
            }

        }

        public static ShortcutBuilder Meta() => new ShortcutBuilder().Meta();
        public static ShortcutBuilder Meta(this ShortcutBuilder sb) => sb.And("Meta");

        public static ShortcutBuilder Shift() => new ShortcutBuilder().Shift();
        public static ShortcutBuilder Shift(this ShortcutBuilder sb) => sb.And("Shift");
        public static ShortcutBuilder Ctrl() => new ShortcutBuilder().Ctrl();
        public static ShortcutBuilder Ctrl(this ShortcutBuilder sb) => sb.And("Ctrl");
        public static ShortcutBuilder Alt() => new ShortcutBuilder().Alt();
        public static ShortcutBuilder Alt(this ShortcutBuilder sb) => sb.And("Alt");

        public static ShortcutBuilder Access() => new ShortcutBuilder().Access();
        public static ShortcutBuilder Access(this ShortcutBuilder sb) => sb.And("Access");
        
        public static ShortcutBuilder And(this ShortcutBuilder sb, string key)
        {
            sb.Parts.Add(key);
            return sb;
        }

    }

}
