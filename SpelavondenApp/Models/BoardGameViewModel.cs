using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace SpelavondenApp.Models
{
    public class BoardGameViewModel
    {
        public int BoardGameId { get; set; }

        [Required(ErrorMessage = "The name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a genre.")]
        public GameGenre Genre { get; set; }

        [Display(Name = "Is this game 18+?")]
        public bool Is18Plus { get; set; }

        [Required(ErrorMessage = "Please select a game type.")]
        public GameType GameType { get; set; }

        // Image upload field
        [Display(Name = "Upload Game Image")]
        public IFormFile? ImageFile { get; set; }

        // For displaying the existing image (if editing)
        public byte[]? ExistingImage { get; set; }
    }
}
