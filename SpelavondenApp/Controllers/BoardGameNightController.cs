using Application.Interfaces;
using Domain.Models;
using Domain.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpelavondenApp.Models.BoardGameNight;
using System;
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

                // Automatisch 18+ instellen als een 18+ spel is gekozen
                if (selectedBoardGames.Any(bg => bg.Is18Plus))
                {
                    model.Is18Plus = true;
                }

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

                // Valideer de bordspellenavond
                var validationResult = _boardGameNightValidator.ValidateBoardGameNight(boardGameNight);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                else
                {
                    // Voeg de BoardGameNight toe aan de repository als validatie succesvol is
                    await _boardGameNightRepository.AddAsync(boardGameNight);
                    return RedirectToAction(nameof(Index));
                }
            }

            // Herlaad de lijst met bordspellen en keer terug naar het formulier bij een fout
            var boardGames = await _boardGameRepository.GetAllAsync();
            model.BoardGames = boardGames.ToList();

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

            var person = await _personRepository.GetByIdAsync(currentUserPersonId.Value);

            var warningMessages = _boardGameNightValidator.CheckDietaryWarnings(person, boardGameNight);

            // Check if there are warning messages
            if (warningMessages.Any())
            {

                    ViewBag.WarningMessages = warningMessages;
            }
            // Map the domain model to the view model
            var viewModel = MapToViewModel(boardGameNight, currentUserPersonId ?? 0);

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

                var viewModel = MapToViewModel(gameNight, personId.Value);
                return View("Details", viewModel);
            }

            var dateValidationResult = await ValidateSignUpAsync(personId.Value, gameNight);
            if (!dateValidationResult.IsValid)
            {
                foreach (var error in dateValidationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                var viewModel = MapToViewModel(gameNight, personId.Value);
                return View("Details", viewModel);
            }

            // Proceed with adding the participant
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

            // Haal de persoon op met zijn/haar participaties
            var person = await _personRepository.GetPersonWithParticipationsAsync(user.PersonId);
            if (person == null)
            {
                return Unauthorized();
            }

            // Filter en sorteer de participaties op basis van datum
            var upcomingNights = person.Participations
                .Where(bgn => bgn.Date >= DateTime.Now)
                .OrderBy(bgn => bgn.Date)
                .ToList();

            var pastNights = person.Participations
                .Where(bgn => bgn.Date < DateTime.Now)
                .OrderByDescending(bgn => bgn.Date)
                .ToList();

            // Maak het viewmodel aan
            var viewModel = new MyBoardGameNightsViewModel
            {
                UpcomingBoardGameNights = upcomingNights,
                PastBoardGameNights = pastNights
            };

            return View(viewModel);
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
        private async Task<ValidationResult> ValidateSignUpAsync(int personId, BoardGameNight gameNight)
        {
            var result = new ValidationResult();

            // Controleer of de speler al is ingeschreven voor een spelavond op dezelfde dag
            var existingGameNightForDay = await _boardGameNightRepository.GetByPersonAndDateAsync(personId, gameNight.Date.Date);

            if (existingGameNightForDay != null)
            {
                result.AddError("You are already signed up for a game night on this day.");
            }

            return result;
        }


        private BoardGameNightDetailViewModel MapToViewModel(BoardGameNight gameNight, int currentUserPersonId)
        {

            return new BoardGameNightDetailViewModel
            {
                BoardGameNightId = gameNight.BoardGameNightId,
                OrganizerId = gameNight.OrganizerId,
                OrganizerName = gameNight.Organizer?.Name ?? "Unknown",
                Participants = gameNight.Participants?.ToList() ?? new List<Person>(),
                MaxPlayers = gameNight.MaxPlayers,
                Date = gameNight.Date,
                Is18Plus = gameNight.Is18Plus,
                Address = gameNight.Address,
                BoardGames = gameNight.BoardGames.ToList(),
                FoodOptions = gameNight.FoodOptions,
                Reviews = gameNight.Reviews,
                CanEditOrDelete = gameNight.Participants.Count == 0 && currentUserPersonId == gameNight.OrganizerId,
                CurrentUserPersonId = currentUserPersonId,
                IsParticipant = IsUserParticipant(currentUserPersonId, gameNight),
            };
        }
        private bool IsUserParticipant(int personId, BoardGameNight boardGameNight)
        {
            return boardGameNight.Participants.Any(p => p.PersonId == personId);
        }


    }
}
