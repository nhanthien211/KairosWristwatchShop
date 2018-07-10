using System.Web.Mvc;
using ProjectKairos.Models;
using ProjectKairos.Utilities;

namespace ProjectKairos.Controllers
{
    public class HomeController : Controller
    {
        private KAIROS_SHOPEntities db;
        private WatchService watchService;

        public HomeController()
        {
            db = new KAIROS_SHOPEntities();
            watchService = new WatchService(db);
        }

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

            var listProductModel = watchService.LoadWatchListIndex();
            ViewBag.listProduct = listProductModel;
            return View("~/Views/Home/index.cshtml");
        }

        [HttpGet]
        [Route("Product")]
        public ActionResult ViewProduct()
        {
            if (Session["CURRENT_USER_ID"] != null)
            {
                string roleName = Session.GetCurrentUserInfo("RoleName");
                if (roleName == "Administrator")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View("~/Views/Home/shopping_product.cshtml");
        }

        [HttpGet]
        [Route("ProductDetail/{ProductID}")]
        public ActionResult ViewProductDetail()
        {
            if (Session["CURRENT_USER_ID"] != null)
            {
                string roleName = Session.GetCurrentUserInfo("RoleName");
                if (roleName == "Administrator")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View("~/Views/Home/shopping_detail.cshtml");
        }

        [HttpGet]
        [Route("Cart")]
        public ActionResult ManageCart()
        {
            if (Session["CURRENT_USER_ID"] != null)
            {
                string roleName = Session.GetCurrentUserInfo("RoleName");
                if (roleName == "Administrator")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View("~/Views/Home/shopping_cart.cshtml");
        }

    }
}