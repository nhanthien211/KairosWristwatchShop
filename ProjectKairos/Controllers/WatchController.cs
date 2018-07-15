using System;
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
        [Route("Category/{category}/{pageNumber?}")]
        public ActionResult ViewProduct(string category, int? pageNumber)
        {
            if (watchModelService.IsModelExisted(category) || category.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                int selectedPage = pageNumber ?? 1;
                int pageSize = 6;
                var viewModel = watchService.GetWatchListBasedOnCategory(category, selectedPage, pageSize);
                return View("~/Views/Watch/watch.cshtml", viewModel);
            }
            return RedirectToAction("NotFound", "Home");

        }

        [HttpGet]
        [AuthorizeUser(Role = "Guest, Member")]
        [Route("{watchCode}")]
        public ActionResult ViewProductDetail(string watchCode)
        {
            if (watchService.IsAvailableWatch(watchCode))
            {
                //valid link. Deny any invalid manual input link.
                var viewModel = watchService.GetWatchFullDetail(watchCode);
                return View("~/Views/Watch/watch_detail.cshtml", viewModel);
            }
            return RedirectToAction("NotFound", "Home");
        }

    }
}