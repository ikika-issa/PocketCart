using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PocketCartApp.Domain.Email;
using PocketCartApp.Domain.Identity_Models;
using PocketCartApp.Repository;
using PocketCartApp.Repository.Implementation;
using PocketCartApp.Repository.Interface;
using PocketCartApp.Repository.Seed;
using PocketCartApp.Service.API.Implementation;
using PocketCartApp.Service.API.Interface;
using PocketCartApp.Service.Implementation;
using PocketCartApp.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (connectionString.Contains(".db") || (connectionString.Contains("Data Source=") && !connectionString.Contains("Server=") && !connectionString.Contains("localdb")))
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<PocketCartApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
});

builder.Services.AddHttpClient<IOpenFoodFactsService, OpenFoodFactsService>(client =>
{
    client.BaseAddress = new Uri("https://world.openfoodfacts.org/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "PocketCartApp/1.0 (contact: georgievskaeva@gmail.com)"
    );
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IReceiptService, ReceiptingService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<ICategoryAPIService, CategoryAPIService>();
builder.Services.AddTransient<IManufacturerService, ManufacturerService>();

builder.Services.AddScoped<IBarcodeService, BarcodeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailSender, EmailService>();

builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSettings"));

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await SeedData.SeedRoles(services);
    await SeedData.SeedAdmin(services);

    // Auto-import sample products if database is empty
    var productService = services.GetRequiredService<IProductService>();
    if (!productService.GetAll().Any())
    {
        var productImportService = services.GetRequiredService<IOpenFoodFactsService>();
        var env = services.GetRequiredService<IWebHostEnvironment>();
        await productImportService.ImportSampleProductsAsync(env.WebRootPath);
    }
}

app.Run();