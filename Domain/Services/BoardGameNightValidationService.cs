using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class BoardGameNightValidationService {        
        public ValidationResult ValidateBoardGameNight(BoardGameNight boardGameNight)
        {
            var result = new ValidationResult();

            // Ensure organizer is at least 18 years old
            var organizerAge = DateTime.Now.Year - boardGameNight.Organizer.BirthDate.Year;
            if (boardGameNight.Organizer.BirthDate > DateTime.Now.AddYears(-organizerAge)) organizerAge--;
            if (organizerAge < 18)
            {
                result.AddError("Organizer must be at least 18 years old.");
            }

            // Ensure board game night date is valid
            if (boardGameNight.Date < DateTime.Now)
            {
                result.AddError("Board game night date cannot be in the past.");
            }

            // Ensure max players is greater than 0
            if (boardGameNight.MaxPlayers <= 0)
            {
                result.AddError("Max players must be greater than 0.");
            }

            return result;
        }
    }   

}
