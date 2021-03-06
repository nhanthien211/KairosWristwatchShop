﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectKairos.Utilities
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public string Role { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string privilegeLevels = httpContext.Session.GetCurrentUserInfo("RoleName");
            if (privilegeLevels == null && Role.Contains("Guest"))
            {
                return true;
            }
            string[] allRole = Role.Split(',');
            foreach (var role in allRole)
            {
                if (role.Trim() == privilegeLevels)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        //change to 404 or default page
                        controller = "Home",
                        action = "Index"
                    })
            );
        }
    }
}