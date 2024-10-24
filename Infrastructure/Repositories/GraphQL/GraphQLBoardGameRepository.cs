using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.GraphQL
{
    public class GraphQLBoardGameRepository : IBoardGameRepository
    {
        private readonly AppDbContext _context;

        public GraphQLBoardGameRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<BoardGame> AddAsync(BoardGame entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(BoardGame entity)
        {
            throw new NotImplementedException();
        }

        // Get all BoardGame entities with all related data included
        public async Task<IEnumerable<BoardGame>> GetAllAsync()
        {
            return await _context.BoardGames
                .Include(bg => bg.BoardGameNights) // Include related BoardGameNight entities
                .AsNoTracking()
                .ToListAsync();
        }

        // Get a BoardGame entity by ID with all related data included
        public async Task<BoardGame> GetByIdAsync(int id)
        {
            return await _context.BoardGames
                .Include(bg => bg.BoardGameNights) // Include related BoardGameNight entities
                .AsNoTracking()
                .FirstOrDefaultAsync(bg => bg.BoardGameId == id);
        }

        public Task<IEnumerable<BoardGame>> GetByIdsAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(BoardGame entity)
        {
            throw new NotImplementedException();
        }
    }
}
