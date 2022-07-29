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

export function init(id, plugins, menubar, toolbar) {
    tinymce.init({
        selector: "#" + id,
        plugins: plugins,
        toolbar: toolbar,
        menubar: menubar
    });
}

export function setContent(id, content) {
    tinymce.get(id).setContent(content);
}

export function getContent(id) {
    return tinymce.get(id).getContent();
}