using System.Web.Mvc;
using ProjectKairos.Models;
using ProjectKairos.Utilities;

namespace ProjectKairos.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
<<<<<<< HEAD
            return View("~/Views/index1.cshtml");
=======
            if (Session["CURRENT_USER_ID"] != null)
            {
                string roleName = Session.GetCurrentUserInfo("RoleName");
                if (roleName == "Administrator")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View("~/Views/index.cshtml");
>>>>>>> origin/backend_admin_feature_nhan
        }

        [HttpGet]
        [Route("Product")]
        public ActionResult ViewProduct()
        {
            return View("~/Views/shopping_product.cshtml");
        }

        [HttpGet]
        [Route("Detail")]
        public ActionResult ViewProductDetail()
        {
            return View("~/Views/shopping_detail.cshtml");
        }

        [HttpGet]
        [Route("Cart")]
        public ActionResult ManageCart()
        {
            return View("~/Views/shopping_cart.cshtml");
        }

        [HttpGet]
        [Route("NotFound")]
        public ActionResult NotFound()
        {
            return View("~/Views/notfound.cshtml");
        }

        

    }
}