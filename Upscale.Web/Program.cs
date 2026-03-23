using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Upscale.Web.Data;
using Upscale.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────────

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// ── Pipeline ──────────────────────────────────────────────────────────────────

var app = builder.Build();

// ── Database: apply migrations + seed on startup ──────────────────────────────
// This runs automatically the first time anyone clones and runs the project.
// It is safe to call on every startup: MigrateAsync is idempotent and the seeder
// checks for existing rows before inserting.
await DbSeeder.InitialiseAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Welcome}/{id?}")
    .WithStaticAssets();

app.Run();