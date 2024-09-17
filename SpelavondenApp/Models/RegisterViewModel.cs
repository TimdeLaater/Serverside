using Domain.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SpelavondenApp.Models
{
    public class RegisterViewModel
    {
        // ApplicationUser properties
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        public Address Address { get; set; }

    }
}
