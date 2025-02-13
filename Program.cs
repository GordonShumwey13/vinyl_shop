using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.SeedData;

var builder = WebApplication.CreateBuilder(args);

// Підключаємо MVC
builder.Services.AddControllersWithViews();

// Підключаємо Entity Framework Core MySQL
builder.Services.AddDbContext<VinylShopContext>(options =>
    options.UseMySql(
        "server=localhost;database=vinyl_shop;user=root;password=qwerty123",
        ServerVersion.AutoDetect("server=localhost;database=vinyl_shop;user=root;password=qwerty123")
    )
);

var app = builder.Build();

// Виконуємо наповнення БД
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VinylShopContext>();
    dbContext.Database.Migrate(); // Виконує міграцію, якщо її ще не було
    DatabaseSeeder.Seed(dbContext); // Заповнює базу даними
}

// Використовуємо маршрутизацію контролерів і представлень
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Albums}/{action=Index}/{id?}");

app.Run();
