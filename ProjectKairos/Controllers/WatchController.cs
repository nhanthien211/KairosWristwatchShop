using System.Web.Mvc;
using ProjectKairos.Models;
using ProjectKairos.Utilities;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Watch")]
    public class WatchController : Controller
    {
        private WatchModelService watchModelService;
        private WatchService watchService;
        private KAIROS_SHOPEntities db;

        public WatchController()
        {
            db = new KAIROS_SHOPEntities();
            watchModelService = new WatchModelService(db);
            watchService = new WatchService(db);
        }

        [HttpGet]
        [AuthorizeUser(Role = "Guest, Member")]
        [Route("{category?}")]
        public ActionResult ViewProduct(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return View("~/Views/Watch/watch.cshtml");
            }

            return Content(category);
        }

        [HttpGet]
        [AuthorizeUser(Role = "Guest, Member")]
        [Route("{category}/{watchCode}")]
        public ActionResult ViewProductDetail(string category, string watchCode)
        {
            if (watchModelService.IsModelExisted(category) && watchService.IsAvailableWatch(watchCode))
            {
                //valid link. Deny any invalid manual input link.
                var viewModel = watchService.GetWatchFullDetail(watchCode);
                return View("~/Views/Watch/watch_detail.cshtml", viewModel);
            }
            return RedirectToAction("NotFound", "Home");
        }




    }
}