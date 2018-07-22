using System.Collections.Generic;
using System.Linq;
using ProjectKairos.ViewModel;
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

        public List<OrderItemViewModel> LoadAllItemInOrder(int orderId)
        {

            var result = db.OrderDetails
                .Include(d => d.Watch)
                .Where(d => d.OrderID == orderId)
                .Select(d => new OrderItemViewModel
                {
                    WatchCode = d.Watch.WatchCode,
                    Price = d.Watch.Price * (1 - d.Watch.Discount * 0.01),
                    Quantity = d.Quantity,
                    Total = d.Watch.Price * (1 - d.Watch.Discount * 0.01) * d.Quantity,
                    WatchId = d.WatchID
                }).ToList();
            return result;
        }
    }
}