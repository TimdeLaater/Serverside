using Application.Interfaces;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpelavondenApp.Models;
using System.Security.Claims;

namespace SpelavondenApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager; 

        public UserController(IApplicationUserRepository userRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new Person entity
                var person = new Person
                {
                    Name = model.Name,
                    Email = model.Email,
                    BirthDate = model.BirthDate,
                    Reviews = new List<Review>(),
                    Participations = new List<BoardGameNight>(),
                    Address = model.Address,
                    Gender = model.Gender,
                    DietaryPreferences = model.DietaryPreferences
                };

                // Validate the person
                var personValidationService = new PersonValidationService();
                var validationResult = personValidationService.ValidatePerson(person);

                if (!validationResult.IsValid)
                {
                    // If validation fails, add errors to ModelState to display in the front end
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View(model); 
                }

                // If validation passes, create the ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Person = person
                };

                var result = await _userRepository.CreateUserAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add PersonId claim after the user is created
                    var claims = new List<Claim>
                    {
                        new Claim("PersonId", user.Person.PersonId.ToString()) 
                    };

                    await _userManager.AddClaimsAsync(user, claims);

                    // Sign in the user after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // Login action
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userRepository.GetByEmailAsync(model.Email);

                    // Ensure the PersonId claim is added after login
                    var claims = await _userManager.GetClaimsAsync(user);
                    if (!claims.Any(c => c.Type == "PersonId"))
                    {
                        var personClaim = new Claim("PersonId", user.Person.PersonId.ToString());
                        await _userManager.AddClaimAsync(user, personClaim);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "User account is locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "User is not allowed to sign in.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            return View(model);
        }

        // Logout action
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
