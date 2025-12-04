using CoffeeShopAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURARE BAZĂ DE DATE ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (connectionString != null)
    {
        options.UseSqlServer(connectionString);
    }
});


// 2. Adaugă serviciile MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. Configurează Pipeline-ul
if (app.Environment.IsDevelopment())
{
    // Swagger/Debugging (Pot fi dezactivate în final)
}

app.UseHttpsRedirection();
// Activează servirea fișierelor statice din wwwroot (CSS, JS, Imagini)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 4. Definește rutarea MVC implicită (Home/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();