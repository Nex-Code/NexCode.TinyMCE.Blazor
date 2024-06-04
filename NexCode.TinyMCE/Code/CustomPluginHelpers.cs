using Microsoft.JSInterop;

namespace NexCode.TinyMCE.Blazor.Code;

public static class CustomPluginHelpers
{
    internal static string? RegistrationFunctionName(this ITinyItem item) => item switch
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
                i,
                DotNetObjectReference.Create(i)
            )
        ).ToArray();

        return items;
    }
}