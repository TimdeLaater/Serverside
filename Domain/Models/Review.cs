using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Review
    {
        public int ReviewId { get; set; }  // Primary key, auto-increment

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Review text is required.")]
        [MaxLength(500, ErrorMessage = "Review text cannot exceed 500 characters.")]
        public required string ReviewText { get; set; }

        [Required(ErrorMessage = "Reviewer is required.")]
        public int ReviewerId { get; set; }  // Foreign key to Person
        [Required(ErrorMessage = "Reviewer is required.")]
        public virtual required Person Reviewer { get; set; }  // Navigation property

        [Required(ErrorMessage = "BoardGameNight is required.")]
        public int BoardGameNightId { get; set; }  // Foreign key to BoardGameNight
        [Required(ErrorMessage = "BoardGameNight is required.")]
        public virtual required BoardGameNight BoardGameNight { get; set; }  // Navigation property
    }
}
