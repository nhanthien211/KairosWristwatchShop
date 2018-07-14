using System.Web.Mvc;
using ProjectKairos.Models;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;
using System.Collections.Generic;

namespace ProjectKairos.Controllers
{
    public class HomeController : Controller
    {
        private KAIROS_SHOPEntities db;
        private WatchService watchService;
        private ShoppingCartService shoppingService;

        public HomeController()
        {
            db = new KAIROS_SHOPEntities();
            watchService = new WatchService(db);
            shoppingService = new ShoppingCartService(db);
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
                } else if (roleName == "Member")
                {
                    string username = Session.GetCurrentUserInfo("Username");
                    //Check if cart existed and not empty
                    bool checkExisted = shoppingService.CheckCartExistedInDB(username);
                    if (!checkExisted)
                    {
                        return View("~/Views/Home/shopping_cart.cshtml");
                    }
                }
            }

            //Not Login => check cart in Session
            var viewModel = new ShoppingCartViewModel((List<ShoppingItem>)Session["CART"]);

            Dictionary<int, int> IdAndRError = shoppingService.CheckCartSession();
            viewModel.IdAndError = IdAndRError;

            return View("~/Views/Home/shopping_cart.cshtml", viewModel);
        }

    }
}