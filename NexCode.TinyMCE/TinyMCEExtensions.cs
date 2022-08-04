using System.ComponentModel.DataAnnotations;
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

            services.AddSingleton((b) => new RichTextDefaultEditorOptions()
            {
                Plugins = Defaults.Plugins,
                Toolbar = Defaults.Toolbar,
            });


            var servies = services.Select(i => i.ServiceType.FullName).ToArray();
            var servies2 = services.Select(i => i.ImplementationInstance?.GetType().FullName??"missing").ToArray();
            var s = string.Join(Environment.NewLine, servies);
            var s2 = string.Join(Environment.NewLine, servies2);
            Console.WriteLine(s);


            if (services.All(i => i.ServiceType != typeof(RichTextDefaultEditorOptions)))
                services.AddScoped<RichTextDefaultEditorOptions>();

            return services;
        }


    }

    public class Defaults
    {
        public const string Plugins =
            "advlist autolink link image lists charmap print preview hr anchor pagebreak searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking table emoticons template paste help";

        public const string Toolbar =
            "undo redo | styles | bold italic | alignleft aligncenter alignright alignjustify | outdent indent";
    }
}
