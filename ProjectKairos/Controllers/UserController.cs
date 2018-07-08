using ProjectKairos.Models;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;
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
            string username = (string)Session["CURRENT_USER_ID"];
            var account = db.Accounts.Where(a => a.Username == username)
                .Select(a => new AccountInfoViewModel
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    Phone = a.Phone,
                    DOB = a.DOB,
                    StartedDate = a.StartDate,
                    Gender = a.Gender
                }).First();
            return View("~/Views/User/user_account.cshtml", account);
        }

        [HttpGet]
        [Route("Checkout")]
        [AuthorizeUser(Role = "Member,Administrator")]
        public ActionResult CheckOut()
        {
            return View("~/Views/User/user_checkout.cshtml");
        }

        [HttpGet]
        [Route("MyOrder")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ManageOrder()
        {
            return View("~/Views/User/user_order.cshtml");
        }

        [HttpGet]
        [Route("OrderDetail")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ViewOrderDetail()
        {
            return View("~/Views/User/user_order_detail.cshtml");
        }
    }
}