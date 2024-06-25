using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NexCode.TinyMCE.Blazor.Code
{
    public static class DependencyInjectionHelper
    {

        public static IServiceCollection AddTinyMCE(this IServiceCollection services)
        {
            services.AddTransient<JsLoaderFactory>();
            services.AddTransient<TinyEditor>();
            services.AddTransient<TinyEditorIntaliser>();

            return services;
        }

    }
}
