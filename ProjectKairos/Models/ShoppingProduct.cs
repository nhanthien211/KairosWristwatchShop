using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectKairos.Models
{
    public class ShoppingProduct
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public double ProductPrice { get; set; }
        public string ProductPhoto { get; set; }
    }
}