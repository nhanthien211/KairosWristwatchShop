using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ProjectKairos.Models;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Admin")]
    public class AdminController : Controller
    {

        private KAIROS_SHOPEntities db = new KAIROS_SHOPEntities();

        // GET: Admin
        [HttpGet]
        [AuthorizeUser(Role = "Administrator")]
        [Route]
        public ActionResult Index()
        {
            string username = (string)Session["CURRENT_USER_ID"];
            var account = db.Accounts.Where(a => a.Username == username)
                .Select(a => new AccountInfoViewModel
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    Phone = a.Phone,
                    DOB = a.DOB,
                    StartedDate = a.StartDate,
                    Gender = a.Gender
                }).First();
            return View("~/Views/Admin/admin_info.cshtml", account);
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

        #region view and edit account
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

            // Getting all Account data  
            var account = db.Accounts.Include(a => a.Role).Select(a => new { a.Username, FullName = a.LastName + " " + a.FirstName, a.Role.RoleName, a.IsActive });


            //sorting
            if (!string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumnDirection)
                {
                    case "asc":
                        account = account.OrderBy(a => a.FullName);
                        break;
                    case "desc":
                        account = account.OrderByDescending(a => a.FullName);
                        break;
                }
            }

            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                account = account.Where(a => a.Username.Contains(searchValue) || a.FullName.Contains(searchValue));
            }

            //total number of rows count   
            recordsTotal = account.Count();

            //Paging   
            var data = account.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        [Route("Manage/Account/View/{username}")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult ViewAccount(string username)
        {
            var role = db.Roles.ToList();
            var account = db.Accounts.Where(a => a.Username == username).Select(a => new AccountInfoRoleViewModel
            {
                Username = a.Username,
                FullName = a.LastName + " " + a.FirstName,
                Phone = a.Phone,
                Email = a.Email,
                DOB = a.DOB,
                Gender = a.Gender,
                StartDate = a.StartDate,
                IsActive = a.IsActive,
                RoleId = a.RoleId

            }).FirstOrDefault();
            if (account == null)
            {
                return HttpNotFound();
            }

            account.Role = role;
            return View("~/Views/Admin/admin_manage_user_detail.cshtml", account);
        }

        [HttpPost]
        [Route("{username}")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult EditAccount(string username)
        {
            string status = Request["isActive"];
            bool isActive = false;
            if (status == "active")
            {
                isActive = true;
            }

            int roleId = Convert.ToInt32(Request["selectRole"]);

            Account account = db.Accounts.Find(username);
            if (account == null)
            {
                return HttpNotFound();
            }
            db.Accounts.Attach(account);

            account.IsActive = isActive;
            account.RoleId = roleId;

            db.SaveChanges();

            return RedirectToAction("ViewAccount", "Admin");
        }
        #endregion

        #region add new watch
        [HttpGet]
        [Route("Manage/Watch/Add")]
        [AuthorizeUser(Role = "Administrator")]
        public ActionResult AddWatch()
        {
            return View("~/Views/Admin/admin_manage_watch_add.cshtml");
        }


        #endregion

    }
}