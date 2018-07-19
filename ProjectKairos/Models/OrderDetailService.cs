using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ProjectKairos.Models
{
    public class OrderDetailService
    {
        private KAIROS_SHOPEntities db;


        public OrderDetailService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public double GetTotalPriceOfOrder(int orderId)
        {
            var priceList = db.OrderDetails
                .Include(d => d.Watch)
                .Where(d => d.OrderID == orderId)
                .Select(d => d.Watch.Price * (1 - d.Watch.Discount * 0.01));

            return 1;
        }
    }
}