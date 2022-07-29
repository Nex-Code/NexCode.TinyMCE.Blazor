using NexCode.TinyMCE.Blazor;
using Microsoft.Extensions.DependencyInjection;

namespace NexCode.TinyMCEEditor
{
    public static  class TinyMCEExtensions
    {

        public static IServiceCollection AddTinyMCE(this IServiceCollection services)
        {
            services.AddScoped<EditorJs>();
            services.AddTransient<JsLoader>();

            return services;
        }


    }
}
