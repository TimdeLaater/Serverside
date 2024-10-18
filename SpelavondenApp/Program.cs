using Application.Interfaces;
using Domain.Models;
using Infrastructure.Repositories;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables directly from the system (no .env file)
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllersWithViews();

// Read connection string from environment variables
var appDbConnectionString = Environment.GetEnvironmentVariable("APPDB_CONNECTION_STRING");
var identityDbConnectionString = Environment.GetEnvironmentVariable("IDENTITYDB_CONNECTION_STRING");


// Add DbContext service for the AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        appDbConnectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Number of retries
                maxRetryDelay: TimeSpan.FromSeconds(30), // Delay between retries
                errorNumbersToAdd: null // SQL error codes to retry on (null means retry on all transient errors)
            );
        })
);

// Add DbContext service for the IdentityAppDbContext
builder.Services.AddDbContext<IdentityAppDbContext>(options =>
    options.UseSqlServer(
        identityDbConnectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        })
);

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityAppDbContext>()
    .AddDefaultTokenProviders();

// Add Repositories DI
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IBoardGameRepository, BoardGameRepository>();
builder.Services.AddScoped<IBoardGameNightRepository, BoardGameNightRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

var app = builder.Build();

// Seed Admin User
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await SeedAdminUser(services);
//}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();

//async Task SeedAdminUser(IServiceProvider serviceProvider)
//{
//    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    // Admin rol aanmaken
//    if (!await roleManager.RoleExistsAsync("Admin"))
//    {
//        await roleManager.CreateAsync(new IdentityRole("Admin"));
//    }

//    // Admin-gebruiker aanmaken
//    if (await userManager.FindByEmailAsync("admin@avans.com") == null)
//    {
//        var adminUser = new ApplicationUser
//        {
//            UserName = "admin@avans.com",
//            Email = "admin@avans.com",
//            PersonId = 1  // Verwijzing naar de seeded persoon AvansDocent
//        };
//        await userManager.CreateAsync(adminUser, "AdminPassword123!");  // Verander dit naar een sterker wachtwoord

//        // Admin-gebruiker de adminrol geven
//        await userManager.AddToRoleAsync(adminUser, "Admin");
//    }
//}
