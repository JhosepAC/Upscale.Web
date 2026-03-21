using Microsoft.EntityFrameworkCore;
using Upscale.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// --- SECTION OF SERVICE CONFIGURATION (Dependency Injection) ---

builder.Services.AddControllersWithViews();

// Configuration of the database context with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- SECTION OF MIDDLEWARE CONFIGURATION (HTTP Request Pipeline) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Welcome}/{id?}")
    .WithStaticAssets();

app.Run();