using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Review
    {
       
        public int ReviewId { get; set; }  // Primary key, auto-increment
        public int Rating { get; set; }
        public string ReviewText { get; set; }

        public int ReviewerId { get; set; }  // Foreign key to Person
        public virtual Person Reviewer { get; set; }  // Navigation property

        public int BoardGameNightId { get; set; }  // Foreign key to BoardGameNight
        public virtual BoardGameNight BoardGameNight { get; set; }  // Navigation property
    }


}
