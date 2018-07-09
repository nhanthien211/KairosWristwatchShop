using ProjectKairos.Models;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;
using System.Linq;
using System.Web.Mvc;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Member")]
    public class UserController : Controller
    {
        private KAIROS_SHOPEntities db;
        private AccountService accountService;

        public UserController()
        {
            db = new KAIROS_SHOPEntities();
            accountService = new AccountService(db);
        }

        // GET: User
        [HttpGet]
        [AuthorizeUser(Role = "Member")]
        [Route]
        public ActionResult Index()
        {
            return View("~/Views/index.cshtml");
        }

        [HttpGet]
        [Route("Manage/Account")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ManageAccount()
        {
            string username = Session.GetCurrentUserInfo("Username");
            var viewModel = accountService.ViewMyAccount(username);
            return View("~/Views/User/user_account.cshtml", viewModel);
        }

        [HttpGet]
        [Route("Checkout")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult CheckOut()
        {
            return View("~/Views/User/user_checkout.cshtml");
        }

        [HttpGet]
        [Route("Manage/Order")]
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