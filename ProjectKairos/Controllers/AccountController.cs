using System;
using System.Linq;
using System.Web.Mvc;
using ProjectKairos.Models;
using ProjectKairos.Utilities;

namespace ProjectKairos.Controllers
{
    public class AccountController : Controller
    {
        private KAIROS_SHOPEntities db = new KAIROS_SHOPEntities();


        [HttpGet]
        public ActionResult Login()
        {
            if (Session["CURRENT_USER_ID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/login.cshtml");
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "username, password")] Account inputAccount)
        {

            if (ModelState.IsValid)
            {

                var result = db.Accounts
                    .Where(a => a.Username == inputAccount.Username && a.IsActive == true)
                    .Select(a => new { a.Username, a.Password, a.PasswordSalt, a.RoleId })
                    .FirstOrDefault();
                //using FirstOrDefault to return null value if no instance found
                //otherwise will return exception sequence does not contain any element
                String invalidMessage = "Invalid username or password. Please try again:";
                if (result == null)
                {
                    ModelState.AddModelError("INVALID", invalidMessage);
                    return View("~/Views/login.cshtml");
                }
                //hashing password using key stored in databased
                String saltKey = result.PasswordSalt;
                String encryptPassword = EncryptPasswordUtil.EncryptPassword(inputAccount.Password, saltKey);

                //compared with encrypt password stored in database
                if (encryptPassword == result.Password)
                {
                    //password is correct
                    //Manage session
                    Session["CURRENT_USER_ID"] = inputAccount.Username;
                    Session.Timeout = 60; //1 hour

                    //return to admin dashboard if admin
                    if (result.RoleId == 1)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    //return to home page if user
                    return RedirectToAction("Index", "User");
                }
                ModelState.AddModelError("INVALID", invalidMessage);
                return View("~/Views/login.cshtml");
            }
            //return unexpected error please try again 
            //will have a 404 not found page default for all error
            return Content("Unexpected error");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (Session["CURRENT_USER"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/login.cshtml");
        }

        [HttpPost]
        public ActionResult Register([Bind(Include = "username, email, firstName, lastName, phone, dob")] Account registerAccount)
        {
            if (ModelState.IsValid)
            {
                if (Request["gender"] == "male")
                {
                    registerAccount.Gender = true;
                }
                else
                {
                    registerAccount.Gender = false;
                }
                //ModelState.IsValid: whether auto binding request
                //parameter to object account field is correct

                //Step 1 check if username exists ?
                string result = db.Accounts.Where(a => a.Username == registerAccount.Username).Select(a => a.Username).FirstOrDefault();
                if (result != null)
                {
                    //send duplicate username error

                    ModelState.AddModelError("DUPLICATE_USERNAME", "Username '" + registerAccount.Username + "' is used");
                    ViewBag.message = @"<script>
                                        $('.login-form').css('display', 'none');
                                        $('.register-form').css('display', 'block');
                                        $('.show-login-form').removeClass('active');
                                        $('.show-register-form').addClass('active');
                                        </script>";
                    return View("~/Views/login.cshtml", registerAccount);
                }

                //halting password to store in database
                //NOTE: do not auto binding password at first
                registerAccount.Password = EncryptPasswordUtil.EncryptPassword(Request["password"], out string key);
                registerAccount.PasswordSalt = key;

                //set roleID, startDate, isActive
                registerAccount.StartDate = DateTime.Now;
                registerAccount.IsActive = true;
                registerAccount.RoleId = 2; // default is member

                db.Accounts.Add(registerAccount);
                db.SaveChanges();

                //auto login and redirect based on role
                return RedirectToAction("Index", "Home");

            }
            //return unexpected error please try again 
            //will have a 404 not found page default for all error
            return Content("Unexpected error");
        }


        public ActionResult Logout()
        {
            Session.Remove("CURRENT_USER_ID");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AuthorizeUser(Role = "Administrator, Member")]
        public ActionResult UpdateMyInfo([Bind(Include = "firstName, lastName, email, phone, dob")] Account account)
        {
            if (ModelState.IsValid)
            {
                string username = (string)Session["CURRENT_USER_ID"];
                Account currentUser = db.Accounts.Find(username);

                db.Accounts.Attach(currentUser);

                currentUser.FirstName = account.FirstName;
                currentUser.LastName = account.LastName;
                currentUser.Email = account.Email;
                currentUser.Phone = account.Phone;
                currentUser.DOB = account.DOB;

                if (Request["gender"] == "male")
                {
                    currentUser.Gender = true;
                }
                else
                {
                    currentUser.Gender = false;
                }

                db.SaveChanges();
                if (currentUser.RoleId == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return Content("Unexpected Error. Please try again");
        }

        [HttpPost]
        [AuthorizeUser(Role = "Administrator, Member")]
        public ActionResult ChangePassword()
        {
            TempData["SHOW_MODAL"] = @"<script>
                                            $('#passwordModal').modal();
                                       </script>";
            string password = Request["oldPassword"];
            string newPassword = Request["newPassword"];
            string username = (string)Session["CURRENT_USER_ID"];

            Account account = db.Accounts.Find(username);

            string encryptedPassword = EncryptPasswordUtil.EncryptPassword(password, account.PasswordSalt);
            if (encryptedPassword != account.Password)
            {
                //incorrect password
                TempData["UPDATE_RESULT"] = "Password is incorrect.";
                //script to display modal
            }
            else
            {
                db.Accounts.Attach(account);
                account.Password = EncryptPasswordUtil.EncryptPassword(newPassword, out string key);
                account.PasswordSalt = key;
                db.SaveChanges();

                TempData["UPDATE_RESULT"] = "Password is saved successfully.";
            }
            //correct password


            if (account.RoleId == 1)
            {
                return RedirectToAction("Index", "Admin");
            }

            return Content("TO USER");
        }
    }


}
