using Application.Interfaces;
using Domain.Models;
using Domain.Services;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpelavondenWebService.Models;
using System.Security.Claims;

namespace SpelavondenWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IApplicationUserRepository _userRepository;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IApplicationUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterApiModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var person = new Person
            {
                Name = model.Name,
                Email = model.Email,
                BirthDate = model.BirthDate,
                Address = model.Address,
                Gender = model.Gender,
                DietaryPreferences = model.DietaryPreferences
            };

            var personValidationService = new PersonValidationService();
            var validationResult = personValidationService.ValidatePerson(person);

            if (!validationResult.IsValid)
            {
                // Voeg validatiefouten toe aan het model
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return BadRequest(ModelState);
            }

            
            var user = new ApplicationUser
            {
                UserName = model.Email,  
                Email = model.Email,
                Person = person
            };

            var result = await _userRepository.CreateUserAsync(user, model.Password);

            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim("PersonId", user.Person.PersonId.ToString())
                };

                await _userManager.AddClaimsAsync(user, claims);

                await _signInManager.SignInAsync(user, isPersistent: false);

                return Ok(new { Message = "Registration and login successful" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
    }
}