using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> AddAsync(Review entity)
        {
            await _context.Reviews.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Review entity)
        {
            _context.Reviews.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews.ToListAsync();
        }


        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }


        public async Task<IEnumerable<Review>> GetReviewsByGameNightAsync(int gameNightId)
        {
            return await _context.Reviews
                .Where(r => r.BoardGameNightId == gameNightId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByPersonAsync(int personId)
        {
            return await _context.Reviews
                .Where(r => r.ReviewerId == personId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Review entity)
        {
            _context.Reviews.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<(int count, double average)> GetReviewStatsForOrganizerAsync(int organizerId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BoardGameNight.OrganizerId == organizerId)
                .ToListAsync();

            int count = reviews.Count;
            double average = count > 0 ? reviews.Average(r => r.Rating) : 0;

            return (count, average);
        }

    }
}
