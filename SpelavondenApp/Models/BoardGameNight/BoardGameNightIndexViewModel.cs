namespace SpelavondenApp.Models.BoardGameNight
{
    public class BoardGameNightIndexViewModel
    {
        public int Id { get; set; }  // Add this to store the ID of the board game night
        public DateTime Date { get; set; }
        public string OrganizerName { get; set; }
        public int ParticipantCount { get; set; }
        public int MaxPlayers { get; set; }
        public bool Is18Plus { get; set; }
    }
}
