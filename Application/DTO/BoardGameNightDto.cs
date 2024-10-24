using Domain.Models;

namespace Application.DTO
{
    public class BoardGameNightDto
    {
        public int OrganizerId { get; set; }
        public required string OrganizerName { get; set; }
        public int BoardGameNightId { get; set; }
        public int MaxPlayers { get; set; }
        public DateTime Date { get; set; }
        public bool Is18Plus { get; set; }
        public required Address Address { get; set; }

        public List<BoardGameDto> BoardGames { get; set; } = new();  // Initialize with an empty list
        public List<ParticipantDto> Participants { get; set; } = new();  // Initialize with an empty list
        public List<DietaryPreference> FoodOptions { get; set; } = new();  // Initialize with an empty list
        public List<ReviewDto> Reviews { get; set; } = new();  // Initialize with an empty list
    }

}
