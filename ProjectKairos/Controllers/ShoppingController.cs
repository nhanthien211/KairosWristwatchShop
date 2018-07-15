using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectKairos.Models;
using ProjectKairos.ViewModel;
using ProjectKairos.Utilities;

namespace ProjectKairos.Controllers
{
    [RoutePrefix("Shopping")]
    public class ShoppingController : Controller
    {
        private KAIROS_SHOPEntities db;
        private ShoppingCartService shoppingService;

        
        public ShoppingController()
        {
            db = new KAIROS_SHOPEntities();
            shoppingService = new ShoppingCartService(db);
        }

        // GET: Shopping
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("AddToCart")]
        public JsonResult AddToCart(string id)
        {
            string username = Session.GetCurrentUserInfo("Username");
            bool result = false;

            if (username == null) //not login => save in session
            {
                result = shoppingService.AddToCartSession(id);
            } else //login => save in DB
            {
                result = shoppingService.AddToCartDB(id, username);
            }

            if (result) //success
            {
                return Json(new { success = true, responseText = "Add successfuly", quantity = -1 }, JsonRequestBehavior.AllowGet);
            } else //fail
            {
                if (shoppingService.FailByQuantity)
                { //fail because quantity not enough => send available quantity in DB back to View to inform
                    int quantityHave = shoppingService.GetItemQuantityInDB(id);
                    return Json(new { success = false, responseText = "Add fail because quantity", quantity = quantityHave }, JsonRequestBehavior.AllowGet);
                }
                //fail
                return Json(new { success = false, responseText = "Add fail", quantity = -1 }, JsonRequestBehavior.AllowGet);
            }            
        }

        [HttpPost]
        [Route("RemoveItem")]
        public JsonResult RemoveItem(string id)
        {
            string username = Session.GetCurrentUserInfo("Username");
            bool result = false;
            double total = 0;

            if (username == null) //not login => save in session
            {
                result = shoppingService.RemoveItemSession(id);
                total = this.CalculateCurrentTotalSession();
            }
            else //login => save in DB
            {
                result = shoppingService.RemoveItemDB(id, username);
                total = shoppingService.CalculateCartTotalDB(username);
            }

            if (result)
            {
                if (shoppingService.EmptyCart) //empty cart => refresh page!
                {
                    return Json(new { success = true, responseText = total.ToString(), empty = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, responseText = total.ToString(), empty = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, responseText = "remove fail" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("UpdateItem")]
        public JsonResult UpdateItem(string id, string quantityNeed)
        {
            string username = Session.GetCurrentUserInfo("Username");
            bool result = false;
            double itemSubTotal = 0;
            double total = 0;

            if (username == null) //not login => update in session
            {
                result = shoppingService.UpdateCartItemSession(id, quantityNeed);
                itemSubTotal = shoppingService.GetItemSubTotalSession(id);
                total = this.CalculateCurrentTotalSession();
            }
            else //login => update in DB
            {
                result = shoppingService.UpdateCartItemDB(id, quantityNeed, username);
                itemSubTotal = shoppingService.GetItemSubTotalDB(id, username);
                total = shoppingService.CalculateCartTotalDB(username);
            }


            if (result) //success
            {
                return Json(new { success = true, responseText = total.ToString(), quantity = -1, subTotal = itemSubTotal.ToString() }, JsonRequestBehavior.AllowGet);
            }
            else //fail
            {
                if (shoppingService.FailByQuantity)
                { //fail because quantity not enough => send available quantity in DB back to View to inform
                    int quantityHave = shoppingService.GetItemQuantityInDB(id);
                    return Json(new { success = false, responseText = "+++ Update fail because quantity in DB", quantity = quantityHave.ToString() }, JsonRequestBehavior.AllowGet);
                }
                //fail
                return Json(new { success = false, responseText = "Add fail", quantity = -1 }, JsonRequestBehavior.AllowGet);
            }       
        }

        private double CalculateCurrentTotalSession()
        {
            if (Session["CART"] == null)
            {
                return 0;
            }

            double total = 0;
            List<ShoppingItem> currentCart = (List<ShoppingItem>)Session["CART"];

            foreach (ShoppingItem item in currentCart)
            {
                total = total + (item.Quantity * item.Product.ProductPrice);
            }

            return total;
        }        
    }
}