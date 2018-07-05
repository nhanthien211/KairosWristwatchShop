using System.Web.Mvc;

namespace ProjectKairos.Utilities
{
    public static class HtmlMenuHelper
    {
        public static string IsLinkActive(this HtmlHelper html, string action, string controller)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            // both must match
            if (controller == routeControl &&
                action == routeAction)
            {
                return "active";
            }
            return "";
        }
    }
}