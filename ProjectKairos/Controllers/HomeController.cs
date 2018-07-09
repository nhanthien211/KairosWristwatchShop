using System.Web.Mvc;
using ProjectKairos.Models;
using ProjectKairos.Utilities;

namespace ProjectKairos.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            if (Session["CURRENT_USER_ID"] != null)
            {
                string roleName = Session.GetCurrentUserInfo("RoleName");
                if (roleName == "Administrator")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View("~/Views/index.cshtml");
        }

    }
}