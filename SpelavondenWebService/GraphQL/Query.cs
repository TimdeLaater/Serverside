using Domain.Models;
using HotChocolate;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpelavondenWebService.GraphQL
{
    public class Query
    {
        // Query to get all board games
        public async Task<IEnumerable<BoardGame>> GetBoardGamesAsync([Service] AppDbContext dbContext) =>
            await dbContext.BoardGames.ToListAsync();

        // Query to get a board game by ID
        public async Task<BoardGame> GetBoardGameAsync(int id, [Service] AppDbContext dbContext) =>
            await dbContext.BoardGames.FirstOrDefaultAsync(bg => bg.BoardGameId == id);

        // Query to get all board game nights with all related entities
        public async Task<IEnumerable<BoardGameNight>> GetBoardGameNightsAsync([Service] AppDbContext dbContext) =>
            await dbContext.BoardGameNights
                .Include(bgn => bgn.Organizer) // Including Organizer (Person)
                .Include(bgn => bgn.Participants) // Including Participants
                .Include(bgn => bgn.BoardGames) // Including BoardGames
                .Include(bgn => bgn.Reviews) // Including Reviews
                .ToListAsync();

        // Query to get a single board game night by ID with all related entities
        public async Task<BoardGameNight> GetBoardGameNightAsync(int id, [Service] AppDbContext dbContext) =>
            await dbContext.BoardGameNights
                .Include(bgn => bgn.Organizer) // Including Organizer (Person)
                .Include(bgn => bgn.Participants) // Including Participants
                .Include(bgn => bgn.BoardGames) // Including BoardGames
                .Include(bgn => bgn.Reviews) // Including Reviews
                .FirstOrDefaultAsync(bgn => bgn.BoardGameNightId == id);
    }
}
