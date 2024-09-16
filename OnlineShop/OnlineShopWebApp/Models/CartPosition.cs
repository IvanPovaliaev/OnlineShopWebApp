namespace OnlineShopWebApp.Models
{
    public class CartPosition
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Cost
        {
            get => Product.Cost * Quantity;
        }

        public CartPosition(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
