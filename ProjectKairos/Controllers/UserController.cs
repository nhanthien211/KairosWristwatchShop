﻿using ProjectKairos.Models;
using ProjectKairos.Utilities;
using ProjectKairos.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Member")]
    public class UserController : Controller
    {
        private KAIROS_SHOPEntities db;
        private AccountService accountService;
        private ShoppingCartService shoppingService;

        public UserController()
        {
            db = new KAIROS_SHOPEntities();
            accountService = new AccountService(db);
            shoppingService = new ShoppingCartService(db);
        }

        // GET: User
        [HttpGet]
        [AuthorizeUser(Role = "Member")]
        [Route]
        public ActionResult Index()
        {
            return View("~/Views/Home/index.cshtml");
        }

        [HttpGet]
        [Route("Manage/Account")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ManageAccount()
        {
            string username = Session.GetCurrentUserInfo("Username");
            var viewModel = accountService.ViewMyAccount(username);
            return View("~/Views/User/user_account.cshtml", viewModel);
        }

        [HttpGet]
        [Route("Checkout")]
        public ActionResult CheckOut()
        {            
            if (Session["CURRENT_USER_ID"] != null)
            {
                string roleName = Session.GetCurrentUserInfo("RoleName");
                if (roleName == "Member") //login
                {
                    string username = Session.GetCurrentUserInfo("Username");
                    List<ShoppingItem> cartDB = shoppingService.LoadCartItemDB(username);
                    if (cartDB != null && cartDB.Count != 0)
                    {
                        Dictionary<int, int> IdAndRErrorDB = shoppingService.CheckCartDB(cartDB);
                        if (IdAndRErrorDB.Count == 0) //cart valid
                        {
                            var viewModel = new ShippingInfoViewModel();
                            viewModel.Total = shoppingService.CalculateCartTotalDB(username);
                            return View("~/Views/User/user_checkout.cshtml", viewModel);
                        }
                    }
                    return RedirectToAction("ManageCart", "Home");
                } else if (roleName == "Administrator")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            //not login =====================================================
            //Check if cart existed and not empty
            List<ShoppingItem> cart = (List<ShoppingItem>)Session["CART"];
            if (cart != null && cart.Count != 0)
            {
                //Validate cart quantity by session
                Dictionary<int, int> IdAndRError = shoppingService.CheckCartSession();
                if (IdAndRError.Count == 0) //cart valid
                {
                    return RedirectToAction("Login", "Account");
                }
            }            
            return RedirectToAction("ManageCart", "Home"); //đã có validate trong đây
        }

        [HttpGet]
        [Route("Manage/Order")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ManageOrder()
        {
            return View("~/Views/User/user_order.cshtml");
        }

        [HttpGet]
        [Route("OrderDetail")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ViewOrderDetail()
        {
            return View("~/Views/User/user_order_detail.cshtml");
        }

        [HttpPost]
        [Route("ConfirmOrder")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ConfirmOrder([Bind(Include = "shipName, shipPhone, shipCity, shipDistrict, shipWard, shipStreet, shippAddressNumber, shipNote")]Order order)
        {
            string username = Session.GetCurrentUserInfo("Username");

            bool hasCart = shoppingService.CheckCartExistedInDB(username);
            if (!hasCart) //cart not existed || cart empty => return to cart page
            {
                return RedirectToAction("ManageCart", "Home");
            }

            //cart existed => get cart
            List<ShoppingItem> cart = shoppingService.LoadCartItemDB(username);
            //validate quantity in cart
            Dictionary<int, int> IDANdError = shoppingService.CheckCartDB(cart);

            if(IDANdError != null && IDANdError.Count != 0) //there are error in quantity in cart => return to cart page
            {
                var viewModelDB = new ShoppingCartViewModel(cart);
                viewModelDB.IdAndError = IDANdError;
                return View("~/Views/Home/shopping_cart.cshtml", viewModelDB);
            }

            //valid cart => check shipping info
            bool result = shoppingService.UpdateShippingInfo(username, order);
            
            if (result)
            {               
                return View("~/Views/User/user_order.cshtml");
            }

            return Content("Unexpected Error. Please try again");
        }
    }
}