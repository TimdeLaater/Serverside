using Application.Interfaces;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<BoardGame>> GetBoardGamesAsync() =>
            await _boardGameRepository.GetAllAsync();

        // Query to get a board game by ID
        public async Task<BoardGame> GetBoardGameAsync(int id) =>
            await _boardGameRepository.GetByIdAsync(id);

        // Query to get a board game night by ID
        public async Task<BoardGameNight> GetBoardGameNightAsync(int id) =>
            await _boardGameNightRepository.GetByIdAsync(id);

        // Query to get all board game nights
        public async Task<IEnumerable<BoardGameNight>> GetBoardGameNightsAsync() =>
            await _boardGameNightRepository.GetAllAsync();
    }
}
