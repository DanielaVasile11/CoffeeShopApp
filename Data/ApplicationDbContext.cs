// ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using CoffeeShopAPI.Models;

namespace CoffeeShopAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Definim tabelele bazei de date
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        // Ar trebui adăugate Order și OrderItem, dar le lăsăm pentru etapa următoare.
    }
}