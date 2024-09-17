using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class BoardGameNight
    {
        public int BoardGameNightId { get; set; }
        public int OrganizerId { get; set; }
        public Person Organizer { get; set; }   // One-to-Many relationship

        public List<Person>? Participants { get; set; }

        public int MaxPlayers { get; set; }
        public DateTime Date { get; set; }
        public bool Is18Plus { get; set; }

        public ICollection<BoardGame> BoardGames { get; set; }
        public string FoodOptions { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public Address Address { get; set; }
    }

}
