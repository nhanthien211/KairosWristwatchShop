using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectKairos.Models
{
    public class ShoppingCartService
    {
        private KAIROS_SHOPEntities db;
        private int quantityHave = 0;
        private bool failByQuantity;

        public bool FailByQuantity
        {
            get { return failByQuantity; }
            set { failByQuantity = value; }
        }

        public ShoppingCartService(KAIROS_SHOPEntities db)
        {
            this.db = db;
            failByQuantity = false;
        }

        public List<ShoppingItem> LoadCartDB(string username)
        {
            return null;
        }

        /// <summary>
        /// Add to cart in Service
        /// </summary>
        /// <param name="id">Cart ID</param>
        /// <returns>false: -2</returns>
        /// <returns>success: -1</returns>
        /// <returns>fasle because not enough qunatity in DB: value Quanityt in DB</returns>
        public bool AddToCartSession(string id)
        {
            int ProductID = Convert.ToInt32(id);

            var product = db.Watches
                .Where(w => w.WatchID == ProductID)
                .Select(p => new ShoppingProduct
                {
                    ProductID = ProductID,
                    ProductCode = p.WatchCode,
                    ProductPrice = p.Price,
                    ProductPhoto = p.Thumbnail
                }).FirstOrDefault();

            try
            {
                if (HttpContext.Current.Session["CART"] == null) //cart not exist
                {
                    List<ShoppingItem> cart = new List<ShoppingItem>();
                    cart.Add(new ShoppingItem(product, 1));
                    HttpContext.Current.Session["CART"] = cart;

                    return true;
                }
                else
                {
                    List<ShoppingItem> cart = (List<ShoppingItem>)HttpContext.Current.Session["CART"];

                    int indexInCart = this.isExist(ProductID);

                    if (indexInCart == -1) //this watch havent been in cart before
                    {
                        cart.Add(new ShoppingItem(product, 1));
                    }

                    else //already existed
                    {
                        //check available in DB
                        int quantityNeed = cart[indexInCart].Quantity + 1;
                        if (this.CheckAvailableAndQuantityInDB(ProductID, quantityNeed))
                        {
                            //update quantity => true
                            cart[indexInCart].Quantity++;
                            cart[indexInCart].calculateSubTotal();
                            HttpContext.Current.Session["CART"] = cart;

                            return true;
                        }
                        //not enough quantity in DB => not ++quantity
                        failByQuantity = true;
                        return false;
                    }
                    //update cart in session
                    HttpContext.Current.Session["CART"] = cart;
                    return true;
                }
            }
            catch (Exception)
            {
                failByQuantity = false;
                return false;
            }
        }

        public bool AddToCartDB(string id, string username)
        {
            int ProductID = Convert.ToInt32(id);

            var product = db.Watches
                .Where(w => w.WatchID == ProductID)
                .Select(p => new ShoppingProduct
                {
                    ProductID = ProductID,
                    ProductCode = p.WatchCode,
                    ProductPrice = p.Price,
                    ProductPhoto = p.Thumbnail
                }).FirstOrDefault();

            try
            {
                //get user's cart with status not checkout
                var userOrderID = db.Orders
                    .Where(o => o.CustomerID.Equals(username) && o.OrderStatus == 1)
                    .Select(o => o.OrderID)
                    .FirstOrDefault();

                //=====================================================================
                if (userOrderID == 0) //currently dont have any cart
                {
                    //create new cart in DB
                    Order currentOrder = new Order();
                    currentOrder.CustomerID = username;
                    currentOrder.OrderStatus = 1;

                    db.Orders.Add(currentOrder);
                    int result = db.SaveChanges();
                    if (result == 0)
                    {
                        failByQuantity = false;
                        return false; //insert fail
                    }

                    //get orderID
                    var orderID = (int)db.Orders
                        .Where(o => o.CustomerID.Equals(username) && o.OrderStatus == 1)
                        .Select(o => o.OrderID)
                        .First();

                    //check quantity of product in DB
                    bool checkQuantity = this.CheckAvailableAndQuantityInDB(ProductID, 1); //add new 1 item
                    if (!checkQuantity)
                    {
                        failByQuantity = true;
                        return false;
                    }

                    //add new item to cart
                    OrderDetail detail = new OrderDetail();
                    detail.OrderID = orderID;
                    detail.WatchID = ProductID;
                    detail.Quantity = 1;

                    db.OrderDetails.Add(detail);
                    int resultDetail = db.SaveChanges();
                    if (resultDetail > 0)
                    {
                        return true; //success
                    }
                    else
                    {
                        failByQuantity = false;
                        return false;
                    }

                }
                //=====================================================================
                else //have cart => check if item have in cart
                {
                    var quantity = db.OrderDetails
                        .Where(d => d.OrderID == userOrderID && d.WatchID == ProductID)
                        .Select(d => d.Quantity).FirstOrDefault();

                    //========================
                    if (quantity == 0) //item not existed in order => add new item to cart, quantity = 1
                    {
                        //check quantity of product in DB
                        bool checkQuantity = this.CheckAvailableAndQuantityInDB(ProductID, 1);
                        if (!checkQuantity)
                        {
                            failByQuantity = true;
                            return false; //add fail
                        }

                        //valid quantity => add new item to order detail
                        OrderDetail detail = new OrderDetail();
                        detail.OrderID = userOrderID;
                        detail.WatchID = ProductID;
                        detail.Quantity = 1;

                        db.OrderDetails.Add(detail);
                        int resultDetail = db.SaveChanges();
                        if (resultDetail > 0)
                        {
                            return true; //success
                        }
                        else
                        {
                            failByQuantity = false;
                            return false; //fail
                        }
                    }
                    else //======================== have item in cart => update quantity
                    {
                        //check quantity of product in DB
                        bool checkQuantity = this.CheckAvailableAndQuantityInDB(ProductID, quantity + 1);
                        if (!checkQuantity)
                        {
                            failByQuantity = true;
                            return false; //add fail
                        }

                        OrderDetail detail = db.OrderDetails.Find(ProductID, userOrderID);
                        db.OrderDetails.Attach(detail);
                        detail.Quantity = quantity + 1;

                        int resultDetail = db.SaveChanges();
                        if (resultDetail > 0)
                        {
                            return true; //success
                        }
                        else
                        {
                            failByQuantity = false;
                            return false; //fail
                        }
                    }
                }
            }
            catch (Exception)
            {
                failByQuantity = false;
                return false;
            }
            failByQuantity = false;
            return false;
        }

        public bool RemoveItemSession(string id)
        {
            int ProductID = Convert.ToInt32(id);

            try
            {
                if (HttpContext.Current.Session["CART"] == null) //cart not exist
                {
                    return false;
                }
                else
                {
                    List<ShoppingItem> cart = (List<ShoppingItem>)HttpContext.Current.Session["CART"];

                    int indexInCart = this.isExist(ProductID);

                    if (indexInCart == -1) //not found item
                    {
                        return false;
                    }

                    else //already existed, remove
                    {
                        ShoppingItem toRemove = cart[indexInCart];
                        cart.Remove(toRemove);

                        //remove cart if empty
                        if (cart.Count == 0)
                        {
                            HttpContext.Current.Session["CART"] = null;
                            return false;
                        }
                    }
                    HttpContext.Current.Session["CART"] = cart;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveItemDB(string id, string username)
        {
            int ProductID = Convert.ToInt32(id);

            try
            {
                //get user's cart with status not checkout
                var userOrderID = db.Orders
                    .Where(o => o.CustomerID.Equals(username) && o.OrderStatus == 1)
                    .Select(o => o.OrderID)
                    .FirstOrDefault();

                //=====================================================================
                if (userOrderID == 0) //currently dont have any cart => can not remove
                {
                    return false;
                }
                //=====================================================================
                else //have cart => check if item have in cart
                {
                    var quantity = db.OrderDetails
                        .Where(d => d.OrderID == userOrderID && d.WatchID == ProductID)
                        .Select(d => d.Quantity).FirstOrDefault();

                    //========================
                    if (quantity == 0) //item not existed in order => can not remove
                    {
                        return false;
                    }
                    else //======================== have item in cart => remove item
                    {                                                                        
                        OrderDetail detail = db.OrderDetails.Find(ProductID, userOrderID);
                        db.OrderDetails.Attach(detail);
                        db.OrderDetails.Remove(detail);

                        int resultDetail = db.SaveChanges();
                        if (resultDetail > 0)
                        {
                            return true; //success
                        }
                        else
                        {
                            return false; //fail
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int UpdateCartItem(string id, string quantity)
        {
            int ProductID = Convert.ToInt32(id);
            int quantityNeed = Convert.ToInt32(quantity);
            try
            {
                if (HttpContext.Current.Session["CART"] == null) //cart not exist
                {
                    return -2;
                }
                else
                {
                    List<ShoppingItem> cart = (List<ShoppingItem>)HttpContext.Current.Session["CART"];

                    int indexInCart = this.isExist(ProductID);

                    if (indexInCart == -1) //not found item
                    {
                        return -2;
                    }

                    else //already existed => check available quantity in DB
                    {
                        ShoppingItem toUpdate = cart[indexInCart];

                        bool result = this.CheckAvailableAndQuantityInDB(ProductID, quantityNeed);

                        if (result) //enough quantity => update cart => return 0
                        {
                            toUpdate.Quantity = quantityNeed; //update quantity
                            toUpdate.calculateSubTotal(); //update subTotal for this product
                            HttpContext.Current.Session["CART"] = cart; //update cart
                            return -1;
                        }
                        //NOT enough quantity => not update => return availavle in DB
                        return quantityHave;
                    }
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public Dictionary<int, int> CheckCartSession()
        {
            //Check before ========================================================
            if (HttpContext.Current.Session["CART"] == null) //cart not exist
            {
                return null;
            }

            List<ShoppingItem> cart = (List<ShoppingItem>)HttpContext.Current.Session["CART"];

            if (cart.Count == 0)
            {
                return null;
            }

            //Begin to find invalid quantity in cart ==============================
            Dictionary<int, int> IdAndError = new Dictionary<int, int>();

            foreach (ShoppingItem item in cart)
            {
                bool result = CheckAvailableAndQuantityInDB(item.Product.ProductID, item.Quantity);
                if (!result)
                {
                    IdAndError.Add(item.Product.ProductID, this.quantityHave);
                }
            }

            return IdAndError;
        }

        public bool CheckAvailableAndQuantityInDB(int id, int quantityWant)
        {
            var quantityHave = db.Watches
                        .Where(w => w.WatchID == id)
                        .Select(w => w.Quantity).First();
            this.quantityHave = quantityHave;
            return quantityHave >= quantityWant;
        }

        public int GetItemQuantityInDB(string id)
        {
            int productID = Convert.ToInt32(id);

            var result = db.Watches.Where(w => w.WatchID == productID).Select(w => w.Quantity).First();

            return result;
        }

        public double GetItemSubTotal(string id)
        {
            int ProductID = Convert.ToInt32(id);

            if (HttpContext.Current.Session["CART"] == null) //cart not exist
            {
                return 0;
            }
            else
            {
                List<ShoppingItem> cart = (List<ShoppingItem>)HttpContext.Current.Session["CART"];

                int indexInCart = this.isExist(ProductID);

                if (indexInCart == -1) //this watch havent been in cart before
                {
                    return 0;
                }

                else //already existed
                {
                    cart[indexInCart].calculateSubTotal();
                    return cart[indexInCart].SubTotal;
                }
            }
        }

        private int isExist(int id)
        {
            List<ShoppingItem> cart = (List<ShoppingItem>)HttpContext.Current.Session["CART"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.ProductID.Equals(id))
                    return i;
            return -1;
        }

    }
}