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
            // Ensure if boardgames is 18+ then boardgame night is 18+
            if (boardGameNight.BoardGames.Any(b => b.Is18Plus) && !boardGameNight.Is18Plus)
            {
                result.AddError("Board game night must be 18+ if any of the board games are 18+.");
            }



            return result;
        }
        public ValidationResult ValidateParticipant(Person participant, BoardGameNight boardGameNight)
        {
            var result = new ValidationResult();

            // Ensure participant is at least 16 years old
            var age = DateTime.Now.Year - participant.BirthDate.Year;
            if (participant.BirthDate > DateTime.Now.AddYears(-age)) age--; // Adjust if birthday hasn't occurred yet this year
            if (age < 16)
            {
                result.AddError("Person must be at least 16 years old to participate.");
            }

            // Ensure participant is not already participating
            if (boardGameNight.Participants != null && boardGameNight.Participants.Any(p => p.PersonId == participant.PersonId))
            {
                result.AddError("Person is already participating in this board game night.");
            }

            // Ensure participant is not the organizer
            if (boardGameNight.Organizer.PersonId == participant.PersonId)
            {
                result.AddError("Organizer cannot participate in their own board game night.");
            }
            // Ensure participant is 18+ if board game night is 18+
            if (boardGameNight.Is18Plus && age < 18)
            {
                result.AddError("Person must be at least 18 years old to participate in this board game night.");
            }
            if (boardGameNight.Participants.Count >= boardGameNight.MaxPlayers)
            {
                result.AddError("Cannot add more participants, the maximum number of players has been reached.");
            }
            return result;


        }
        public List<string> CheckDietaryWarnings(Person person, BoardGameNight gameNight)
        {
            var warnings = new List<string>();

            // Ga door de dieetvoorkeuren van de persoon
            foreach (var preference in person.DietaryPreferences)
            {
                if (preference != DietaryPreference.NoPreference)
                {
                    // Controleer of een van de voedselopties overeenkomt met de dieetvoorkeuren
                    if (!gameNight.FoodOptions.Contains(preference))
                    {
                        warnings.Add($"Warning: No food options match your dietary preference: {preference}.");
                    }
                }
            }

            return warnings;
        }
    }   

}
