using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectKairos.Utilities;

namespace ProjectKairos.Models
{
    public class OrderStatusService
    {
        private KAIROS_SHOPEntities db;

        public OrderStatusService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public List<OrderStatu> GetAllOrderStatusExceptCart()
        {
            return db.OrderStatus.Where(s => s.StatusID != (int)Enumeration.OrderStatus.InCart).ToList();
        }
    }
}