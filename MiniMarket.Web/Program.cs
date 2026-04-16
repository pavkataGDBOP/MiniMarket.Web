using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Services;
using System.Globalization;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// 🔥 ТУК трябва да е (преди Build)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddSession();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddRazorPages();


var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var user = await userManager.FindByEmailAsync("admin@test.com");

    if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
    {
        await userManager.AddToRoleAsync(user, "Admin");
    }

}


// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication(); // 🔥 липсваше
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("APP STARTED");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Categories.Any())
    {
        context.Categories.Add(new Category { Name = "Electronics" });
        context.Categories.Add(new Category { Name = "Clothes" });
        context.Categories.Add(new Category { Name = "Books" });

        context.SaveChanges();
    }
}
var culture = new CultureInfo("bg-BG");

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
};

app.UseRequestLocalization(localizationOptions);

app.MapRazorPages();
app.Run();