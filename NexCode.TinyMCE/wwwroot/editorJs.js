// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.



export function setContent(id, content) {
    var editor = tinymce.get(id);

    if (editor == null)
        return;

    editor.setContent(content);
}

export function getContent(id) {
    var editor = tinymce.get(id);

    if (editor == null)
        return null;

    return editor.get(id).getContent();
}


export function init(id, plugins, menubar, toolbar, external_plugins) {
    tinymce.init({
        selector: "#" + id,
        plugins: plugins,
        toolbar: toolbar,
        
        menu: {
            custom: { title: 'Custom Menu', items: 'basicitem' }
        },
        menubar: 'file edit custom',
        external_plugins: external_plugins,
        setup:(editor) => {
            

        }
    });
}


export function registerPlugin(id, buttons) {

    if (tinymce.PluginManager.lookup[id] != null) {
        tinymce.PluginManager.remove(id);
    }


    tinymce.PluginManager.add(id,
        function(editor, url) {

            buttons.forEach(function(b) {

                var item = {
                    text: b.Text,
                    icon: b.Icon,
                    tooltip: b.Tooltip,
                    enabled: b.Enabled,
                }

                if (b.HasSetup)
                    item.onSetup = () => b.DotNetHelper.invokeMethodAsync("OnSetup");

                if (b.HasAction)
                    item.onAction = () => b.DotNetHelper.invokeMethodAsync("OnAction");
                else
                    item.onAction = () => {};



                switch (b.FuncName) {
                case "addToggleButton":
                case "addToggleMenuItem":
                    item.active = b.Active;
                    break;
                case "addSplitButton":
                    if (b.HasItemAction)
                        item.onItemAction = (api, value) => b.DotNetHelper.invokeMethodAsync("OnItemAction", value);
                    else
                            item.onItemAction = () => { };

                case "addMenuButton":
                
                case "addNestedMenuItem":
                    item.fetch = (callback) => {
                        b.DotNetHelper.invokeMethodAsync("Fetch").then((items) => {
                            callback(items.result);
                        })
                    };
                        break;

                    case "addGroupToolbarButton":
                        item.items = b.Items;
                        break;
                };

                editor.ui.registry[b.FuncName](b.Id, item);
            });
            
        });
}


export function destroy(id) {
    var editor = tinymce.get(id);

    if (editor == null)
        return null;

    return editor.destroy();
}