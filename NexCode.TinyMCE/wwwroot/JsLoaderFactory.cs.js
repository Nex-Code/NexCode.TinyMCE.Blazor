export function importScript(url, dotNethelper, onSuccessCallback, onErrorCallback) {

    if (document.querySelectorAll("[src='" + url + "']").length != 0) {
        return false;
    }

    var script = document.createElement("script");
    script.setAttribute("src", url);

    if (onSuccessCallback != undefined)
        script.onload = (e)=>dotNethelper.invokeMethodAsync(onSuccessCallback);
    if (onErrorCallback != undefined)
        script.onerror = (e) => dotNethelper.invokeMethodAsync(onErrorCallback, e);

    document.body.appendChild(script);

    return true;
    
}
