using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace SpelavondenApp.Models.BoardGameNight
{
    public class BoardGameNightViewModel
    {
        public int BoardGameNightId { get; set; }

        [Required]
        [Display(Name = "Maximum Players")]
        public int MaxPlayers { get; set; }

        [Required]
        [Display(Name = "Date of Game Night")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "18+ Event")]
        public bool Is18Plus { get; set; }

        [Required]
        public Address Address { get; set; }

        [Display(Name = "Board Games")]
        public List<int> SelectedBoardGameIds { get; set; } = new List<int>(); // This will capture the selected IDs

        public ICollection<BoardGame> BoardGames { get; set; } = new List<BoardGame>(); // For displaying in the form


        [Display(Name = "Food Options")]
        public List<DietaryPreference> FoodOptions { get; set; } = new List<DietaryPreference>();

    }
}
