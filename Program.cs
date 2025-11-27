using CoffeeShopAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer; // NOU: Folosim provider-ul SQL Server

var builder = WebApplication.CreateBuilder(args);

// --- SECȚIUNE ADAUGATĂ PENTRU CONTEXTUL BAZEI DE DATE ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (connectionString != null)
    {
        // NOU: Folosim UseSqlServer()
        options.UseSqlServer(connectionString);
    }
});
// ------------------------------------------------------------


// NOU: Adaugă serviciile MVC pentru Controller-e și Views
builder.Services.AddControllersWithViews();


// Opțional: Poți șterge liniile de Swagger/API Explorer dacă nu le mai folosești în MVC
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Opțional: Poți șterge liniile de Swagger dacă nu le mai folosești
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// NOU: Asigură-te că fișierele statice (CSS, JS, Imagini din wwwroot) sunt servite
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// NOU: Definește rutarea MVC implicită
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ȘTERGE: app.MapControllers(); (Aceasta era pentru API)

app.Run();