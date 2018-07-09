using ProjectKairos.Models;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;
<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
=======
using System.Linq;
>>>>>>> origin/backend_admin_feature_nhan
using System.Web.Mvc;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Member")]
    public class UserController : Controller
    {
<<<<<<< HEAD
        private KAIROS_SHOPEntities db = new KAIROS_SHOPEntities();
=======
        private KAIROS_SHOPEntities db;
        private AccountService accountService;

        public UserController()
        {
            db = new KAIROS_SHOPEntities();
            accountService = new AccountService(db);
        }
>>>>>>> origin/backend_admin_feature_nhan

        // GET: User
        [HttpGet]
        [AuthorizeUser(Role = "Member")]
        [Route]
        public ActionResult Index()
        {
<<<<<<< HEAD
            return View("~/Views/index1.cshtml");
=======
            return View("~/Views/index.cshtml");
>>>>>>> origin/backend_admin_feature_nhan
        }

        [HttpGet]
        [Route("Manage/Account")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ManageAccount()
        {
<<<<<<< HEAD
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
=======
            string username = Session.GetCurrentUserInfo("Username");
            var viewModel = accountService.ViewMyAccount(username);
            return View("~/Views/User/user_account.cshtml", viewModel);
>>>>>>> origin/backend_admin_feature_nhan
        }

        [HttpGet]
        [Route("Checkout")]
<<<<<<< HEAD
        [AuthorizeUser(Role = "Member,Administrator")]
=======
        [AuthorizeUser(Role = "Member")]
>>>>>>> origin/backend_admin_feature_nhan
        public ActionResult CheckOut()
        {
            return View("~/Views/User/user_checkout.cshtml");
        }

        [HttpGet]
<<<<<<< HEAD
        [Route("MyOrder")]
=======
        [Route("Manage/Order")]
>>>>>>> origin/backend_admin_feature_nhan
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