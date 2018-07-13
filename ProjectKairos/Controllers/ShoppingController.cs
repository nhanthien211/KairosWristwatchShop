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

            if (username == null) //not login => save in session
            {
                result = shoppingService.RemoveItemSession(id);
            }
            else //login => save in DB
            {
                result = shoppingService.RemoveItemDB(id, username);
            }

            if (result)
            {
                var total = this.CalculateCurrentTotal();
                return Json(new { success = true, responseText = total.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, responseText = "remove fail || empty cart" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("UpdateItem")]
        public JsonResult UpdateItem(string id, string quantityNeed)
        {
            int result = shoppingService.UpdateCartItem(id, quantityNeed);
            double itemSubTotal = shoppingService.GetItemSubTotal(id);

            if (result == -1) //success
            {
                var total = this.CalculateCurrentTotal();
                return Json(new { success = true, responseText = total.ToString(), quantity = -1, subTotal = itemSubTotal.ToString() }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -2) //fail
            {
                return Json(new { success = false, responseText = "+++ Update fail", quantity = -1 }, JsonRequestBehavior.AllowGet);
            }
            //update fail because not enought quantity
            //send available quantity in DB back to View to inform
            return Json(new { success = false, responseText = "+++ Update fail because quantity in DB", quantity = result }, JsonRequestBehavior.AllowGet);
        }

        private double CalculateCurrentTotal()
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