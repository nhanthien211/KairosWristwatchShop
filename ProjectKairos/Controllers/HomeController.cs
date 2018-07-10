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

            return View("~/Views/Home/index.cshtml", listProductModel);
        }

        public ActionResult NotFound()
        {
            return View("~/Views/Home/404_page.cshtml");
        }

        [HttpGet]
        [Route("Product")]
        public ActionResult ViewProduct()
        {
            return View("~/Views/Home/shopping_product.cshtml");
        }

        [HttpGet]
        [Route("Detail")]
        public ActionResult ViewProductDetail()
        {
            return View("~/Views/Home/shopping_detail.cshtml");
        }

        [HttpGet]
        [Route("Cart")]
        public ActionResult ManageCart()
        {
            return View("~/Views/Home/shopping_cart.cshtml");
        }

    }
}