using System;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ImageProcessor.Imaging;
using ProjectKairos.Models;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Admin")]
    public class AdminController : Controller
    {

        private KAIROS_SHOPEntities db;
        private WatchService watchService;
        private ModificationService modificationService;
        private WatchModelService watchModelService;
        private MovementService movementService;
        private AccountService accountService;
        private RoleService roleService;

        public AdminController()
        {
            db = new KAIROS_SHOPEntities();
            watchService = new WatchService(db);
            modificationService = new ModificationService(db);
            watchModelService = new WatchModelService(db);
            movementService = new MovementService(db);
            accountService = new AccountService(db);
            roleService = new RoleService(db);
        }

        // GET: Admin
        [HttpGet]
        [AuthorizeUser(Role = "Administrator")]
        [Route]
        public ActionResult Index()
        {
            string username = Session.GetCurrentUserInfo("Username");
            AccountInfoViewModel viewModel = accountService.ViewMyAccount(username);
            return View("~/Views/Admin/admin_info.cshtml", viewModel);
        }

        [HttpGet]
        [Route("Manage/Account")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult ManageAccount()
        {
            return View("~/Views/Admin/admin_manage_user.cshtml");
        }

        [HttpGet]
        [Route("Manage/Order")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult ManageOrder()
        {
            return View("~/Views/Admin/admin_manage_order.cshtml");
        }

        [HttpGet]
        [Route("Manage/Watch")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult ManageWatch()
        {
            return View("~/Views/Admin/admin_manage_watch.cshtml");
        }

        [HttpPost]
        [AuthorizeUser(Role = "Administrator")]
        [Route("LoadAccount")]
        public ActionResult LoadAccount()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Request.Form.GetValues("order[0][column]").FirstOrDefault(): get column index that used for sorting
            //columns[index][name]:get column name to used in orderBy LINQ statement            
            string sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            //Paging Size (5, 10,20,50,100)  
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var data = accountService.LoadAccountTable(sortColumnDirection, searchValue, ref recordsTotal, pageSize, skip);
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Route("Manage/Account/View/{username}")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult ViewAccount(string username)
        {
            var account = accountService.ViewAccountInfo(username);
            if (account == null)
            {
                return HttpNotFound();
            }
            var role = roleService.GetListRole();
            account.Role = role;
            return View("~/Views/Admin/admin_manage_user_detail.cshtml", account);
        }

        [HttpPost]
        [Route("Manage/Account/Edit/{username}")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult EditAccount(string username)
        {
            bool isActive = Request["isActive"] == "active";
            int roleId = Convert.ToInt32(Request["selectRole"]);

            if (accountService.AdminUpdateAccount(username, isActive, roleId))
            {
                if (username == Session.GetCurrentUserInfo("Username"))
                {
                    if (isActive == false || roleId != Convert.ToInt32(Session.GetCurrentUserInfo("RoleId")))
                    {
                        Session.RemoveAll();
                        return RedirectToAction("Index", "Home");
                    }

                }
                TempData["SHOW_MODAL"] = @"<script>$('#successModal').modal();</script>";
                return RedirectToAction("ViewAccount", "Admin");
            }

            return HttpNotFound(); //change to 404/ server busy
        }

        [HttpGet]
        [Route("Manage/Watch/Add")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult AddWatch()
        {
            //load movement and model to view 
            var movement = db.Movements.ToList();
            var watchModel = db.WatchModels.ToList();
            var viewModel = new AddWatchViewModel(movement, watchModel);
            return View("~/Views/Admin/admin_manage_watch_add.cshtml", viewModel);
        }

        [HttpPost]
        [Route("Manage/Watch/Add")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult AddWatch([Bind(Include = "WatchDescription, WatchCode, Quantity, Price, MovementID, ModelId, BandMaterial, CaseRadius, CaseMaterial, Discount, Guarantee")] Watch watch, HttpPostedFileBase thumbnail)
        {
            if (ModelState.IsValid)
            {
                watch.WaterResistant = Request["water"] == "yes";
                watch.LEDLight = Request["led"] == "yes";
                watch.Alarm = Request["alarm"] == "yes";
                //check if watch code is unique
                if (watchService.IsDuplicatedWatchCode(watch.WatchCode))
                {
                    //code đã tồn tại
                    //thông báo điền lại code 
                    //prefill các field
                    var movement = movementService.GetMovementList();
                    var watchModel = watchModelService.GetModelsList();
                    var viewModel = new AddWatchViewModel
                    {
                        Movement = movement,
                        WatchModel = watchModel,
                        WatchName = watch.WatchCode,
                        WatchDescription = watch.WatchDescription,
                        ModelId = watch.ModelID,
                        MovementId = watch.MovementID,
                        BandMaterial = watch.BandMaterial,
                        CaseRadius = watch.CaseRadius.GetValueOrDefault(),
                        CaseMaterial = watch.CaseMaterial,
                        Discount = watch.Discount,
                        Quantity = watch.Quantity,
                        Price = watch.Price,
                        Guarantee = watch.Guarantee,
                        Alarm = watch.Alarm,
                        LedLight = watch.LEDLight,
                        WaterResistant = watch.WaterResistant,
                        DuplicateErrorMessage = "Watch with code '" + watch.WatchCode + "' already existed"
                    };
                    return View("~/Views/Admin/admin_manage_watch_add.cshtml", viewModel);
                }
                //lưu hình ảnh xuống máy  
                string path = HostingEnvironment.MapPath("~/Content/img/ProductThumbnail/") + watch.WatchCode + DateTime.Now.ToBinary();
                ImageProcessHelper.ResizedImage(thumbnail.InputStream, 360, 750, ResizeMode.Max, ref path);
                watch.Thumbnail = path;
                watch.PublishedTime = DateTime.Now;
                watch.PublishedBy = Session.GetCurrentUserInfo("Username");
                watch.Status = true;
                if (watchService.AddNewWatch(watch))
                {
                    TempData["SHOW_MODAL"] = @"<script>$('#successModal').modal();</script>";
                    return RedirectToAction("AddWatch", "Admin");
                }
                return HttpNotFound();
            }
            return HttpNotFound();
        }

        [HttpPost]
        [AuthorizeUser(Role = "Administrator")]
        [Route("LoadWatch")]
        public ActionResult LoadWatch()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Request.Form.GetValues("order[0][column]").FirstOrDefault(): get column index that used for sorting
            //columns[index][name]:get column name to used in orderBy LINQ statement            
            string sortColumnDirection = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            //Paging Size (5, 10,20,50,100)  
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            // Getting all Account data  
            //Paging   
            var data = watchService.LoadWatchList(sortColumnDirection, searchValue, ref recordsTotal, skip, pageSize);
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Route("Manage/Watch/View/{watchId}")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult ViewWatch(string watchId)
        {
            int id;
            try
            {
                id = Convert.ToInt32(watchId);
            }
            catch (FormatException e)
            {
                return HttpNotFound();
            }

            var watchDetail = watchService.ViewWatchDetail(id);
            if (watchDetail == null)
            {
                return HttpNotFound();
            }
            watchDetail.Movement = movementService.GetMovementList();
            watchDetail.WatchModel = watchModelService.GetModelsList();
            watchDetail.Thumbnail = MyCustomUtility.RelativeFromAbsolutePath(watchDetail.Thumbnail);
            return View("~/Views/Admin/admin_manage_watch_detail.cshtml", watchDetail);
        }

        [HttpPost]
        [Route("Manage/Watch/Edit/{watchId}")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult EditWatch([Bind(Include = "WatchId, WatchCode, WatchDescription, Quantity, Price, " +
                                                      "MovementID, ModelID, BandMaterial, CaseMaterial, " +
                                                      "CaseRadius, Discount, Guarantee, PublishedBy, PublishedTime")] Watch watch, HttpPostedFileBase thumbnail)
        {

            if (ModelState.IsValid)
            {
                watch.WaterResistant = Request["water"] == "yes";
                watch.LEDLight = Request["led"] == "yes";
                watch.Alarm = Request["alarm"] == "yes";
                watch.Status = Request["status"] == "yes";
                //check if watch code is unique
                if (watchService.IsDuplicatedWatchCode(watch.WatchCode, watch.WatchID))
                {
                    //code đã tồn tại
                    //thông báo điền lại code 
                    //prefill các field
                    //result ở đây là link thumbnail cũ
                    var movement = movementService.GetMovementList();
                    var watchModel = watchModelService.GetModelsList();
                    WatchDetailViewModel viewModel = watchService.PrepopulateEditValue(watch, movement, watchModel);
                    return View("~/Views/Admin/admin_manage_watch_detail.cshtml", viewModel);
                }
                if (watchService.UpdateWatchInfo(watch, thumbnail))
                {
                    //save old value to modification table
                    String userId = Session.GetCurrentUserInfo("Username");
                    String oldValue = watchService.SerializeOldValue(watch.WatchID);
                    if (modificationService.CreateNewModificationHistory(watch.WatchID, oldValue, userId))
                    {
                        TempData["SHOW_MODAL"] = @"<script>$('#successModal').modal();</script>";
                        return RedirectToAction("ViewWatch", "Admin");
                    }
                }
                return HttpNotFound(); //change to 404
            }
            return HttpNotFound();
        }
    }
}