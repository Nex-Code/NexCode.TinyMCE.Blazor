using NexCode.TinyMCEEditor;
using NexCode.TinyMCEEditor.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NexCode.TinyMCE.Blazor;
using NexCode.TinyMCE.Blazor.Code;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTinyMCE(settings =>
{
    settings.DefaultPlugins.Add("link");
    settings.DefaultToolbar.Add("link");
});





await builder.Build().RunAsync();

