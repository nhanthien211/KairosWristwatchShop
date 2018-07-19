using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectKairos.ViewModel
{
    public class OrderTableViewModel
    {
        public int OrderId { get; set; }
        public string Customer { get; set; }
        public DateTime? OrderDate { get; set; }
        public double Total { get; set; }
        public string Status { get; set; }


    }
}