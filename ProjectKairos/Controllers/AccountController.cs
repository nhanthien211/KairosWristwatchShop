using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using ProjectKairos.Models;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;

namespace ProjectKairos.Controllers
{
    public class AccountController : Controller
    {
        private KAIROS_SHOPEntities db;
        private AccountService accountService;
        private ShoppingCartService shoppingService;

        public AccountController()
        {
            db = new KAIROS_SHOPEntities();
            accountService = new AccountService(db);
            shoppingService = new ShoppingCartService(db);
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["CURRENT_USER_ID"] != null)
            {

                return RedirectToAction("Index", "Home");
            }
            AccountRegisterViewModel viewModel = new AccountRegisterViewModel();
            return View("~/Views/Home/login.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            string result = accountService.CheckLogin(username, password);
            if (result == null)
            {
                //login k thành công 
                var viewModel = new AccountRegisterViewModel
                {
                    InvalidLogin = "Invalid username or password. Please try again"
                };
                return View("~/Views/Home/login.cshtml", viewModel);
            }
            Session["CURRENT_USER_ID"] = result;

            //merge cart if any
            bool resultMerge = shoppingService.MergeCartSessionAnddDDB(Session.GetCurrentUserInfo("Username"));
            if (resultMerge) //done => remove cart in session
            {
                Session["CART"] = null;
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (Session["CURRENT_USER_ID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            AccountRegisterViewModel viewModel = new AccountRegisterViewModel();
            return View("~/Views/Home/login.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult Register([Bind(Include = "username, email, firstName, lastName, phone, dob")] Account registerAccount)
        {
            if (ModelState.IsValid)
            {
                registerAccount.Gender = Request["gender"] == "male";
                //ModelState.IsValid: whether auto binding request
                //parameter to object account field is correct

                //Step 1 check if username exists ?
                string duplicateUsername = "";
                string duplicateEmail = "";
                bool canAdd = true;
                if (accountService.IsDuplicatedUsername(registerAccount.Username))
                {
                    duplicateUsername = "Username '" + registerAccount.Username + "' is duplicated";
                    canAdd = false;
                }

                if (accountService.IsDuplicatedEmail(registerAccount.Email))
                {
                    duplicateEmail = "Email '" + registerAccount.Email + "' is duplicated";
                    canAdd = false;
                }

                if (!canAdd)
                {
                    var viewModel = new AccountRegisterViewModel
                    {
                        Username = registerAccount.Username,
                        Email = registerAccount.Email,
                        FirstName = registerAccount.FirstName,
                        LastName = registerAccount.LastName,
                        Gender = registerAccount.Gender,
                        Dob = registerAccount.DOB,
                        Phone = registerAccount.Phone,
                        DuplicateEmailErrorMessage = duplicateEmail,
                        DuplicateUsernameErrorMessage = duplicateUsername
                    };
                    ViewBag.message = @"<script>$('.login-form').css('display', 'none');$('.register-form').css('display', 'block');$('.show-login-form').removeClass('active');$('.show-register-form').addClass('active');</script>";
                    return View("~/Views/Home/login.cshtml", viewModel);
                }
                //halting password to store in database
                //NOTE: do not auto binding password at first
                registerAccount.Password = EncryptPasswordUtil.EncryptPassword(Request["password"], out string key);
                registerAccount.PasswordSalt = key;
                //set roleID, startDate, isActive
                registerAccount.StartDate = DateTime.Now;
                registerAccount.IsActive = true;
                registerAccount.RoleId = 2; // default is member

                if (accountService.AddNewAccount(registerAccount))
                {
                    //auto login and redirect based on role
                    var loginAccount = new
                    {
                        Username = registerAccount.Username,
                        RoleName = accountService.GetRoleName(registerAccount.Username)
                    };
                    Session["CURRENT_USER_ID"] = JsonConvert.SerializeObject(loginAccount, Formatting.Indented);
                    return Redirect(Request.UrlReferrer.ToString());
                }
                return HttpNotFound();
            }
            //return unexpected error please try again 
            //will have a 404 not found page default for all error
            return Content("Unexpected error");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AuthorizeUser(Role = "Administrator, Member")]
        public ActionResult UpdateMyInfo([Bind(Include = "firstName, lastName, phone, dob")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.Gender = Request["gender"] == "male";
                string username = Session.GetCurrentUserInfo("Username");
                if (accountService.UpdateAccountInfo(account, username))
                {
                    TempData["SHOW_MODAL"] = @"<script>$('#successModal').modal();</script>";
                    if (Session.GetCurrentUserInfo("RoleName") == "Administrator")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    if (Session.GetCurrentUserInfo("RoleName") == "Member")
                    {
                        return RedirectToAction("ManageAccount", "User");
                    }
                }

            }
            return Content("Unexpected Error. Please try again");
        }

        [HttpPost]
        [AuthorizeUser(Role = "Administrator, Member")]
        public ActionResult ChangePassword(string oldPassword, string newPassword)
        {
            TempData["SHOW_MODAL"] = @"<script>$('#passwordModal').modal();</script>";
            string username = Session.GetCurrentUserInfo("Username");

            if (accountService.UpdateAccountPassword(username, oldPassword, newPassword))
            {
                TempData["UPDATE_RESULT"] = "Password is saved successfully.";
            }
            else
            {
                //incorrect password
                TempData["UPDATE_RESULT"] = "Old Password is incorrect.";
                //script to display modal
            }
            //correct password
            if (Session.GetCurrentUserInfo("RoleName") == "Administrator")
            {
                return RedirectToAction("Index", "Admin");
            }
            if (Session.GetCurrentUserInfo("RoleName") == "Member")
            {
                return RedirectToAction("ManageAccount", "User");
            }
            return RedirectToAction("Index", "Home");
        }
    }


}
