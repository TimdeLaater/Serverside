using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpelavondenApp.Models.BoardGameNight
{
    public class BoardGameNightViewModel
    {
        public int BoardGameNightId { get; set; }

        [Required(ErrorMessage = "Maximum number of players is required.")]
        [Range(2, 20, ErrorMessage = "The maximum number of players must be between 2 and 20.")]
        [Display(Name = "Maximum Players")]
        public int MaxPlayers { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [Display(Name = "Date of Game Night")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please specify if this is an 18+ event.")]
        [Display(Name = "18+ Event")]
        public bool Is18Plus { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public Address Address { get; set; }

        [Display(Name = "Board Games")]
        public List<int> SelectedBoardGameIds { get; set; } = new List<int>(); // This will capture the selected IDs

        public ICollection<BoardGame> BoardGames { get; set; } = new List<BoardGame>(); // For displaying in the form

        [Display(Name = "Food Options")]
        public List<DietaryPreference> FoodOptions { get; set; } = new List<DietaryPreference>();

    }
}
