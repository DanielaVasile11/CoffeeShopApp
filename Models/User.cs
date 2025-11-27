// User.cs
namespace CoffeeShopAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // "Customer" sau "Admin" [cite: 3]
    }
}