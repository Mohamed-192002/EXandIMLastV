using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EXandIM.Web.Helpers
{
    [HtmlTargetElement("a", Attributes = "active-when-action")]
    public class ActiveTagWithAction : TagHelper
    {
        public string? ActiveWhenAction { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(ActiveWhenAction))
                return;

            var currentController = ViewContextData?.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            var currentaction = ViewContextData?.RouteData.Values["action"]?.ToString() ?? string.Empty;
            var current = $"{currentController}-{currentaction}";
            if (current!.Equals(ActiveWhenAction))
            {
                if (output.Attributes.ContainsName("class"))
                    output.Attributes.SetAttribute("class", $"{output.Attributes["class"].Value} active");
                else
                    output.Attributes.SetAttribute("class", "active");
            }
        }
    }
}