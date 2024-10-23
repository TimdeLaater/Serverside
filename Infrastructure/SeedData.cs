using Domain.Models;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        // Create Admin role and Admin user
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var adminUser = await userManager.FindByEmailAsync("admin@domain.com");
        var adminPerson = new Person
        {
            Name = "Admin User",
            Email = "admin@domain.com",
            BirthDate = new DateTime(1980, 1, 1),
            Address = new Address { Street = "Admin Street", City = "Admin City", HouseNumber = "123" },
            DietaryPreferences = new List<DietaryPreference> { DietaryPreference.NoPreference }
        };
        if (adminUser == null)
        {

            dbContext.Persons.Add(adminPerson);
            await dbContext.SaveChangesAsync();

            adminUser = new ApplicationUser
            {
                UserName = "admin@domain.com",
                Email = "admin@domain.com",
                EmailConfirmed = true,
                PersonId = adminPerson.PersonId,
                Person = adminPerson
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create additional participants
        var participant1 = new Person
        {
            Name = "Participant One",
            Email = "participant1@domain.com",
            BirthDate = new DateTime(1995, 5, 15),
            Address = new Address { Street = "Street One", City = "City One", HouseNumber = "111" },
            DietaryPreferences = new List<DietaryPreference> { DietaryPreference.Vegetarian }
        };

        var participant2 = new Person
        {
            Name = "Participant Two",
            Email = "participant2@domain.com",
            BirthDate = new DateTime(1993, 4, 12),
            Address = new Address { Street = "Street Two", City = "City Two", HouseNumber = "222" },
            DietaryPreferences = new List<DietaryPreference> { DietaryPreference.LactoseFree }
        };

        dbContext.Persons.AddRange(participant1, participant2);
        await dbContext.SaveChangesAsync();

        // Create board games
        var boardGame1 = new BoardGame { Name = "Catan", Is18Plus = false, Description = "The game of Catan" };
        var boardGame2 = new BoardGame { Name = "Pandemic", Is18Plus = false, Description = "The game of Pandemic" };
        var boardGame3 = new BoardGame { Name = "Gloomhaven", Is18Plus = true , Description = "The adult game of Gloomhaven" };

        dbContext.BoardGames.AddRange(boardGame1, boardGame2, boardGame3);
        await dbContext.SaveChangesAsync();

        // Create board game nights
        var boardGameNight1 = new BoardGameNight
        {
            OrganizerId = adminUser.PersonId,
            Organizer = adminPerson,
            DateTime = DateTime.Now.AddMonths(-1), // One month in the past
            MaxPlayers = 5,
            Is18Plus = false,
            Address = new Address { Street = "Night One Street", City = "Night One City", HouseNumber = "123" },
            BoardGames = new List<BoardGame> { boardGame1, boardGame2 },
            Participants = new List<Person> { participant1, participant2 }
        };

        var boardGameNight2 = new BoardGameNight
        {
            OrganizerId = participant1.PersonId,
            Organizer = participant1,
            DateTime = DateTime.Now.AddMonths(-2), // Two months in the past
            MaxPlayers = 6,
            Is18Plus = true,
            Address = new Address { Street = "Night Two Street", City = "Night Two City", HouseNumber = "456" },
            BoardGames = new List<BoardGame> { boardGame3 },
            Participants = new List<Person> { adminPerson, participant2 }
        };

        dbContext.BoardGameNights.AddRange(boardGameNight1, boardGameNight2);
        await dbContext.SaveChangesAsync();
    }
}
