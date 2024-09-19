using Application.Interfaces;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpelavondenApp.Models.BoardGameNight;
using System.Threading.Tasks;

namespace SpelavondenApp.Controllers
{
    public class BoardGameNightController : Controller
    {
        private readonly IBoardGameNightRepository _boardGameNightRepository;
        private readonly IPersonRepository _personRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBoardGameRepository _boardGameRepository;

        public BoardGameNightController(IBoardGameNightRepository boardGameNightRepository, IPersonRepository personRepository, UserManager<ApplicationUser> userManager, IBoardGameRepository boardGameRepository)
        {
            _boardGameNightRepository = boardGameNightRepository;
            _personRepository = personRepository;
            _userManager = userManager;
            _boardGameRepository = boardGameRepository;
        }

        // Index action to show all board game nights
        public async Task<IActionResult> Index()
        {
            var boardGameNights = await _boardGameNightRepository.GetAllAsync();
            var sortedBoardGameNights = boardGameNights.OrderBy(bgn => bgn.Date);

            // Map to the ViewModel
            var viewModel = sortedBoardGameNights.Select(bgn => new BoardGameNightIndexViewModel
            {
                Id = bgn.BoardGameNightId,  // Map the ID
                Date = bgn.Date,
                OrganizerName = bgn.Organizer.Name,
                ParticipantCount = bgn.Participants?.Count() ?? 0,
                MaxPlayers = bgn.MaxPlayers,
                Is18Plus = bgn.Is18Plus
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Retrieve the list of board games from the repository
            var boardGames = await _boardGameRepository.GetAllAsync();

            // Create a view model and populate the BoardGames property
            var viewModel = new BoardGameNightViewModel
            {
                BoardGames = boardGames.ToList() // Ensure it's a list if needed
            };

            return View(viewModel);
        }

        // POST: Create action to save the new board game night
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardGameNightViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the logged-in user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                // Get the Person entity of the logged-in user
                var person = await _personRepository.GetByIdAsync(user.PersonId);
                if (person == null)
                {
                    return Unauthorized();
                }

                // Fetch the selected board games from the repository
                var selectedBoardGames = await _boardGameRepository.GetByIdsAsync(model.SelectedBoardGameIds);
                Console.WriteLine("Selected Board Games:");
                foreach (var game in selectedBoardGames)
                {
                    Console.WriteLine($"Board Game ID: {game.BoardGameId}, Name: {game.Name}");
                }

                // Create a new BoardGameNight and map the view model to the domain model
                var boardGameNight = new BoardGameNight
                {
                    MaxPlayers = model.MaxPlayers,
                    Date = model.Date,
                    Is18Plus = model.Is18Plus,
                    Address = model.Address,
                    OrganizerId = person.PersonId,
                    Organizer = person, // Set the logged-in user as the organizer
                    BoardGames = selectedBoardGames.ToList(),  // Add the selected board games
                    FoodOptions = model.FoodOptions.ToList()
                };

                // Add the BoardGameNight to the database
                var boardgamenight = await _boardGameNightRepository.AddAsync(boardGameNight);
                Console.WriteLine($"Board game night created with ID: {boardgamenight.BoardGameNightId}");
                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, return the same view with the model to show validation errors
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var boardGameNight = await _boardGameNightRepository.GetByIdAsync(id);

            if (boardGameNight == null)
            {
                return NotFound();
            }
            var personIdFromCookie = Request.Cookies["PersonID"];


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
                Reviews = boardGameNight.Reviews
            };
            viewModel.CurrentUserPersonId = personIdFromCookie != null ? int.Parse(personIdFromCookie) : (int?)null;


            return View(viewModel);
        }

        // MyOrganisedNights action to show all board game nights organized by the logged-in user
        public async Task<IActionResult> MyOrganisedNights()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var myBoardGameNights = await _boardGameNightRepository.GetByOrganizerIdAsync(user.PersonId);
            var upcomingNights = myBoardGameNights.Where(bgn => bgn.Date >= DateTime.Now).OrderBy(bgn => bgn.Date).ToList();
            var pastNights = myBoardGameNights.Where(bgn => bgn.Date < DateTime.Now).OrderByDescending(bgn => bgn.Date).ToList();

            var viewModel = new MyBoardGameNightsViewModel
            {
                UpcomingBoardGameNights = upcomingNights,
                PastBoardGameNights = pastNights
            };

            return View(viewModel);
        }


        // Participations action to show all board game nights the logged-in user is participating in
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

        [HttpPost]
        public async Task<IActionResult> SignUp(int id)
        {
            // Fetch the BoardGameNight from the repository
            var gameNight = await _boardGameNightRepository.GetByIdAsync(id);

            // Check if the number of participants is less than the maximum allowed players
            if (gameNight.Participants.Count < gameNight.MaxPlayers)
            {
                // Retrieve the current user's PersonID from the cookies
                var personId = int.Parse(Request.Cookies["PersonID"]);

                // Fetch the Person from the repository
                var person = await _personRepository.GetByIdAsync(personId);

                if (person != null)
                {
                    // Add the person as a participant to the game night
                    await _boardGameRepository.AddParticipant(id, person);
                    return RedirectToAction("Details", new { id });
                }
                else
                {
                    // Handle case where person cannot be found
                    return NotFound();
                }
            }
            else
            {
                // Handle the case where the game night is already full
                TempData["ErrorMessage"] = "Sorry, the game night is full.";
                return RedirectToAction("Details", new { id });
            }
        }

        public async Task<IActionResult> CancelParticipation(int id)
        {
            
            var personId = int.Parse(Request.Cookies["PersonID"]);
            var person = await _personRepository.GetByIdAsync(personId);
            if (person != null)
            {
                await _boardGameRepository.RemoveParticipant(id, personId);
                return RedirectToAction("Details", new { id });
            }
            return View();
            
        }
    }
}
