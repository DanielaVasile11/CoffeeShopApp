using System.ComponentModel.DataAnnotations;

namespace CoffeeShopAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele produsului este obligatoriu.")]
        [StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Range(0.01, 1000.00, ErrorMessage = "Prețul trebuie să fie pozitiv.")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public string Category { get; set; } // Cafea, Ceai, Deserturi, etc.
    }
}