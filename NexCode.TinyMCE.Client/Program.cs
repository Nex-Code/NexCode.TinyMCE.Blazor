using NexCode.TinyMCEEditor;
using NexCode.TinyMCEEditor.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NexCode.TinyMCE.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTinyMCE();

builder.Services.AddSingleton(() => new RichTextDefaultEditorOptions()
{
    Plugins = "advlist autolink link image lists charmap print preview hr anchor pagebreak searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking table emoticons template paste help",
    Toolbar = "undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | outdent indent | code"
});



await builder.Build().RunAsync();

