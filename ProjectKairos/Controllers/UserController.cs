using ProjectKairos.Models;
using ProjectKairos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Member")]
    public class UserController : Controller
    {
        private KAIROS_SHOPEntities db = new KAIROS_SHOPEntities();

        // GET: User
        [HttpGet]
        [AuthorizeUser(Role = "Member")]
        [Route]
        public ActionResult Index()
        {
            return View("~/Views/index1.cshtml");
        }

        [HttpGet]
        [Route("Manage/Account")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ManageAccount()
        {
            return View("~/Views/User/user_account.cshtml");
        }

        [HttpGet]
        [Route("Checkout")]
        [AuthorizeUser(Role = "Member,Administrator")]
        public ActionResult CheckOut()
        {
            return View("~/Views/User/user_checkout.cshtml");
        }
    }
}