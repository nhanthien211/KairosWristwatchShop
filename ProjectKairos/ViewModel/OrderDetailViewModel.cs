using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectKairos.ViewModel
{
    public class OrderDetailViewModel
    {
        private List<OrderItemViewModel> orderItem;
        public int OrderStatus { get; set; }
        public List<OrderItemViewModel> OrderItem
        {
            get => orderItem;
            set
            {
                orderItem = value;
                TotalPrice = orderItem.Sum(o => o.Total);
            }
        }

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
        public double TotalPrice { get; set; }

    }
}