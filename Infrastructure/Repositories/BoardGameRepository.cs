using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class BoardGameRepository : IBoardGameRepository
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
        public async Task<IEnumerable<BoardGame>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.BoardGames
                .Where(bg => ids.Contains(bg.BoardGameId))
                .ToListAsync();
        }

        public async Task AddParticipant(int id, Person person)
        {
            // Fetch the BoardGameNight by its id
            var boardGameNight = await _context.BoardGameNights
                .Include(b => b.Participants) // Include participants
                .FirstOrDefaultAsync(b => b.BoardGameNightId == id);

            if (boardGameNight == null)
            {
                throw new Exception("BoardGameNight not found");
            }

            // Check if the person is already a participant
            if (!boardGameNight.Participants.Any(p => p.PersonId == person.PersonId))
            {
                // Add the person to the participants collection
                boardGameNight.Participants.Add(person);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Person is already a participant");
            }
        }

        public async Task RemoveParticipant(int id, int personId)
        {
            // Fetch the BoardGameNight by its id
            var boardGameNight = await _context.BoardGameNights
                .Include(b => b.Participants) // Include participants
                .FirstOrDefaultAsync(b => b.BoardGameNightId == id);

            if (boardGameNight == null)
            {
                throw new Exception("BoardGameNight not found");
            }

            // Find the person in the participants list
            var person = boardGameNight.Participants.FirstOrDefault(p => p.PersonId == personId);

            if (person != null)
            {
                // Remove the person from the participants collection
                boardGameNight.Participants.Remove(person);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Person not found in participants");
            }
        }

    }
}
