using System.Web.Mvc;

namespace ProjectKairos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/index.cshtml");
        }

    }
}