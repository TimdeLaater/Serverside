using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class ReviewValidationService
    {
        public ValidationResult ValidateReview(BoardGameNight boardGameNight, Person person)
        { 
            var result = new ValidationResult();
            //Check if the person is participating in the board game night
            if (boardGameNight.Participants == null || !boardGameNight.Participants.Any(p => p.PersonId == person.PersonId))
            {
                result.AddError("Person is not participating in this board game night.");
            }
            //Check if the boardgame has already happend
            if (boardGameNight.Date > DateTime.Now)
            {
                result.AddError("Board game night has not yet happened.");
            }
            //Check if the person has already reviewed the boardgame
            if (boardGameNight.Reviews != null && boardGameNight.Reviews.Any(r => r.ReviewerId == person.PersonId))
            {
                result.AddError("Person has already reviewed this board game night.");
            }
            return result;
        }   
        
    }
}
