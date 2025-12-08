using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Online_Food_Portal.Data;
using Online_Food_Portal.Services;
using Online_Food_Portal.Models;
using Online_Food_Portal.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add secrets configuration
builder.Configuration.AddJsonFile("secrets.json",
    optional: true,
    reloadOnChange: true);

// Add services to the container.

builder.Services.AddScoped<ISecretRepository, SecretRepository>(); // Secret configuration repository

builder.Services.AddControllersWithViews(); // Support MVC
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<CustomIdentityContext>(options =>
    options.UseSqlServer(connectionString)); // Modified Identity context
builder.Services.AddDatabaseDeveloperPageExceptionFilter(); // Development use only!

//builder.Services.AddDefaultIdentity
builder.Services.AddDefaultIdentity<IdentityUserModel>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 10;

    
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<CustomIdentityContext>();

// Implement custom hashing using SHA512 + BCrypt
builder.Services.AddScoped<IPasswordHasher<IdentityUserModel>, PasswordService>();

// Lenient login settings and pathing settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    options.Cookie.Name = "FoodPortalAuthCookie";
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Database services for direct injection
builder.Services.AddScoped<IPasswordService, PasswordService>(); // Password hashing and verification
builder.Services.AddScoped<ISqlConnectionStringBuilder, SqlConnectionStringBuilder>(); // Sql Connection string builder
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); // MySql user service
builder.Services.AddScoped<IOrderService, OrderService>(); // Order service
builder.Services.AddScoped<IItemService, ItemService>(); // Item service
builder.Services.AddScoped<IModificationService, ModificationService>(); // Modification service
builder.Services.AddScoped<IStoreSettingsService, StoreSettingsService>(); // Store settings service
builder.Services.AddScoped<IKitchenService, KitchenService>(); // Kitchen Order Service
builder.Services.AddScoped<IUserService, UserService>(); // User Order Service
// Store settings service


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles();

app.UseRouting(); // Use routes

app.UseAuthentication(); // Enables authentication
app.UseAuthorization(); // Enables authorization

app.MapRazorPages(); // Map razor pages to controllers
app.MapControllers(); // Map controllers

app.MapControllerRoute( // Map default route for initial access
    name: "default",
    pattern: "{controller=Home}/{action=Home}");

app.Run();
