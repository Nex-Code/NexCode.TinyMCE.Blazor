using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NexCode.TinyMCE.Blazor.Code;
using static NexCode.TinyMCE.Blazor.Button;
using static NexCode.TinyMCE.Blazor.ToggleButton;

namespace NexCode.TinyMCE.Blazor
{
    public class Button : BaseMenuItem
    {
        public override string Type => "button";
    }

    public class ToggleButton : BaseMenuItem
    {
        public override string Type => "togglebutton";
    }

    public class MenuButton : BaseMenuItem
    {
        [JsonIgnore, Parameter, EditorRequired]
        public GetItems Fetch { get; init; } = (_) => ValueTask.FromResult(Array.Empty<BaseMenuItem>().AsEnumerable());

        public bool HasFetch => true;

        public override string Type => "menubutton";


        [JSInvokable]
        public async ValueTask<IEnumerable<BaseMenuItem>> OnFetchCall()
        {
            var eventFactory = new TinyEventFactory(this);
            var items = await Fetch(eventFactory);

            items = items.Select(i =>
            {
                i.SetEditor(Editor);
                return i;
            });

            return items;
        }

    }

    public class Seperator : BaseMenuItem
    {
        public override string Type => "button";
        public override string Name { 
            get => "|" ; 
            set => _ = value; }
    }

    public record MenuButtonResult : EventResult<MenuApi>
    {
        public IEnumerable<BaseMenuItem> Items { get; set; }
    };

    public class MenuButtonApi : MenuApi
    {
        public string Text { get; set; }
        public string Icon { get; set; }
    }



    public delegate ValueTask<IEnumerable<BaseMenuItem>> GetItems(TinyEventFactory eventCallbackFactory);


}
