namespace ProjectKairos.Models
{
    public class ShoppingProduct
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public double ProductDiscount { get; set; }
        public string ProductPhoto { get; set; }

        private double productPrice;

        public double ProductPrice
        {
            get { return productPrice * (1 - ProductDiscount * 0.01); }
            set { productPrice = value; }
        }

    }
}