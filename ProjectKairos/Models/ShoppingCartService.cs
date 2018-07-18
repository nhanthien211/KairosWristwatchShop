using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectKairos.Models
{
    public class ShoppingCartService
    {
        private KAIROS_SHOPEntities db;
        private int quantityHave = 0;

        // Variable to check if action fail because invalid quantity in DB
        private bool failByQuantity;
        public bool FailByQuantity
        {
            get { return failByQuantity; }
            set { failByQuantity = value; }
        }

        // Variable to check if cart is empty after remove a product
        private bool emptyCart;
        public bool EmptyCart
        {
            get { return emptyCart; }
            set { emptyCart = value; }
        }


        public ShoppingCartService(KAIROS_SHOPEntities db)
        {
            this.db = db;
            failByQuantity = false;
            emptyCart = false;
        }

        public bool MergeCartSessionAnddDDB(string username)
        {
            List<ShoppingItem> cartSession = (List<ShoppingItem>)HttpContext.Current.Session["CART"];
            List<ShoppingItem> cartDB = this.LoadCartItemDB(username);

            if (cartSession == null) //no cart in session => dont need to merge
            {
                return true;
            }
            if (cartSession != null)
            {
                if (cartDB == null) //no cart in DB => create new order
                {
                    //create new cart in DB
                    Order currentOrder = new Order();
                    currentOrder.CustomerID = username;
                    currentOrder.OrderStatus = 1;

                    db.Orders.Add(currentOrder);
                    int result = db.SaveChanges();
                    if (result == 0)
                    {
                        return false; //insert fail
                    }
                }
                //have cart => merge!!!
                int cartID = db.Orders
                    .Where(o => o.CustomerID.Equals(username) && o.OrderStatus == 1)
                    .Select(o => o.OrderID)
                    .FirstOrDefault();

                foreach (ShoppingItem item in cartSession)
                {
                    bool re = this.MergeItemInCartDB(cartID, item.Product.ProductID, item.Quantity);
                    if (!re)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool MergeItemInCartDB(int orderID, int productID, int quantitySession)
        {
            OrderDetail itemDB = db.OrderDetails.Find(productID, orderID);

            if (itemDB == null) //item not existed => create new item
            {
                OrderDetail itemToAdd = new OrderDetail
                {
                    OrderID = orderID,
                    WatchID = productID,
                    Quantity = quantitySession
                };
                db.OrderDetails.Add(itemToAdd);
                int re = db.SaveChanges();
                return re > 0;
            } //item existed => update quantity, no need to validate quantity avaiable in DB

            db.OrderDetails.Attach(itemDB);
            int quantityDB = itemDB.Quantity;
            itemDB.Quantity = quantityDB + quantitySession;

            int re1 = db.SaveChanges();
            return re1 > 0;
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

        }

        public bool RemoveItemSession(string id)
        {
            int ProductID = Convert.ToInt32(id);
            emptyCart = false;

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
                            emptyCart = true;
                            return true;
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
            emptyCart = false;

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
                    if (quantity == 0) //item not existed in order || item auto become 0 =>
                    {
                        OrderDetail detail = db.OrderDetails.Find(ProductID, userOrderID);
                        db.OrderDetails.Attach(detail);
                        db.OrderDetails.Remove(detail);

                        int resultDetail = db.SaveChanges();
                        if (resultDetail > 0)
                        {
                            //check if cart is empty => to refresh page
                            var itemInCart = db.OrderDetails
                                .Where(d => d.OrderID == userOrderID)
                                .Select(d => d.WatchID)
                                .ToList();

                            if (itemInCart.Count != 0)
                            {
                                return true; //still have item in cart in OrderDetail DB
                            }

                            var curEmptyOrder = db.Orders.Find(userOrderID);
                            db.Orders.Attach(curEmptyOrder);
                            db.Orders.Remove(curEmptyOrder);
                            db.SaveChanges();

                            emptyCart = true;
                            return true; //success + empty cart!
                        }
                        else
                        {
                            return false; //SaveChange fail
                        }
                    }
                    else //======================== have item in cart => remove item
                    {
                        OrderDetail detail = db.OrderDetails.Find(ProductID, userOrderID);
                        db.OrderDetails.Attach(detail);
                        db.OrderDetails.Remove(detail);

                        int resultDetail = db.SaveChanges();
                        if (resultDetail > 0)
                        {
                            //check if cart is empty => to refresh page
                            var itemInCart = db.OrderDetails
                                .Where(d => d.OrderID == userOrderID)
                                .Select(d => d.WatchID)
                                .ToList();

                            if (itemInCart.Count != 0)
                            {
                                return true; //still have item in cart in OrderDetail DB
                            }

                            var curEmptyOrder = db.Orders.Find(userOrderID);
                            db.Orders.Attach(curEmptyOrder);
                            db.Orders.Remove(curEmptyOrder);
                            db.SaveChanges();

                            emptyCart = true;
                            return true; //success + empty cart!
                        }
                        else
                        {
                            return false; //SaveChange fail
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateCartItemSession(string id, string quantity)
        {
            int ProductID = Convert.ToInt32(id);
            int quantityNeed = Convert.ToInt32(quantity);
            failByQuantity = false;

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

                    else //already existed => check available quantity in DB
                    {
                        ShoppingItem toUpdate = cart[indexInCart];

                        bool result = this.CheckAvailableAndQuantityInDB(ProductID, quantityNeed);

                        if (result) //enough quantity => update cart => return 0
                        {
                            toUpdate.Quantity = quantityNeed; //update quantity
                            toUpdate.calculateSubTotal(); //update subTotal for this product
                            HttpContext.Current.Session["CART"] = cart; //update cart
                            return true;
                        }
                        //NOT enough quantity => not update => return availavle in DB
                        failByQuantity = true;
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateCartItemDB(string id, string quantity, string username)
        {
            int productID = Convert.ToInt32(id);
            int quantityNeed = Convert.ToInt32(quantity);
            failByQuantity = false;

            //get user's cart with status not checkout
            var userOrderID = db.Orders
                .Where(o => o.CustomerID.Equals(username) && o.OrderStatus == 1)
                .Select(o => o.OrderID)
                .FirstOrDefault();

            //=====================================================================
            if (userOrderID == 0) //currently dont have any cart => can not update => fail
            {
                return false;
            }
            //=====================================================================
            else //have cart => check if item have in cart
            {
                var quantityCurent = db.OrderDetails
                    .Where(d => d.OrderID == userOrderID && d.WatchID == productID)
                    .Select(d => d.Quantity).FirstOrDefault();

                //========================
                if (quantityCurent == 0) //item not existed in order => can not update
                {
                    return false;
                }
                else //======================== have item in cart => check avaiable quantity in DB
                {
                    bool checkQuantity = this.CheckAvailableAndQuantityInDB(productID, quantityNeed);
                    if (!checkQuantity)
                    {
                        failByQuantity = true;
                        return false; //fail because not enough quantity
                    }

                    //enough quantity => update
                    OrderDetail detail = db.OrderDetails.Find(productID, userOrderID);
                    db.OrderDetails.Attach(detail);
                    detail.Quantity = quantityNeed;

                    int resultDetail = db.SaveChanges();
                    if (db.Entry(detail).State == EntityState.Unchanged || resultDetail > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false; //fail
                    }
                }
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

        public Dictionary<int, int> CheckCartDB(List<ShoppingItem> cart)
        {
            //check cart ========================================================
            if (cart == null)
            {
                return null;
            }
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

        public bool CheckCartExistedInDB(string username)
        {
            try
            {
                //get user's cart with status not checkout
                var userOrderID = db.Orders
                    .Where(o => o.CustomerID.Equals(username) && o.OrderStatus == 1)
                    .Select(o => o.OrderID)
                    .FirstOrDefault();

                //=====================================================================
                if (userOrderID == 0) //currently dont have any cart => not check
                {
                    return false;
                }
                //=====================================================================
                else //have cart => check if cart is empty
                {
                    var item = db.OrderDetails
                            .Where(o => o.OrderID == userOrderID)
                            .Select(o => new { o.WatchID, o.Quantity })
                            .ToList();

                    if (item.Count == 0) //no item in cart
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ShoppingItem> LoadCartItemDB(string username)
        {
            var orderID = db.Orders
                .Where(o => o.CustomerID == username && o.OrderStatus == 1)
                .Select(o => o.OrderID).FirstOrDefault();

            var itemsID = db.OrderDetails
                .Where(d => d.OrderID == orderID)
                .Select(o => new { o.WatchID, o.Quantity })
                .ToList();

            List<ShoppingItem> listItem = new List<ShoppingItem>();

            foreach (var item in itemsID)
            {
                ShoppingProduct p = db
                        .Watches.Where(w => w.WatchID == item.WatchID)
                        .Select(w => new ShoppingProduct
                        {
                            ProductID = w.WatchID,
                            ProductCode = w.WatchCode,
                            ProductPrice = w.Price,
                            ProductPhoto = w.Thumbnail
                        })
                        .FirstOrDefault();

                ShoppingItem curItem = new ShoppingItem(p, item.Quantity);
                listItem.Add(curItem);
            }

            if (listItem.Count != 0)
            {
                return listItem;
            }
            return null;
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

        public double GetItemSubTotalSession(string id)
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

        public double GetItemSubTotalDB(string id, string username)
        {
            int productID = Convert.ToInt32(id);

            //get user's cart with status not checkout
            var userOrderID = db.Orders
                .Where(o => o.CustomerID.Equals(username) && o.OrderStatus == 1)
                .Select(o => o.OrderID)
                .FirstOrDefault();

            //=====================================================================
            if (userOrderID == 0) //currently dont have any cart
            {
                return 0;
            }
            //=====================================================================
            else //have cart => check if item have in cart
            {
                var quantityCurent = db.OrderDetails
                    .Where(d => d.OrderID == userOrderID && d.WatchID == productID)
                    .Select(d => d.Quantity).FirstOrDefault();

                //========================
                if (quantityCurent == 0) //item not existed in order
                {
                    return 0;
                }
                else //======================== have item in cart
                {
                    var price = db.Watches.Where(w => w.WatchID == productID).Select(w => w.Price).First();
                    return price * quantityCurent;
                }
            }
        }

        public double CalculateCartTotalDB(string username)
        {
            List<ShoppingItem> cart = this.LoadCartItemDB(username);
            double total = 0;

            if (cart != null)
            {
                foreach (ShoppingItem item in cart)
                {
                    total = total + (item.Product.ProductPrice * item.Quantity);
                }
                return total;
            }
            return 0;
        }

        private int isExist(int id)
        {
            List<ShoppingItem> cart = (List<ShoppingItem>)HttpContext.Current.Session["CART"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.ProductID.Equals(id))
                    return i;
            return -1;
        }

        public bool UpdateShippingInfo(string username, Order order)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //get user's cart with status not checkout
                    var orderID = db.Orders
                        .Where(o => o.CustomerID.Equals(username) && o.OrderStatus == 1)
                        .Select(o => o.OrderID)
                        .FirstOrDefault();

                    Order updateOrder = db.Orders.Find(orderID);
                    db.Orders.Attach(updateOrder);
                    updateOrder.ShipName = order.ShipName;
                    updateOrder.ShipPhone = order.ShipPhone;
                    updateOrder.ShipCity = order.ShipCity;
                    updateOrder.ShipDistrict = order.ShipDistrict;
                    updateOrder.ShipWard = order.ShipWard;
                    updateOrder.ShipStreet = order.ShipStreet;
                    updateOrder.ShippAddressNumber = order.ShippAddressNumber;
                    updateOrder.ShipNote = order.ShipNote;
                    updateOrder.OrderDate = DateTime.Now;
                    updateOrder.OrderStatus = 2;
                    db.SaveChanges();

                    var items = db.OrderDetails
                        .Where(d => d.OrderID == orderID)
                        .Select(d => new { d.WatchID, d.Quantity })
                        .ToList();

                    foreach (var item in items)
                    {
                        Watch curItem = db.Watches.Find(item.WatchID);
                        db.Watches.Attach(curItem);
                        curItem.Quantity = curItem.Quantity - item.Quantity;
                        db.SaveChanges();
                    }

                    dbTransaction.Commit();

                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

        }

    }
}
