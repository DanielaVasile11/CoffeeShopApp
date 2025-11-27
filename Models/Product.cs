// Product.cs
namespace CoffeeShopAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // Cafea, Ceai, Deserturi, etc.
    }
}