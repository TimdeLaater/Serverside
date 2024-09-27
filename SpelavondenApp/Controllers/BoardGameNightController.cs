using Application.Interfaces;
using Domain.Models;
using Domain.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpelavondenApp.Models.BoardGameNight;
using System.Threading.Tasks;

namespace SpelavondenApp.Controllers
{
    public class BoardGameNightController : BaseController
    {
        private readonly IBoardGameNightRepository _boardGameNightRepository;
        private readonly IPersonRepository _personRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBoardGameRepository _boardGameRepository;
        private readonly BoardGameNightValidationService _boardGameNightValidator = new BoardGameNightValidationService();

        public BoardGameNightController(IBoardGameNightRepository boardGameNightRepository, IPersonRepository personRepository, UserManager<ApplicationUser> userManager, IBoardGameRepository boardGameRepository)
        {
            _boardGameNightRepository = boardGameNightRepository;
            _personRepository = personRepository;
            _userManager = userManager;
            _boardGameRepository = boardGameRepository;
        }

        // This action does not require authentication, so no [Authorize]
        public async Task<IActionResult> Index()
        {
            var boardGameNights = await _boardGameNightRepository.GetAllAsync();
            var sortedBoardGameNights = boardGameNights.OrderBy(bgn => bgn.Date);

            var viewModel = sortedBoardGameNights.Select(bgn => new BoardGameNightIndexViewModel
            {
                Id = bgn.BoardGameNightId,
                Date = bgn.Date,
                OrganizerName = bgn.Organizer.Name,
                ParticipantCount = bgn.Participants?.Count() ?? 0,
                MaxPlayers = bgn.MaxPlayers,
                Is18Plus = bgn.Is18Plus
            }).ToList();

            return View(viewModel);
        }

        // Requires the user to be logged in
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var boardGames = await _boardGameRepository.GetAllAsync();
            var viewModel = new BoardGameNightViewModel
            {
                BoardGames = boardGames.ToList()
            };
            return View(viewModel);
        }

        // POST: Requires the user to be logged in
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardGameNightViewModel model)
        {
            if (ModelState.IsValid)
            {
                var personId = GetPersonIdFromClaims();
                if (personId == null)
                {
                    TempData["Error"] = "You need to be logged in to create a board game night.";
                    return RedirectToAction("Login", "User");
                }

                var person = await _personRepository.GetByIdAsync(personId.Value);
                var selectedBoardGames = await _boardGameRepository.GetByIdsAsync(model.SelectedBoardGameIds);

                var boardGameNight = new BoardGameNight
                {
                    MaxPlayers = model.MaxPlayers,
                    Date = model.Date,
                    Is18Plus = model.Is18Plus,
                    Address = model.Address,
                    OrganizerId = person.PersonId,
                    Organizer = person,
                    BoardGames = selectedBoardGames.ToList(),
                    FoodOptions = model.FoodOptions.ToList()
                };

                // Validate the board game night
                var validationResult = _boardGameNightValidator.ValidateBoardGameNight(boardGameNight);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }

                    // Reload the list of board games before returning the view
                    var allBoardGames = await _boardGameRepository.GetAllAsync();
                    model.BoardGames = allBoardGames.ToList();

                    // Return the view with validation errors and the reloaded list of games
                    return View(model);
                }

                // If validation passes, add the BoardGameNight to the repository
                await _boardGameNightRepository.AddAsync(boardGameNight);
                return RedirectToAction(nameof(Index));
            }

            // Reload the list of board games if ModelState is invalid
            var boardGames = await _boardGameRepository.GetAllAsync();
            model.BoardGames = boardGames.ToList();

            // Return the view with the reloaded list of games
            return View(model);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var boardGameNight = await _boardGameNightRepository.GetByIdAsync(id);

            if (boardGameNight == null)
            {
                return NotFound();
            }

            // Retrieve PersonID from the cookie
            // Get the current logged-in user's PersonId
            var currentUserPersonId = GetPersonIdFromClaims();
            if (currentUserPersonId == null)
            {
                return Unauthorized();
            }

            // Map the domain model to the view model
            var viewModel = new BoardGameNightDetailViewModel
            {
                BoardGameNightId = boardGameNight.BoardGameNightId,
                OrganizerId = boardGameNight.OrganizerId,
                OrganizerName = boardGameNight.Organizer.Name,
                Participants = boardGameNight.Participants?.ToList() ?? new List<Person>(), // Keep the full Person object
                MaxPlayers = boardGameNight.MaxPlayers,
                Date = boardGameNight.Date,
                Is18Plus = boardGameNight.Is18Plus,
                Address = boardGameNight.Address,
                BoardGames = boardGameNight.BoardGames.ToList(),
                FoodOptions = boardGameNight.FoodOptions.Select(f => f.ToString()).ToList(), // Convert enum to string
                Reviews = boardGameNight.Reviews,
                CanEditOrDelete = boardGameNight.Participants.Count == 0 && currentUserPersonId == boardGameNight.OrganizerId,
                CurrentUserPersonId = currentUserPersonId
            };

            return View(viewModel);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SignUp(int id)
        {
            var gameNight = await _boardGameNightRepository.GetByIdAsync(id);
            if (gameNight == null)
            {
                return NotFound();
            }

            var personId = GetPersonIdFromClaims();
            if (personId == null)
            {
                return Unauthorized();
            }

            var person = await _personRepository.GetByIdAsync(personId.Value);

            var validationResult = _boardGameNightValidator.ValidateParticipant(person, gameNight);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return RedirectToAction("Details", new { id });
            }

            await _boardGameRepository.AddParticipant(id, person);
            return RedirectToAction("Details", new { id });
        }
        public async Task<IActionResult> CancelParticipation(int id)
        {
            var personId = GetPersonIdFromClaims();
            if (personId == null)
            {
                return Unauthorized();
            }

            // Use .Value because personId is nullable (int?)
            var person = await _personRepository.GetByIdAsync(personId.Value);
            if (person != null)
            {
                await _boardGameRepository.RemoveParticipant(id, personId.Value);
                return RedirectToAction("Details", new { id });
            }

            return View();
        }


        [Authorize]
        public async Task<IActionResult> MyOrganisedNights()
        {
            var personId = GetPersonIdFromClaims();
            if (personId == null)
            {
                return Unauthorized();
            }

            var myBoardGameNights = await _boardGameNightRepository.GetByOrganizerIdAsync(personId.Value);
            var upcomingNights = myBoardGameNights.Where(bgn => bgn.Date >= DateTime.Now).OrderBy(bgn => bgn.Date).ToList();
            var pastNights = myBoardGameNights.Where(bgn => bgn.Date < DateTime.Now).OrderByDescending(bgn => bgn.Date).ToList();

            var viewModel = new MyBoardGameNightsViewModel
            {
                UpcomingBoardGameNights = upcomingNights,
                PastBoardGameNights = pastNights
            };

            return View(viewModel);
        }
        [Authorize]
        public async Task<IActionResult> Participations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Get the person with their participations
            var person = await _personRepository.GetPersonWithParticipationsAsync(user.PersonId);
            if (person == null)
            {
                return Unauthorized();
            }

            var participationNights = person.Participations;
            var orderedParticipationNights = participationNights.OrderBy(bgn => bgn.Date);
            return View(orderedParticipationNights);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var boardGameNight = await _boardGameNightRepository.GetByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (boardGameNight == null || boardGameNight.OrganizerId != user.PersonId)
            {
                return NotFound();
            }

            if (boardGameNight.Participants.Any())
            {
                TempData["Error"] = "You cannot delete a board game night that already has participants.";
                return RedirectToAction("Details", new { id });
            }

            await _boardGameNightRepository.DeleteAsync(boardGameNight);
            return RedirectToAction("Index");
        }
    }
}
