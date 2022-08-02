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
        toolbar: toolbar,
        menubar: menubar,
        external_plugins: external_plugins
    });
}

export function setContent(id, content) {
    tinymce.get(id).setContent(content);
}

export function getContent(id) {
    return tinymce.get(id).getContent();
}


export function registerPlugin(id, buttons, menuItems) {

    tinymce.PluginManager.add(id,
        function(editor, url) {


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