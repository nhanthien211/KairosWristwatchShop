using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ProjectKairos.Utilities;
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
                .Where(o => o.OrderStatus != (int)Enumeration.OrderStatus.InCart)
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
                int search;
                try
                {
                    search = Convert.ToInt32(searchValue);
                }
                catch (FormatException)
                {
                    search = 0;
                }
                order = order
                    .Where(o => o.OrderId == search || o.Customer.Contains(searchValue));
            }
            //total number of rows count   
            recordsTotal = order.Count();
            return order.Skip(skip).Take(pageSize).ToList();
        }

        public bool IsValidOrderId(int orderId)
        {
            return db.Orders.Any(o => o.OrderID == orderId && o.OrderStatus != (int)Enumeration.OrderStatus.InCart);
        }

        public AdminOrderDetailViewModel LoadOrderDetailAdmin(int orderId)
        {
            var viewModel = db.Orders
                .Include(o => o.Ward)
                .Include(o => o.District)
                .Include(o => o.City)
                .Where(o => o.OrderID == orderId)
                .Select(o => new AdminOrderDetailViewModel
                {
                    OrderId = o.OrderID,
                    Customer = o.CustomerID,
                    Receiver = o.ShipName,
                    Phone = o.ShipPhone,
                    OrderDate = o.OrderDate,
                    Address = o.ShippAddressNumber + " " + o.ShipStreet + ", "
                              + o.Ward.Type + " " + o.Ward.WardName + ", "
                              + o.District.Type + " " + o.District.DistrictName + ", "
                              + o.City.Type + " " + o.City.CityName,
                    ShipNote = o.ShipNote,
                    OrderStatus = o.OrderStatus
                })
                .FirstOrDefault();
            return viewModel;
        }

        public OrderDetailViewModel LoadOrderDetailUser(int orderId)
        {
            var viewModel = db.Orders
                .Include(o => o.OrderStatu)
                .Include(o => o.Ward)
                .Include(o => o.District)
                .Include(o => o.City)
                .Where(o => o.OrderID == orderId)
                .Select(o => new OrderDetailViewModel
                {
                    OrderId = o.OrderID,
                    Receiver = o.ShipName,
                    Phone = o.ShipPhone,
                    OrderDate = o.OrderDate,
                    Address = o.ShippAddressNumber + " " + o.ShipStreet + ", "
                              + o.Ward.Type + " " + o.Ward.WardName + ", "
                              + o.District.Type + " " + o.District.DistrictName + ", "
                              + o.City.Type + " " + o.City.CityName,
                    ShipNote = o.ShipNote,
                    Status = o.OrderStatu.StatusDescription
                })
                .FirstOrDefault();
            return viewModel;
        }

        public List<OrderTableViewModel> LoadAllCustomerOrder(string username)
        {
            var order = db.Orders
                .Include(o => o.OrderStatu)
                .Where(o => o.OrderStatus != (int)Enumeration.OrderStatus.InCart && o.CustomerID.Equals(username))
                .Select(o => new OrderTableViewModel
                {
                    OrderId = o.OrderID,
                    Status = o.OrderStatu.StatusDescription,
                    Customer = username,
                    OrderDate = o.OrderDate,
                    Total = o.OrderDetails.Sum(d => (d.Watch.Price * (1 - d.Watch.Discount * 0.01)) * d.Quantity)
                });
            return order.ToList();
        }

        public bool AdminUpdateOrderStatus(int orderId, int status)
        {
            var order = db.Orders.Find(orderId);
            db.Orders.Attach(order);

            order.OrderStatus = status;

            int result = db.SaveChanges();
            return (result > 0 || db.Entry(order).State == EntityState.Unchanged);
        }
    }
}
