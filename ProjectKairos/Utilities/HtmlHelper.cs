using System;
using System.Web.Mvc;
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

        public static string CreatePagingList(this System.Web.Mvc.HtmlHelper html, int totalPage,
            int currentPage, int maxDisplayPage)
        {
            if (totalPage <= maxDisplayPage)
            {
                for (int i = 1; i <= totalPage; i++)
                {
                    TagBuilder tb = new TagBuilder("a");
                    tb.MergeAttribute("href", "#");
                    tb.AddCssClass("");
                }
            }
            else
            {

            }


            //string result = "";
            //for (var i = 1; i <= totalPage; i++)
            //{
            //    string status = "";
            //    if (i == currentPage)
            //    {
            //        status = "active";
            //    }
            //    result += @"<a href = '#' class='item-pagination flex-c-m trans-0-4 " + status + "-pagination'>" + i + "</a>";
            //}

            return "";
        }
    }
}