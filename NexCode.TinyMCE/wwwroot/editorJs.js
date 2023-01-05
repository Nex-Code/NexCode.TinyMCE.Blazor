// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.




export function init(id, plugins, menubar, toolbar, branding) {

    tinymce.init({
        selector: "#" + id,
        plugins: plugins,
        toolbar: toolbar,
        menubar: menubar,
        branding: branding
    });
}


export function removePlugin(id) {
    if (tinymce.PluginManager.lookup[id] != null) {
        tinymce.PluginManager.remove(id);
    }
}

export function registerPlugin(id, buttons) {
    removePlugin(id);
    tinymce.PluginManager.add(id,
        function(editor, url) {

            buttons.forEach(function(b) {

                var item = {
                    text: b.text,
                    icon: b.icon,
                    tooltip: b.tooltip,
                    enabled: b.enabled
                }

                if (b.hasSetup)
                    item.onSetup = () => b.dotNetHelper.invokeMethodAsync("OnSetup", editor.id);

                if (b.hasAction)
                    item.onAction = () => b.dotNetHelper.invokeMethodAsync("OnAction", editor.id);
                else
                    item.onAction = () => {};



                switch (b.funcName) {
                case "addToggleButton":
                case "addToggleMenuItem":
                    item.active = b.active;
                    break;
                case "addSplitButton":
                    if (b.hasItemAction)
                        item.onItemAction = (api, value) => b.dotNetHelper.invokeMethodAsync("OnItemAction", value);
                    else
                            item.onItemAction = () => { };

                case "addMenuButton":
                
                case "addNestedMenuItem":


                    const fetchFunc = (subItem) => {
                          if (subItem.items ) {
                              subItem.getSubmenuItems = () => subItem.items;
                              subItem.items.forEach(fetchFunc);
                        }

                        if (subItem.hasAction) {
                            subItem.onAction = function() {
                                subItem.dotNetHelper.invokeMethodAsync("OnAction", editor.id);
                            }
                        }

                    }
                    item.fetch = (callback) => {
                        b.dotNetHelper.invokeMethodAsync("Fetch").then((r) => {
                            var items = r.result;
                            items.forEach(fetchFunc);
                            callback(items);
                        })
                    };
                    break;

                    case "addGroupToolbarButton":
                        item.items = b.items;
                        break;
                };

                editor.ui.registry[b.funcName](b.id, item);
            });
            
        });
}


function GetEditorAndExecute(id, func) {

    if (tinymce == null)
        return null;

    var editor = tinymce.get(id);

    if (editor == null)
        return null;

    return func(editor);
}


export function getContent(id, args) {
    return GetEditorAndExecute(id,
        (editor) => {
            if (args == null)
                return editor.getContent();
            return editor.getContent(args);
        });
}

export function getParam(id, name, defaultValue, type) {
    return GetEditorAndExecute(id, (editor) => editor.getParam(name, defaultValue, type));
}

export function hasPlugin(id, name, loaded) {
    return GetEditorAndExecute(id, (editor) => editor.hasPlugin(name, loaded));
}

export function hide(id) {
    GetEditorAndExecute(id, (editor) => editor.hide());
}
export function load(id) {
    return GetEditorAndExecute(id, (editor) => editor.load());
}

export function remove(id) {
    GetEditorAndExecute(id, (editor) => editor.remove());
}
export function save(id) {

    if (!content)
        content = "";

    return GetEditorAndExecute(id, (editor) => editor.save());
}
export function setContent(id, content, args) {
    return GetEditorAndExecute(id, (editor) => editor.setContent(content, args));
}

export function setProgressState(id, state, time) {
    return GetEditorAndExecute(id, (editor) => editor.setProgressState(state, time));
}
export function show(id) {
    GetEditorAndExecute(id, (editor) => editor.show());
}

export function insertContent(id, content, args) {
    GetEditorAndExecute(id, (editor) => editor.insertContent(content, args));
}