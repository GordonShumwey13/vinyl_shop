
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;

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
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Albums}/{action=Index}/{id?}");

app.Run();
