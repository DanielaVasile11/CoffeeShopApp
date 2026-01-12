using System;
using System.Collections.Generic;

namespace CoffeeShopAPI.Models
{
    public class Order
    {
        public string OrderId { get; set; }       // ID unic al comenzii (ex: #CMD-1234)
        public string Username { get; set; }      // Cine a făcut comanda
        public string CustomerName { get; set; }  // Numele din formularul de checkout
        public DateTime OrderDate { get; set; }   // Data și ora
        public decimal TotalAmount { get; set; }  // Total de plată
        public string Address { get; set; }       // Adresa de livrare

        // Lista produselor din acea comandă
        public List<Product> OrderedItems { get; set; } = new List<Product>();
    }
}