using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectKairos.Models
{
    public class ShoppingItem
    {
        public ShoppingItem(ShoppingProduct p, int quantity)
        {
            this.Product = p;
            this.Quantity = quantity;
            this.SubTotal = p.ProductPrice * quantity;
        }

        public ShoppingProduct Product { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }


        public void calculateSubTotal()
        {
            this.SubTotal = this.Product.ProductPrice * this.Quantity;
        }
    }
}