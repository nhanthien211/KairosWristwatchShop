using System;
using System.Collections.Generic;

namespace ProjectKairos.ViewModel
{
    public class OrderDetailViewModel
    {
        public List<OrderItemViewModel> OrderItem { get; set; }
        public int OrderId { get; set; }
        public string Receiver { get; set; }
        public string Phone { get; set; }

        public string Date { get; set; }
        private DateTime? orderDate;
        public DateTime? OrderDate
        {
            get => orderDate;
            set
            {
                orderDate = value;
                Date = orderDate.GetValueOrDefault().ToString("yyyy-MM-dd");
            }
        }

        public string Address { get; set; }
        public string ShipNote { get; set; }
        public string Status { get; set; }
        public int TotalPrice { get; set; }

    }
}