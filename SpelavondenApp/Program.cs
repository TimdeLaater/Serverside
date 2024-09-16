using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Microsoft.AspNetCore.Identity; // Assuming your DbContext is in Infrastructure namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext service
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Read the connection string from appsettings.json

//Add Identity
builder.Services.AddDbContext<IdentityAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"))); // Read the connection string from appsettings.json
                                                                                            // Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityAppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // identety middleware

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
