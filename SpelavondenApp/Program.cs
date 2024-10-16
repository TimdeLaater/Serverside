using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables directly from the system (no .env file)
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllersWithViews();

// Read connection string from environment variables
var connectionString = Environment.GetEnvironmentVariable("APPDB_CONNECTION_STRING");


//// Add DbContext service
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(connectionString)); // Using environment variable directly

//// Add Identity
//var identityConnectionString = Environment.GetEnvironmentVariable("IDENTITYDB_CONNECTION_STRING");
//builder.Services.AddDbContext<IdentityAppDbContext>(options =>
//    options.UseSqlServer(identityConnectionString)); // Using environment variable for Identity connection

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        Environment.GetEnvironmentVariable("APPDB_CONNECTION_STRING"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Number of retries
                maxRetryDelay: TimeSpan.FromSeconds(30), // Delay between retries
                errorNumbersToAdd: null // SQL error codes to retry on (null means retry on all transient errors)
            );
        })
);

builder.Services.AddDbContext<IdentityAppDbContext>(options =>
    options.UseSqlServer(
        Environment.GetEnvironmentVariable("IDENTITYDB_CONNECTION_STRING"),
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
