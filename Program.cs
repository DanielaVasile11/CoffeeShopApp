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

// --- 2. CONFIGURARE SESIUNI (NOU) ---
// Avem nevoie de aceste linii pentru a ține minte utilizatorul logat
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sesiunea expiră după 30 minute de inactivitate
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Avem nevoie de acest serviciu pentru a accesa sesiunea din Navbar (_Layout)
builder.Services.AddHttpContextAccessor();

// 3. Adaugă serviciile MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. Configurează Pipeline-ul
if (app.Environment.IsDevelopment())
{
    // Swagger/Debugging
}

app.UseHttpsRedirection();

// Activează servirea fișierelor statice (CSS, JS, Imagini)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// --- 5. ACTIVARE SESIUNI (NOU) ---
// Trebuie pus obligatoriu DUPĂ UseRouting și ÎNAINTE de MapControllerRoute
app.UseSession();

// 6. Definește rutarea MVC implicită
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();