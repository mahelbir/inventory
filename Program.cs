using Inventory.Services;
using Microsoft.EntityFrameworkCore;
using Inventory.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<StockService>();

// Configure the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Custom HTTP Error Page
app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
