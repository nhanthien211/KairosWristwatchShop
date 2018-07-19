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

        private DateTime? orderDate;

        public DateTime? OrderDate
        {
            get { return orderDate; }
            set
            {
                orderDate = value;
                Date = orderDate.GetValueOrDefault().ToString("yyyy-MM-dd");
            }
        }

        public double Total { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }

    }
}