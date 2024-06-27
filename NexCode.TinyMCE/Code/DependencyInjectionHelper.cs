using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NexCode.TinyMCE.Blazor.Code
{
    public static class DependencyInjectionHelper
    {



        public static IServiceCollection AddTinyMCE(this IServiceCollection services) => services.AddTinyMCE(config =>
        {

        });

        public static IServiceCollection AddTinyMCE(this IServiceCollection services, Action<TinyMCESettings> config)
        {
            services.AddTransient<JsLoaderFactory>();
            services.AddTransient<IEditor, TinyEditor>();
            services.AddTransient<IJsEditor, TinyEditor>();
            services.AddTransient<TinyEditor>();
            services.AddTransient<TinyEditorIntaliser>();

            services.Configure(config);

            return services;
        }
        




    }


    public class TinyMCESettings
    {
        
        public bool ShowPromotion { get; set; }

        /**
         * <summary>
         *  Ensure you are following the branding rules
         * <see href="https://www.tiny.cloud/docs/tinymce/6/statusbar-configuration-options/"> shown here</see>
         * </summary>
         */
        public bool ShowBranding { get; set; } = true;

        public string? LicenseKey { get; set; }

        public IList<string> DefaultPlugins { get; set; } = PluginsString.Split(" ").ToList();

        public IList<string> DefaultToolbar { get; set; } = ToolbarString.Split(" ").ToList();
        public IList<string> DefaultMenu { get; set; } = MenuString.Split(" ").ToList();



        private const string PluginsString =
            "advlist autolink link image lists charmap print preview hr anchor pagebreak searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking table emoticons template paste help";

        private const string ToolbarString = "undo redo | styles | bold italic | alignleft aligncenter alignright alignjustify | outdent indent | code";
        private const string MenuString = "File Edit View Insert Format Tools Table Help";
    }

}
