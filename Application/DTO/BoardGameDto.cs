using Domain.Models;

namespace Application.DTO
{
    public class BoardGameDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public GameGenre Genre { get; set; }
        public GameType GameType { get; set; }
        public bool Is18Plus { get; set; }

    }
}
