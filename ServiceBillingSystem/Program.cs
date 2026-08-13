using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceBillingSystem.Data;
using ServiceBillingSystem.Models;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Evaluation;

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddSession();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Users.Any(x => x.Role == "Admin"))
    {
        var admin = new User
        {
            Name = "Admin12",
            Email = "admin@gmail.com",
            Role = "Admin"
        };

        var passwordHasher =new PasswordHasher<User>();

        admin.Password =passwordHasher.HashPassword( admin,"Admin@123");

        context.Users.Add(admin);

        context.SaveChanges();
    }
}

app.Run();