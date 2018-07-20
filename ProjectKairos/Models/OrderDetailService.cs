using System.Linq;
using ProjectKairos.ViewModel;

namespace ProjectKairos.Models
{
    public class OrderDetailService
    {
        private KAIROS_SHOPEntities db;


        public OrderDetailService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public OrderItemViewModel LoadAllItemInOrder(int orderId)
        {

            var result = db.OrderDetails.Where(d => d.OrderID == 1001)
                .Select(d => new
                {
                    Product = d.Watch.WatchCode,
                    Price = d.Watch.Price * (1 - d.Watch.Discount * 0.01),
                    Quantity = d.Quantity,
                });
            return null;
        }
    }
}