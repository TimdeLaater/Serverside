using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(string userId);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user);
        Task UpdateUserAsync(ApplicationUser user);
        Task DeleteUserAsync(ApplicationUser user);
    }
}
