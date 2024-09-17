using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public int ReviewerId { get; set; }
        public Person Reviewer { get; set; }
        public int BoardGameNightId { get; set; }
        public BoardGameNight BoardGameNight { get; set; }
    }

}
