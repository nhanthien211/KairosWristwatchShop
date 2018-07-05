using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using ProjectKairos.Models;

namespace ProjectKairos.Utilities
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        private KAIROS_SHOPEntities db = new KAIROS_SHOPEntities();

        public string Role { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string username = (string)httpContext.Session["CURRENT_USER_ID"];

            Account account = db.Accounts.Include(a => a.Role).FirstOrDefault(a => a.Username == username);
            if (account == null)
            {
                return false;
            }

            string privilegeLevels = account.Role.RoleName;
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