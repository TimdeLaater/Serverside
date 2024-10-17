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

// Print the connection strings to the console for debugging
Console.WriteLine($"APPDB_CONNECTION_STRING: {appDbConnectionString}");
Console.WriteLine($"IDENTITYDB_CONNECTION_STRING: {identityDbConnectionString}");

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
