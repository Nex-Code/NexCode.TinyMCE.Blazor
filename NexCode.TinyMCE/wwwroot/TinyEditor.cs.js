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

    Object.getOwnPropertyNames(options).forEach((el) => {
        if (!el.endsWith("regexp"))
            return;

        var rg = options[el];
        var flags = "";
        if (rg[0] == "/" && rg.lastIndexOf("/") != -1) {
            flags = rg.substring(rg.lastIndexOf("/") + 1)
            rg = rg.substring(1, rg.lastIndexOf("/"))
        }

        options[el] = new RegExp(rg,flags)
    });


    options.setup = (editor) => {
        editor.on("change", e => options.DotNetHelper.invokeMethodAsync(options.onchange));
        editor.on("blue", e => options.DotNetHelper.invokeMethodAsync(options.onblur));
    };


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
                addItem(editor, item.func, item.details);
            });
        });
}

export function addItem(editor, funcName, details) {


    

    //const CallMenuApiFunc = async (api, func, dotnetHelper, callback) => {
    //    let mApi = GetMenuApiItem(api);
    //    let r = await dotnetHelper.invokeMethodAsync(func, mApi);
    //    var results = r.result;
    //    if (callback != undefined)
    //        callback(results)
    //    SetMenuApi(api, results.api)
    //};
    

    const GetMenuApiItem = (api) => {

        let obj = {
            enabled: api.isEnabled(),
            active: true
        };

        if (api.isActive != undefined) {
            obj.active = api.isActive();
        }

        if (api.getText != undefined) {
            obj.text = api.getText();
        }
        if (api.getIcon != undefined) {
            obj.icon = api.getIcon();
        }

        return obj
    }

    const SetMenuApi = (api, r) => {

        if (!r)
            return;

        api.setEnabled(r.enabled);

        if (api.setActive != undefined)
            api.setActive(r.active);

        if (api.setText != undefined)
            api.setText(r.text);

        if (api.setIcon != undefined)
            api.setIcon(r.icon);

    }


    const cleanObject = (item) => {
        if (Array.isArray(item)) {
            item.forEach(el => cleanObject(el))
        } else {
            Object.getOwnPropertyNames(item).forEach((el) => {
                var v = item[el];

                if (v && typeof (v) === 'object') {
                    cleanObject(v);
                    return;
                }

                if (v === null) {
                    delete item[el];
                }
            });
        }

        wireInEvents(item);

        return item;
    }


    const CallMenuApiFunc = (helper, func, api) =>
        helper.invokeMethodAsync(func, GetMenuApiItem(api)).then(r => SetMenuApi(api, r.result))

    const wireInEvents = (item) => {

        if (!item.helper && item.dotnetHelper) {
            item.helper = item.dotnetHelper;
        }


        if (item.hasSetup) {
            item.onSetup = async (api) => {
                await CallMenuApiFunc(item.helper, "OnSetupCall", api);

                if (item.hasTeardown) {
                    return (api) => CallMenuApiFunc(item.helper, "OnTeardownCall", api);
                }
            }
        } else {
            item.onSetup = () => { };
        }

        if (item.hasAction) {
            item.onAction = (api) => CallMenuApiFunc(item.helper, "OnActionCall", api);
        } else {
            item.onAction = () => { };
        }

        if (item.hasFetch) {
            item.fetch = (callback, context) => item.helper.invokeMethodAsync("OnFetchCall").then(r => callback(cleanObject(r.result)));
        }
    }



    switch (funcName) {
        case "addMenuButton":
        case "addToggleButton":
        case "addButton":
        case "addGroupToolbarButton":
        case "addSplitButton":
            wireInEvents(details);
    };

    editor.ui.registry[funcName](details.name, details);
}

window.convertElementToObject = (el) => {

    if (!el)
        return null;

    var item = {
        nodeName: el.nodeName,
        nodeValue: el.nodeValue,
        content: el.textContent,
        children: null,
        attributes: null
    }

    if (el.children && el.children.length > 0) {

        item.children = [];

        for (let i = 0; i < el.children.length; i++) {
            let cEl = el.children[i];
            let n = window.convertElementToObject(cEl);
            if(n)
                item.children.push(n);
        }
    }

    if (el.attributes && el.attributes.length > 0) {

        item.attributes = {}
;
        for (let i = 0; i < el.attributes.length; i++) {
            let attr = el.attributes[i];

            if (!attr.name.startsWith('data-mce'))
                item.attributes[attr.name] = attr.value;
        }

        if (item.attributes.length == 0)
            item.attributes = null;

    }



    return item;
}