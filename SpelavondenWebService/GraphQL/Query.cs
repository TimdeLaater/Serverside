using Application.Interfaces;
using Domain.Models;

namespace SpelavondenWebService.GraphQL
{
    public class Query
    {
        private readonly IBoardGameRepository _boardGameRepository;
        private readonly IBoardGameNightRepository _boardGameNightRepository;

        public Query(IBoardGameRepository boardGameRepository, IBoardGameNightRepository boardGameNightRepository)
        {
            _boardGameRepository = boardGameRepository;
            _boardGameNightRepository = boardGameNightRepository;
        }

        // Query to get all board games
        public IEnumerable<BoardGame> GetBoardGames() =>
            _boardGameRepository.GetAllAsync().Result;

        // Query to get a board game by ID
        public BoardGame GetBoardGame(int id) =>
            _boardGameRepository.GetByIdAsync(id).Result;

        public BoardGameNight GetBoardGameNight(int id) =>
            _boardGameNightRepository.GetByIdAsync(id).Result;
        public IEnumerable<BoardGameNight> GetBoardGameNights() =>
            _boardGameNightRepository.GetAllAsync().Result;
    }
}
