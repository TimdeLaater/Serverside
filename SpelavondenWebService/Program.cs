using Application.Interfaces;
using Domain.Models;
using DotNetEnv;
using GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpelavondenWebService.GraphQL;
using SpelavondenWebService.GraphQL.Queries;
using SpelavondenWebService.GraphQL.Types;

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
    options.UseSqlServer(
        appDbConnectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
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

// Identity configuration without JWT
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<IdentityAppDbContext>();

// Add Authorization middleware
builder.Services.AddAuthorization();

// Add Repositories (DI)
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IBoardGameRepository, BoardGameRepository>();
builder.Services.AddScoped<IBoardGameNightRepository, BoardGameNightRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Add Controllers
builder.Services.AddControllers();

// Voeg GraphQL services en types toe
builder.Services.AddGraphQL().AddSystemTextJson()  // Gebruik System.Text.Json voor serialisatie
.AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)  // Handige foutmeldingen
.AddGraphTypes(typeof(AppSchema).Assembly);  // Voeg je schema toe
// Voeg schema en query's toe aan de DI-container
builder.Services.AddSingleton<AppSchema>();
builder.Services.AddSingleton<BoardGameNightQuery>();
builder.Services.AddSingleton<BoardGameQuery>();
builder.Services.AddSingleton<IBoardGameNightRepository, BoardGameNightRepository>();
builder.Services.AddSingleton<IBoardGameRepository, BoardGameRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGraphQL<AppSchema>();  // Maak je schema beschikbaar via GraphQL
app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
{
    Path = "/ui/playground"  // URL voor GraphQL Playground
});

app.UseHttpsRedirection();

// Enable the authorization middleware
app.UseAuthorization();

// Routes to activate Identity API Endpoints
app.MapIdentityApi<ApplicationUser>();

// Map Controllers
app.MapControllers();

app.Run();
