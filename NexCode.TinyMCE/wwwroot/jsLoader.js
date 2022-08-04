export function loadJs(url, dotNethelper, method) {

    if (document.querySelectorAll("[src='" + url + "']").length != 0) {
        return;
    }

    var script = document.createElement("script");
    script.setAttribute("src", url);
    script.onload=function() {
        return dotNethelper.invokeMethodAsync(method);
    }

    document.body.appendChild(script);
    
}
