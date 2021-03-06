﻿using System;

namespace ProjectKairos.ViewModel
{
    public class OrderTableViewModel
    {
        public int OrderId { get; set; }
        public string Customer { get; set; }

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

        public double Total { get; set; }
        public string Status { get; set; }



    }
}
