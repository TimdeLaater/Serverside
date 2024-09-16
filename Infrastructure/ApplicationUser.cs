using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        public int PersonId { get; set; } // FK to Person entity

        [NotMapped] // This prevents Entity Framework from trying to map the 'Person' property in Identity context
        public virtual Person Person { get; set; } // Navigation property to Person in AppDbContext
    }

}

