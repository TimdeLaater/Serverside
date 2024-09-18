using Application.Interfaces;
using Domain.Models;
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

        public BoardGameNightController(IBoardGameNightRepository boardGameNightRepository, IPersonRepository personRepository, UserManager<ApplicationUser> userManager)
        {
            _boardGameNightRepository = boardGameNightRepository;
            _personRepository = personRepository;
            _userManager = userManager;
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

        // GET: Create action to show the form for creating a new board game night
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create action to save the new board game night
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardGameNight boardGameNight)
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

                // Set the organizer of the BoardGameNight
                boardGameNight.Organizer = person;

                // Add the BoardGameNight to the database
                await _boardGameNightRepository.AddAsync(boardGameNight);

                return RedirectToAction(nameof(Index));
            }
            return View(boardGameNight);
        }

        // Details action to show details of a specific board game night
        public async Task<IActionResult> Details(int id)
        {
            var boardGameNight = await _boardGameNightRepository.GetByIdAsync(id);
            if (boardGameNight == null)
            {
                return NotFound();
            }
            return View(boardGameNight);
        }

        // MyOrganisedNights action to show all board game nights organized by the logged-in user
        [Authorize]
        public async Task<IActionResult> MyOrganisedNights()
        {
            // Get the logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var myBoardGameNights = await _boardGameNightRepository.GetByOrganizerIdAsync(user.PersonId);
            return View(myBoardGameNights);
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
            return View(participationNights);
        }
    }
}
