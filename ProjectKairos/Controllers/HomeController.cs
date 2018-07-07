using System.Web.Mvc;

namespace ProjectKairos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/index1.cshtml");
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

    }
}