namespace SpelavondenApp.Models.BoardGameNight
{
    public class MyBoardGameNightsViewModel
    {
        public List<Domain.Models.BoardGameNight> PastBoardGameNights { get; set; }
        public List<Domain.Models.BoardGameNight> UpcomingBoardGameNights { get; set; }
    }
}
