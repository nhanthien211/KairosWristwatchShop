using System.Collections.Generic;
using ProjectKairos.Models;

namespace ProjectKairos.ViewModel
{
    public class AdminOrderDetailViewModel : OrderDetailViewModel
    {
        public string Customer { get; set; }
        
        public List<OrderStatu> OrderStatusList { get; set; }
    }
}