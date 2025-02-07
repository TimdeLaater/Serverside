﻿using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BoardGameNightRepository : IBoardGameNightRepository
    {
        private readonly AppDbContext _context;

        public BoardGameNightRepository(AppDbContext context)
        {
            _context = context;
        }

        // Implement the generic GetByIdAsync
        public async Task<BoardGameNight> GetByIdAsync(int id)
        {
            return await _context.BoardGameNights
                .Include(bgn => bgn.Organizer)
                .Include(bgn => bgn.Participants)
                .Include(b => b.BoardGames) // Load the board games
                .Include(bgn => bgn.Reviews)
                 .AsSplitQuery() // Split the query into multiple queries
                .FirstOrDefaultAsync(bgn => bgn.BoardGameNightId == id);
        }

        // Implement the generic GetAllAsync
        public async Task<IEnumerable<BoardGameNight>> GetAllAsync()
        {
            return await _context.BoardGameNights
                .Include(bgn => bgn.Organizer)
                .Include(bgn => bgn.Participants)
                .Include(b => b.BoardGames) 
                .ToListAsync();
        }

        // Override AddAsync to return the added entity
        public async Task<BoardGameNight> AddAsync(BoardGameNight boardGameNight)
        {
            await _context.BoardGameNights.AddAsync(boardGameNight);
            await _context.SaveChangesAsync();
            return boardGameNight;
        }

        // Update an existing BoardGameNight entity
        public async Task UpdateAsync(BoardGameNight boardGameNight)
        {
            _context.BoardGameNights.Update(boardGameNight);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BoardGameNight boardGameNight)
        {
            _context.BoardGameNights.Remove(boardGameNight);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BoardGameNight>> GetByOrganizerIdAsync(int organizerId)
        {
            return await _context.BoardGameNights
                .Where(bgn => bgn.OrganizerId == organizerId)
                .Include(bgn => bgn.Organizer)
                .Include(bgn => bgn.Participants)
                .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var boardGameNight = await GetByIdAsync(id);
            if (boardGameNight != null)
            {
                _context.BoardGameNights.Remove(boardGameNight);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<BoardGameNight?> GetByPersonAndDateAsync(int personId, DateTime date)
        {
            return await _context.BoardGameNights
                .Include(bgn => bgn.Participants)
                .Where(bgn => bgn.Participants.Any(p => p.PersonId == personId) && bgn.DateTime.Date == date)
                .FirstOrDefaultAsync();
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
