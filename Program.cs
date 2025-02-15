
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.SeedData;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<VinylShopContext>(options =>
    options.UseMySql(
        "server=localhost;database=vinyl_shop;user=root;password=qwerty123",
        ServerVersion.AutoDetect("server=localhost;database=vinyl_shop;user=root;password=qwerty123")
    )
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VinylShopContext>();
    dbContext.Database.Migrate(); 
    DatabaseSeeder.Seed(dbContext);
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Albums}/{action=Index}/{id?}");

app.Run();
