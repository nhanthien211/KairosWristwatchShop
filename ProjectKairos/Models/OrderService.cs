using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ProjectKairos.ViewModel;

namespace ProjectKairos.Models
{
    public class OrderService
    {
        private KAIROS_SHOPEntities db;

        public OrderService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public List<OrderTableViewModel> LoadOrderTable(string sortColumnDirection, string searchValue, ref int recordsTotal, int skip, int pageSize)
        {
            var order = db.Orders
                .Include(o => o.OrderStatu)
                .Where(o => o.OrderStatus != 1)
                .Select(o => new OrderTableViewModel
                {
                    OrderId = o.OrderID,
                    Status = o.OrderStatu.StatusDescription,
                    Customer = o.Account.Username,
                    OrderDate = o.OrderDate,
                    Total = o.OrderDetails.Sum(d => (d.Watch.Price * (1 - d.Watch.Discount * 0.01)) * d.Quantity)
                });

            //sorting
            if (!string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumnDirection)
                {
                    case "asc":
                        order = order.OrderBy(o => o.Status);
                        break;
                    case "desc":
                        order = order.OrderByDescending(o => o.Status);
                        break;
                }
            }
            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                //can search based on order id
                int search = Convert.ToInt32(searchValue);
                order = order
                    .Where(o => o.OrderId == search || o.Customer.Contains(searchValue));
            }
            //total number of rows count   
            recordsTotal = order.Count();
            return order.Skip(skip).Take(pageSize).ToList();
        }
    }
}