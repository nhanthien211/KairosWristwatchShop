using System.Collections.Generic;
using ProjectKairos.Models;

namespace ProjectKairos.ViewModel
{
    public class ShoppingCartViewModel
    {
        public ShoppingCartViewModel()
        {

        }

        public ShoppingCartViewModel(List<ShoppingItem> CartProduct)
        {
            this.CartProduct = CartProduct;
            this.CalculateTotal();
        }

        public List<ShoppingItem> CartProduct { get; set; }
        public double CartTotal { get; set; }
        public Dictionary<int, int> IdAndError { get; set; }


        public void CalculateTotal()
        {
            double total = 0;

            if (CartProduct == null)
            {
                this.CartTotal = 0;
                return;
            }

            foreach (ShoppingItem item in CartProduct)
            {
                total = total + (item.Quantity * item.Product.ProductPrice);
            }
            this.CartTotal = total;
        }

    }
}
