using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpelavondenApp.Models
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Birthdate is required.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public required Address Address { get; set; }

        [Required]
        public List<DietaryPreference> DietaryPreferences { get; set; } = new List<DietaryPreference>();
    }
}
