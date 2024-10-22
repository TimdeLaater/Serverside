using Application.Interfaces;
using Domain.Models;
using DotNetEnv;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpelavondenWebService.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
builder.Configuration.AddEnvironmentVariables();
var appDbConnectionString = Environment.GetEnvironmentVariable("APPDB_CONNECTION_STRING");
var identityDbConnectionString = Environment.GetEnvironmentVariable("IDENTITYDB_CONNECTION_STRING");

// Add Swagger for API documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Spelavonden API", Version = "v1" });
});

// Add DbContext service for the AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(appDbConnectionString));


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

// Identity configuration without JWT


// Add Identity API endpoints middleware
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<IdentityAppDbContext>();

// Add Authorization middleware
builder.Services.AddAuthorization();
// Add hotchocolate GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();
// Add Repositories (DI)
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IBoardGameRepository, BoardGameRepository>();
builder.Services.AddScoped<IBoardGameNightRepository, BoardGameNightRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.MapGraphQL();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable the authorization middleware
app.UseAuthorization();

// Routes to activate Identity API Endpoints
app.MapIdentityApi<ApplicationUser>();

// Map Controllers
app.MapControllers();

app.Run();
