// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.




//export function init(id, plugins, menubar, toolbar, branding) {

//    tinymce.init({
//        selector: "#" + id,
//        plugins: plugins,
//        toolbar: toolbar,
//        menubar: menubar,
//        branding: branding
//    });
//}

export function init(options) {
    tinymce.init(options);
}


export function removePlugin(id) {
    if (tinymce.PluginManager.lookup[id] != null) {
        tinymce.PluginManager.remove(id);
    }
}

export function registerPlugin(id, items) {
    removePlugin(id);
    tinymce.PluginManager.add(id,
        function(editor, url) {
            items.forEach(function(item) {
                addItem(editor, item.func, item.details, item.dotnetHelper);
            });
        });
}

export function addItem(editor, funcName, details, dotnetHelper) {


    const CallMenuApiFunc = (api, func) => {
        dotnetHelper.invokeMethodAsync(func, GetMenuApiItem(api)).then(r => SetMenuApi(api, r.result))
    }

    const GetMenuApiItem = (api) => {

        var obj = {
            enabled: api.isEnabled(),
            active: true
        };

        if (api.isActive != undefined) {
            obj.active = api.isActive();
        }

        return obj
    }

    const SetMenuApi = (api, r) => {
        api.setEnabled(r.enabled);
        if (api.setActive != undefined)
            api.setActive(r.active);
    }

    switch (funcName) {
        case "addButton":
        case "addToggleButton":
        case "addMenuButton":
        case "addMenuButton":
        case "addGroupToolbarButton":
        case "addSplitButton":
            {
                details.onSetup = (api) => {
                    CallMenuApiFunc(api, "OnSetupCall");
                    return (api) => CallMenuApiFunc(api, "OnTeardownCall");
                }
                details.onAction = (api) => CallMenuApiFunc(api, "OnActionCall");
            }
    };

    editor.ui.registry[funcName](details.name, details);
}



export function addItemToEditor(editorId, funcName, details, dotnetHelper) {
    GetEditorAndExecute(id, (editor) => addItem(editor, funcName, details, dotnetHelper));
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