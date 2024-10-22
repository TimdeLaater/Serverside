using Application.Interfaces;
using Domain.Models;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BoardGameNightValidationService : DomainBoardGameNightValidationService
    {
        private readonly IBoardGameNightRepository _boardGameNightRepository;

        public BoardGameNightValidationService(IBoardGameNightRepository boardGameNightRepository)
        {
            _boardGameNightRepository = boardGameNightRepository;
        }


        public async Task<ValidationResult> ValidateSignUpAsync(int personId, BoardGameNight boardGameNight)
        {
            var result = new ValidationResult();

            // Check if the person is already signed up for a game night on the same day
            var existingGameNightForDay = await _boardGameNightRepository.GetByPersonAndDateAsync(personId, boardGameNight.DateTime.Date);
            if (existingGameNightForDay != null)
            {
                result.AddError("You are already signed up for a game night on this day.");
            }

            return result;
        }
    }

}
