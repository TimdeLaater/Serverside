using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Domain.Models;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public ApplicationUserRepository(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Get ApplicationUser by UserId
        public async Task<ApplicationUser> GetByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Person = await _context.Persons.FindAsync(user.PersonId);
            }
            return user;
        }

        // Get ApplicationUser by Email
        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.Person = await _context.Persons.FindAsync(user.PersonId);
            }
            return user;
        }


        // Create ApplicationUser and Person
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user)
        {
            var person = user.Person;
            // Add the Person entity first
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            // Link the PersonId to ApplicationUser
            user.PersonId = person.PersonId;

            // Add the ApplicationUser using ASP.NET Identity's UserManager
           var result= await _userManager.CreateAsync(user);
            return result;
        }

        // Update ApplicationUser and Person
        public async Task UpdateUserAsync(ApplicationUser user)
        {
            var person = user.Person;
            // Update the Person entity
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();

            // Update the ApplicationUser entity using ASP.NET Identity's UserManager
           await _userManager.UpdateAsync(user);

        }
        // Delete ApplicationUser and Person
        public async Task DeleteUserAsync(ApplicationUser user)
        {
            // Remove the associated Person entity
            var person = await _context.Persons.FindAsync(user.PersonId);
            if (person != null)
            {
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
            }

            // Remove the ApplicationUser using ASP.NET Identity's UserManager
            await _userManager.DeleteAsync(user);
        }

    }
    }