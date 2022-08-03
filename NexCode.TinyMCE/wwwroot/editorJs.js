// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
    return prompt(message, 'Type anything here');
}


export function LoadJs(url, dotNethelper, method) {

    if (document.querySelectorAll("[src='" + url + "']").length != 0) {
        return;
    }

    var script = document.createElement("script");
    script.setAttribute("src", url);
    script.onload = function () {
        return dotNethelper.invokeMethodAsync(method);
    }

    document.body.appendChild(script);

}




export function init(id, plugins, menubar, toolbar, external_plugins) {
    tinymce.init({
        selector: "#" + id,
        plugins: plugins,
        toolbar: toolbar+" mybutton",
        menubar: menubar,
        external_plugins: external_plugins,
        setup:(editor) => {
            

        }
    });
}

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


export function registerPlugin(id, buttons, menuItems, toolbarButtons) {

    if (tinymce.PluginManager.lookup[id] != null) {
        tinymce.PluginManager.remove(id);
    }


    tinymce.PluginManager.add(id,
        function(editor, url) {

            toolbarButtons.forEach(function(b) {

                editor.ui.registry.addMenuButton(id, {
                    text: b.text,
                    fetch: (callback) => {
                        const items = b.items.map((item) => buildMenuItems(item));
                        callback(items);
                    }
                });
            })

            buttons.forEach(function(b)
            {
                editor.ui.registry.addButton(id, {
                    text: b.text,
                    onAction: function () {
                        return b.dotNethelper.invokeMethodAsync("TriggerAction");
                    }
                });
            });

            menuItems.forEach(function (b) {
                editor.ui.registry.addMenuItem(id, {
                    text: b.text,
                    onAction: function () {
                        return b.dotNethelper.invokeMethodAsync("TriggerAction");
                    }
                });
            });
        });
}


function buildMenuItems(menuItem) {

    if (menuItem.items != null) {

        return {
            text: menuItem.text,
            type: 'nestedmenuitem',
            getSubmenuItems: () => menuItem.items.map((item) => buildMenuItems(item))
        };

    }

    return {
        type: 'menuitem',
        text: menuItem.text,
        onAction: () => menuItem.dotNethelper.invokeMethodAsync("TriggerAction")
    }

}