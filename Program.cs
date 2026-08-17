using Microsoft.EntityFrameworkCore;
using ShopEase.Data;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// DATABASE - MySQL
// =========================================

builder.Services.AddDbContext<ShopEaseDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    )
);


// =========================================
// RAZOR PAGES
// =========================================

builder.Services.AddRazorPages();
// =========================================
// SESSION
// =========================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();


// =========================================
// HTTP REQUEST PIPELINE
// =========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession(); 

app.UseAuthorization();


// =========================================
// RAZOR PAGES ROUTING
// =========================================

app.MapRazorPages();

app.Run();