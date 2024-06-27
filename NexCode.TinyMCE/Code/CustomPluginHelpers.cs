using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor.Code;

public static class CustomPluginHelpers
{
    internal static string? RegistrationFunctionName(this object item) => item switch
    {
        MenuItem mi => "addMenuItem",
        ToggleMenuItem tmi => "addToggleMenuItem",
        //NestedMenuItem nmi => "addNestedMenuItem",



        Button tb => "addButton",
        ToggleButton ttb => "addToggleButton",
        MenuButton mtb => "addMenuButton",
        //GroupToolbarbutton gtb => "addGroupToolbarButton",
        //SplitToolbarButton stb => "addSplitButton",


        ContextMenu ct => "addContextMenu",
        ContextToolbar ct => "addContextToolbar",
        //ContextForm cf => "addContextForm",

        //"addIcon",
        //"addAutocompleter",
        //"addSidebar",
        //"addView",

        _ => null
    };

    internal static IEnumerable<RegisterableItem> GetRegisterableItems(this CustomPlugin plugin)
    {
        var items = plugin.Items
            .Where(i=>i.RegistrationFunctionName().IsNotNullOrWhiteSpace())
            .Select(i => new
            RegisterableItem
            (
                i.RegistrationFunctionName()!,
                i
            )
        ).ToArray();

        return items;
    }

}



public sealed class TinyEventFactory
{

    private object Parent { get; }

    internal TinyEventFactory(object parent)
    {
        Parent = parent;
    }


    public EventCallback<TValue> Create<TValue>(Action callback) => EventCallback.Factory.Create<TValue>(Parent, callback);
    public EventCallback<TValue> Create<TValue>(Action<TValue> callback) => EventCallback.Factory.Create<TValue>(Parent, callback);
    public EventCallback<TValue> Create<TValue>(Func<TValue, Task> callback) => EventCallback.Factory.Create<TValue>(Parent, callback);
    public EventCallback Create(Action callback) => EventCallback.Factory.Create(Parent, callback);
    public EventCallback Create(Func<Task> callback) => EventCallback.Factory.Create(Parent, callback);


}