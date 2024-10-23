using Application.Interfaces;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Attempt to find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Check the password and sign in
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                // Ensure the PersonId claim is added after login
                var claims = await _userManager.GetClaimsAsync(user);
                if (!claims.Any(c => c.Type == "PersonId"))
                {
                    var personClaim = new Claim("PersonId", user.PersonId.ToString());
                    await _userManager.AddClaimAsync(user, personClaim);
                }

                return RedirectToAction("Index", "Home");
            }

            // Handle different sign-in failure scenarios
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "User account is locked out.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "User is not allowed to sign in.");
            }
            else
            {
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

        // POST: User/Delete
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _userRepository.DeleteUserAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }

        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> Edit()
        //{
        //    Console.WriteLine("Edit");
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null || user.Person == null)
        //    {
        //        return NotFound();
        //    }

        //    var model = new UserEditViewModel
        //    {
        //        Name = user.Person.Name,
        //        BirthDate = user.Person.BirthDate,
        //        Address = user.Person.Address,
        //        DietaryPreferences = user.Person.DietaryPreferences,

        //    };
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(UserEditViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        if (user == null || user.Person == null)
        //        {
        //            return NotFound();
        //        }

        //        user.Person.Name = model.Name;
        //        user.Person.BirthDate = model.BirthDate;
        //        user.Person.Address = model.Address;
        //        user.Person.DietaryPreferences = model.DietaryPreferences;

        //        var personValidationService = new PersonValidationService();
        //        var validationResult = personValidationService.ValidatePerson(user.Person);

        //        if (!validationResult.IsValid)
        //        {
        //            foreach (var error in validationResult.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error);
        //            }
        //            return View(model);
        //        }

        //        await _userRepository.UpdateUserAsync(user);
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View(model);
        //}
    }

}

