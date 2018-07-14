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
                return RedirectToAction("NotFound", "Home");
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

            return RedirectToAction("NotFound", "Home"); //change to 404/ server busy
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
            if (TempData["EXCEL"] != null)
            {
                viewModel.InvalidExcelFileMessage = (string)TempData["EXCEL"];
            }
            if (TempData["ZIP"] != null)
            {
                viewModel.InvalidZipFileMessage = (string)TempData["ZIP"];
            }
            if (TempData["RESULT"] != null)
            {
                viewModel.ImportMessage = (string)TempData["RESULT"];
            }
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
                watch.WatchCode = watch.WatchCode.ToUpper();
                bool duplicateCode = watchService.IsDuplicatedWatchCode(watch.WatchCode);
                bool validImage = FileTypeDetector.IsImageFile(thumbnail);
                //check if watch code is unique
                if (!validImage || duplicateCode)
                {
                    //code đã tồn tại hoặc hình ảnh không hợp lệ
                    //thông báo điền lại code                     
                    //prefill các field
                    var movement = movementService.GetMovementList();
                    var watchModel = watchModelService.GetModelsList();
                    var viewModel = watchService.PrepopulateInputValue(watch, movement, watchModel);
                    if (duplicateCode)
                    {
                        viewModel.DuplicateErrorMessage = "Watch with code '" + watch.WatchCode + "' already existed";
                    }

                    if (!validImage)
                    {
                        viewModel.InvalidImageFileMessage = "Invalid Thumbnail. Upload file is not an image";
                    }
                    return View("~/Views/Admin/admin_manage_watch_add.cshtml", viewModel);
                }
                //lưu hình ảnh xuống máy  
                string path = HostingEnvironment.MapPath("~/Content/img/ProductThumbnail/") + watch.WatchCode + DateTime.Now.ToBinary();
                //thumbnail.InputStream.Position = 0;
                ImageProcessHelper.ResizedImage(thumbnail.InputStream, 360, 500, ResizeMode.Pad, ref path);
                watch.Thumbnail = path;
                watch.PublishedTime = DateTime.Now;
                watch.PublishedBy = Session.GetCurrentUserInfo("Username");
                watch.Status = true;
                if (watchService.AddNewWatch(watch))
                {
                    TempData["SHOW_MODAL"] = @"<script>$('#successModal').modal();</script>";
                    return RedirectToAction("AddWatch", "Admin");
                }
                return Content("Unexpected Error");
            }
            return RedirectToAction("NotFound", "Home");
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
            catch (FormatException)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var watchDetail = watchService.ViewWatchDetail(id);
            if (watchDetail == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            watchDetail.Movement = movementService.GetMovementList();
            watchDetail.WatchModel = watchModelService.GetModelsList();
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
                watch.WatchCode = watch.WatchCode.ToUpper();
                bool duplicateCode = watchService.IsDuplicatedWatchCode(watch.WatchCode, watch.WatchID);
                bool validImage = FileTypeDetector.IsImageFile(thumbnail);
                if (duplicateCode || !validImage)
                {
                    var movement = movementService.GetMovementList();
                    var watchModel = watchModelService.GetModelsList();
                    ManageWatchDetailViewModel viewModel = watchService.PrepopulateEditValue(watch, movement, watchModel);
                    if (duplicateCode)
                    {
                        viewModel.DuplicateErrorMessage = "Watch with code '" + watch.WatchCode + "' already existed. Please choose another one";
                    }

                    if (!validImage)
                    {
                        viewModel.InvalidImageFileMessage = "Invalid Thumbnail. Upload file is not image";
                    }
                    return View("~/Views/Admin/admin_manage_watch_detail.cshtml", viewModel);
                }
                String oldValue = watchService.SerializeOldValue(watch.WatchID);
                //thumbnail.InputStream.Position = 0;

                if (watchService.UpdateWatchInfo(watch, thumbnail))
                {
                    //save old value to modification table
                    String userId = Session.GetCurrentUserInfo("Username");

                    if (modificationService.CreateNewModificationHistory(watch.WatchID, oldValue, userId))
                    {
                        TempData["SHOW_MODAL"] = @"<script>$('#successModal').modal();</script>";
                        return RedirectToAction("ViewWatch", "Admin");
                    }
                }
                return Content("Unexpected Error"); //change to 404
            }
            return RedirectToAction("NotFound", "Home");
        }

        [HttpPost]
        [Route("ImportWatch")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult ImportWatch(HttpPostedFileBase zip, HttpPostedFileBase excel)
        {

            bool canImport = true;
            TempData["SHOW_MODAL"] = @"<script>$('#importModal').modal();</script>";

            string excelResult = ProcessImportFileHelper.CheckValidExcelFile(excel);
            if (excelResult != null)
            {
                canImport = false;
                TempData["EXCEL"] = excelResult;
            }

            string zipResult = ProcessImportFileHelper.CheckValidZipFile(zip);
            if (zipResult != null)
            {
                canImport = false;
                TempData["ZIP"] = zipResult;
            }

            if (!canImport)
            {
                return RedirectToAction("AddWatch", "Admin");
            }
            int result = watchService.ImportWatchFromFile(excel, zip, Session.GetCurrentUserInfo("Username"));
            TempData["RESULT"] = "Total " + result + " watch(es) imported successfully";

            return RedirectToAction("AddWatch", "Admin");
        }
    }
}