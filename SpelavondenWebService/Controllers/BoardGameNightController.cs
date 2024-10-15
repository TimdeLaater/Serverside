using Application.Interfaces;
using Domain.Models;
using Domain.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpelavondenWebService.DTO;
using System.Security.Claims;

namespace SpelavondenWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardGameNightController : ControllerBase
    {
        private readonly IBoardGameNightRepository _boardGameNightRepository;
        private readonly IPersonRepository _personRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public BoardGameNightController(IBoardGameNightRepository boardGameNightRepository, IPersonRepository personRepository, UserManager<ApplicationUser> userManager)
        {
            _boardGameNightRepository = boardGameNightRepository;
            _personRepository = personRepository;
            _userManager = userManager;
        }

        // GET: api/boardgamenight
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardGameNightDto>>> GetAllBoardGames()
        {
            var boardGameNights = await _boardGameNightRepository.GetAllAsync();
            if (boardGameNights == null || !boardGameNights.Any())
            {
                return NotFound("No board game nights found.");
            }

            var boardGameNightDtos = boardGameNights.Select(MapToDto).ToList();
            return Ok(boardGameNightDtos);
        }

        // GET: api/boardgamenight/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardGameNightDto>> GetBoardGameNightById(int id)
        {
            var boardGameNight = await _boardGameNightRepository.GetByIdAsync(id);

            if (boardGameNight == null)
            {
                return NotFound();
            }

            var boardGameNightDto = MapToDto(boardGameNight);
            return Ok(boardGameNightDto);
        }
        // POST: api/boardgamenight/{id}/signup
        [Authorize]
        [HttpPost("{id}/signup")]
        public async Task<IActionResult> SignUp(int id)
        {
            // Retrieve the board game night
            var boardGameNight = await _boardGameNightRepository.GetByIdAsync(id);
            if (boardGameNight == null)
            {
                return NotFound("Board game night not found.");
            }

            // Extract user ID from the JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            // Use UserManager to find the user by their ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Retrieve the PersonId from ApplicationUser
            if (user.PersonId == null)
            {
                return NotFound("Associated person for this user not found.");
            }

            // Retrieve the person associated with this user
            var person = await _personRepository.GetByIdAsync(user.PersonId);
            if (person == null)
            {
                return NotFound("Person not found.");
            }

            // Validate the participant using the validation service
            var validationService = new BoardGameNightValidationService();
            var validationResult = validationService.ValidateParticipant(person, boardGameNight);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            
            await _boardGameNightRepository.AddParticipant(id, person);
         

            return Ok("You have successfully signed up for the board game night.");
        }


        // Mapping method to convert BoardGameNight to BoardGameNightDto
        private BoardGameNightDto MapToDto(BoardGameNight boardGameNight)
        {
            return new BoardGameNightDto
            {
                BoardGameNightId = boardGameNight.BoardGameNightId,
                OrganizerId = boardGameNight.OrganizerId,
                OrganizerName = boardGameNight.Organizer?.Name ?? "Unknown Organizer",  // Handle null organizer
                MaxPlayers = boardGameNight.MaxPlayers,
                Date = boardGameNight.Date,
                Is18Plus = boardGameNight.Is18Plus,
                Address = boardGameNight.Address,
                Participants = boardGameNight.Participants?.Select(p => new ParticipantDto
                {
                    ParticipantId = p.PersonId,
                    Name = p.Name
                }).ToList() ?? new List<ParticipantDto>(),  // Ensure the list is never null
                BoardGames = boardGameNight.BoardGames?.Select(bg => new BoardGameDto
                {
                    Id = bg.BoardGameId,
                    Name = bg.Name,
                    Genre = bg.Genre,
                    GameType = bg.GameType,
                    Is18Plus = bg.Is18Plus
                }).ToList() ?? new List<BoardGameDto>(),  // Ensure the list is never null
                FoodOptions = boardGameNight.FoodOptions ?? new List<DietaryPreference>(),  // Ensure the list is never null
                Reviews = boardGameNight.Reviews?.Where(r => r.Reviewer != null)  // Check if Reviewer is not null
                                .Select(r => new ReviewDto
                                {
                                    ReviewerName = r.Reviewer.Name ?? "Unknown Reviewer",  // Handle null reviewer name
                                    Rating = r.Rating,
                                    ReviewText = r.ReviewText
                                }).ToList() ?? new List<ReviewDto>()  // Ensure the list is never null
            };
        }

    }
}
