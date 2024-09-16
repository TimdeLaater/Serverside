using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class IdentityAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
    : base(options)
        {
        }
    }
}
