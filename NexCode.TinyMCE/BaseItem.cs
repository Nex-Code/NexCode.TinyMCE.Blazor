using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NexCode.TinyMCE.Blazor.Code;

namespace NexCode.TinyMCE.Blazor
{
    public interface ITinySpec
    {
        public string Type { get; }

    }

    public interface ITinyItem : ITinySpec
    {

        public string? Icon { get; }

        public string? Tooltip { get; }

        public string Text { get; }

        public string? Shortcut { get; }

        public bool Enabled { get; }

        public string Type { get; }
    }

    public abstract class ItemSpec : ITinySpec
    {
        public abstract string Type { get; }
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public abstract class BaseMenuItem : ComponentBase, ITinyItem
    {
        [Parameter]
        [JsonIgnore]
        public EventCallback<EditorEvent> OnSetup { get; init; }

        [Parameter]
        [JsonIgnore]
        public EventCallback<EditorEvent> OnTeardown { get; init; }

        [Parameter]
        [JsonIgnore]
        public EventCallback<EditorEvent> OnAction { get; set; }


        public bool HasSetup => OnSetup.HasDelegate;
        public bool HasTeardown => OnTeardown.HasDelegate;
        public bool HasAction => OnAction.HasDelegate;




        [Parameter]
        [JsonIgnore]
        public bool Show { get; set; } = true;

        [Parameter]
        [JsonIgnore]
        public MenuItemLocation Location { get; set; } = MenuItemLocation.All;

        [Parameter] public virtual string Name { get; set; } = CommonHelpers.Random(10);

        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public string? Tooltip { get; set; }

        [Parameter]
        public string? Text { get; set; }

        [Parameter]
        public string? Shortcut { get; set; }

        [Parameter] public bool Enabled { get; set; } = true;

        [JsonIgnore]
        public abstract string Type { get; }
        

        [CascadingParameter]
        [JsonIgnore]
        protected CustomPlugin? Parent { get; set; }

        [Inject] protected IEditor Editor { get; set; } = default!;


        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Parent != null)
            {
                Parent.Register(this);
            }
                
        }



        [JSInvokable]
        public async ValueTask<MenuApi> OnSetupCall(MenuApi item) => await CallEvent(item, OnSetup);

        [JSInvokable]
        public async ValueTask<MenuApi> OnTeardownCall(MenuApi item) => await CallEvent(item, OnTeardown);

        [JSInvokable]
        public async ValueTask<MenuApi> OnActionCall(MenuApi item) => await CallEvent(item, OnAction);

        private async ValueTask<MenuApi> CallEvent(MenuApi apiItem, EventCallback<EditorEvent> handler)
        {
            if (handler.HasDelegate)
                await handler.InvokeAsync(new EditorEvent(apiItem, Editor));
            return apiItem;
        }
    }

    public record EditorEvent(MenuApi MenuApi, IEditor? Editor);


    public class MenuApi
    {
        public bool Enabled { get; set; }
        /*public string Text { get; set; }
        public string Icon { get; set; }*/
        public bool Active { get; set; }

    }

    [Flags]
    public enum MenuItemLocation
    {
        Toolbar = 1,
        Menubar = 2,
        Contextmenu = 4,
        All = Toolbar | Menubar | Contextmenu,
    }
}
