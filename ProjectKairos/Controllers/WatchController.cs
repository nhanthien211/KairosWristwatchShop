using System.Web.Mvc;
using ProjectKairos.Utilities;

namespace ProjectKairos.Controllers
{
    public class WatchController : Controller
    {

        [HttpGet]
        [AuthorizeUser(Role = "Guest, Member")]
        [Route("Product")]
        public ActionResult ViewProduct()
        {
            return View("~/Views/Watch/watch.cshtml");
        }

        [HttpGet]
        [AuthorizeUser(Role = "Guest, Member")]
        [Route("Detail")]
        public ActionResult ViewProductDetail(string watchId)
        {

            return View("~/Views/Watch/watch_detail.cshtml");
        }


    }
}