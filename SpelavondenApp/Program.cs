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
                maxRetryCount: 5, 
                maxRetryDelay: TimeSpan.FromSeconds(30), 
                errorNumbersToAdd: null // SQL error codes to retry on (null means retry on all transient errors)
            );
            sqlOptions.CommandTimeout(60); 
        })
    .UseSqlServer(appDbConnectionString, optionsBuilder =>
    {
        optionsBuilder.CommandTimeout(60); 
    }));

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
            sqlOptions.CommandTimeout(60); // Set command timeout to 60 seconds
        })
    .UseSqlServer(identityDbConnectionString, optionsBuilder =>
    {
        optionsBuilder.CommandTimeout(60); // Command timeout of 60 seconds for individual SQL commands
    }));

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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedData.Initialize(services, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

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
