using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace NexCode.TinyMCE.Blazor.Code
{
    public static class RenderTreeBuilderExtensions
    {

        public static void AddCascadingValue<TValue>(this RenderTreeBuilder builder, TValue value, RenderFragment childContent)
        {
            builder.AddCascadingValue(value, true, childContent);
        }



        public static void AddCascadingValue<TValue>(this RenderTreeBuilder builder, TValue value, bool isFixed, RenderFragment childContent)
        {
            builder.OpenComponent<CascadingValue<TValue>>(0);
            builder.AddAttribute(1, "Value", value);
            builder.AddAttribute(2, "IsFixed", isFixed);
            builder.AddAttribute(3, "ChildContent", childContent);
            builder.CloseComponent();
        }

    }
}
