using System;
using System.Web.Routing;

namespace ProjectKairos.Utilities
{
    public static class HtmlHelper
    {
        public static string IsSelected(this System.Web.Mvc.HtmlHelper html, string category)
        {
            RequestContext requestContext = html.ViewContext.RequestContext;
            RouteValueDictionary routeData = requestContext.RouteData.Values;
            string currentCategory = routeData["category"].ToString();
            if (currentCategory.Equals(category, StringComparison.OrdinalIgnoreCase))
            {
                return "active1";
            }
            return "";
        }
    }
}