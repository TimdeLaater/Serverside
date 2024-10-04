using Domain.Models;

namespace SpelavondenApp.Models.BoardGameNight
{
    public class BoardGameNightDetailViewModel
    {
        public int BoardGameNightId { get; set; }
        public int? CurrentUserPersonId { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; }
        public List<Person>? Participants { get; set; }
        public int MaxPlayers { get; set; }
        public DateTime Date { get; set; }
        public bool Is18Plus { get; set; }
        public Address Address { get; set; }
        public List<BoardGame> BoardGames { get; set; }
        public List<DietaryPreference> FoodOptions { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public bool CanEditOrDelete { get; set; }
        public bool IsParticipant { get; set; }

    }
}
