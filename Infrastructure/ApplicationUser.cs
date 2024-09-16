using System;
using System.Collections.Generic;
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
        public virtual Person Person { get; set; } // Navigation property to Person
    }
}   

