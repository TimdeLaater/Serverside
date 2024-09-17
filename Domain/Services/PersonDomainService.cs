using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class PersonDomainService
    {
        public void ValidatePersonForRegistration(Person person)
        {
            if (CalculateAge(person.BirthDate) < 16)
            {
                throw new InvalidOperationException("Person must be at least 16 years old to register.");
            }
        }

        // Helper method to calculate age
        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--; 
            return age;
        }
    }
}
