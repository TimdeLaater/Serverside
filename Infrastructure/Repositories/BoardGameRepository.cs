using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class BoardGameRepository : IRepo<BoardGame>
    {
        private readonly AppDbContext _context;

        public BoardGameRepository(AppDbContext context)
        {
            _context = context;
        }

        // Add a new BoardGame entity
        public async Task<BoardGame> AddAsync(BoardGame entity)
        {
            await _context.BoardGames.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // Delete a BoardGame entity
        public async Task DeleteAsync(BoardGame entity)
        {
            _context.BoardGames.Remove(entity);
            await _context.SaveChangesAsync();
        }

        // Get all BoardGame entities
        public async Task<IEnumerable<BoardGame>> GetAllAsync()
        {
            return await _context.BoardGames.ToListAsync();
        }

        // Get a BoardGame entity by ID
        public async Task<BoardGame> GetByIdAsync(int id)
        {
            return await _context.BoardGames.FindAsync(id);
        }

        // Update an existing BoardGame entity
        public async Task UpdateAsync(BoardGame entity)
        {
            _context.BoardGames.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
