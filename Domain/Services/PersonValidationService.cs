using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class PersonValidationService
    {
        public ValidationResult ValidatePerson(Person person)
        {
            var result = new ValidationResult();

            // Ensure birthdate is not in the future
            if (person.BirthDate > DateTime.Now)
            {
                result.AddError("Birthdate cannot be in the future.");
            }

            // Ensure person is at least 16 years old
            var age = DateTime.Now.Year - person.BirthDate.Year;
            if (person.BirthDate > DateTime.Now.AddYears(-age)) age--; // Adjust if birthday hasn't occurred yet this year
            if (age < 16)
            {
                result.AddError("Person must be at least 16 years old to register.");
            }

            return result;
        }
    }

}
