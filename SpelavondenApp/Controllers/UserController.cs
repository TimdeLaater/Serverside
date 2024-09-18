using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpelavondenApp.Models;

namespace SpelavondenApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(IApplicationUserRepository userRepository, SignInManager<ApplicationUser> signInManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
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
            //Log the DietaryPreferences
            var dietaryPreferences = model.DietaryPreferences;
            Console.WriteLine(dietaryPreferences);
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Person = new Person
                    {
                        Name = model.Name,
                        Email = model.Email,
                        BirthDate = model.BirthDate,
                        Reviews = new List<Review>(),
                        Participations = new List<BoardGameNight>(),
                        Address = model.Address,
                        Gender = model.Gender,
                        DietaryPreferences = model.DietaryPreferences



                    }
                };

                var result = await _userRepository.CreateUserAsync(user);
                if (result.Succeeded)
                {
                    // Store PersonID in a cookie after successful registration
                    Response.Cookies.Append("PersonID", user.Person.PersonId.ToString(), new CookieOptions
                    {
                        HttpOnly = true,
                        IsEssential = true,
                        Expires = DateTimeOffset.UtcNow.AddDays(30) // Expiration as needed
                    });
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
                    Console.WriteLine("Login succeeded!");
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    Console.WriteLine("User account is locked out.");
                    // Handle lockout case
                    ModelState.AddModelError("", "User account is locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    Console.WriteLine("User is not allowed to sign in.");
                    // Email not confirmed or other issues
                    ModelState.AddModelError("", "User is not allowed to sign in.");
                }
                else
                {

                    Console.WriteLine("Invalid login attempt.");
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
                if (result.Succeeded)
                {
                    var user = await _userRepository.GetByEmailAsync(model.Email);

                    Response.Cookies.Append("PersonID", user.PersonId.ToString(), new CookieOptions
                    {
                        HttpOnly = true, // Ensures the cookie is accessible only by the server
                        IsEssential = true, // Mark it as essential for non-EU GDPR
                        Expires = DateTimeOffset.UtcNow.AddDays(30) // Set an expiration date
                    });
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
