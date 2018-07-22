using System;
using ProjectKairos.Models;
using ProjectKairos.Utilities;
using System.Collections.Generic;
using System.Web.Mvc;
using ProjectKairos.ViewModel;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Member")]
    public class UserController : Controller
    {
        private KAIROS_SHOPEntities db;
        private AccountService accountService;
        private ShoppingCartService shoppingService;
        private OrderService orderService;
        private OrderDetailService orderDetailService;
        private ReviewService reviewService;

        public UserController()
        {
            db = new KAIROS_SHOPEntities();
            accountService = new AccountService(db);
            shoppingService = new ShoppingCartService(db);
            orderService = new OrderService(db);
            orderDetailService = new OrderDetailService(db);
            reviewService = new ReviewService(db);
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

        [HttpPost]
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
                            var viewModel = new ShippingInfoViewModel
                            {
                                Total = shoppingService.CalculateCartTotalDB(username)
                            };
                            return View("~/Views/User/user_checkout.cshtml", viewModel);
                        }
                    }
                    return RedirectToAction("ManageCart", "Home");
                }
                else if (roleName == "Administrator")
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
            string username = Session.GetCurrentUserInfo("Username");
            List<OrderTableViewModel> viewModel = orderService.LoadAllCustomerOrder(username);
            return View("~/Views/User/user_order.cshtml", viewModel);
        }

        [HttpGet]
        [Route("Manage/Order/{orderId}")]
        [AuthorizeUser(Role = "Member")]
        public ActionResult ViewOrderDetail(string orderId)
        {
            int id;
            try
            {
                id = Convert.ToInt32(orderId);
                if (!orderService.IsValidOrderId(id))
                {
                    return RedirectToAction("NotFound", "Home");
                }
            }
            catch (FormatException)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var viewModel = orderService.LoadOrderDetailUser(id);
            viewModel.OrderItem = orderDetailService.LoadAllItemInOrder(id);

            return View("~/Views/User/user_order_detail.cshtml", viewModel);
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

            if (IDANdError != null && IDANdError.Count != 0) //there are error in quantity in cart => return to cart page
            {
                var viewModelDB = new ShoppingCartViewModel(cart);
                viewModelDB.IdAndError = IDANdError;
                return View("~/Views/Home/shopping_cart.cshtml", viewModelDB);
            }

            //valid cart => check shipping info
            bool result = shoppingService.UpdateShippingInfo(username, order);

            if (result)
            {
                return RedirectToAction("ManageOrder", "User");
            }

            return Content("Unexpected Error. Please try again");
        }

        [HttpPost]
        [Route("Review/Watch")]
        public ActionResult ReviewWatch(string watchId, string orderId)
        {
            int id, order;
            try
            {
                id = Convert.ToInt32(watchId);
                order = Convert.ToInt32(orderId);
            }
            catch
            {
                return RedirectToAction("NotFound", "Home");
            }

            string username = Session.GetCurrentUserInfo("Username");
            var viewModel = reviewService.ViewWatchReview(id, username, order);
            return View("~/Views/User/user_review.cshtml", viewModel);
        }

        public ActionResult SubmitRating(string watchId, string star, string orderId)
        {
            int rating, id;
            try
            {
                rating = Convert.ToInt32(star);
                id = Convert.ToInt32(watchId);
            }
            catch (FormatException)
            {
                return RedirectToAction("NotFound", "Home");
            }

            string username = Session.GetCurrentUserInfo("Username");
            bool result = reviewService.RateStarWatch(id, username, rating);
            if (result)
            {
                return RedirectToAction("ViewOrderDetail", "User", new { orderId = orderId });
            }

            return Content("Unexpected Error Orrcured");
        }
    }
}
