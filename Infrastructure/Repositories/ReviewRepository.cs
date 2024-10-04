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

        public async Task<double> GetAverageRatingForOrganizerAsync(int organizerId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BoardGameNight.OrganizerId == organizerId)
                .ToListAsync();

            if (reviews.Count == 0)
            {
                return 0; // No reviews means average rating is 0
            }

            return reviews.Average(r => r.Rating);
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task<int> GetReviewCountForOrganizerAsync(int organizerId)
        {
            return await _context.Reviews
                .CountAsync(r => r.BoardGameNight.OrganizerId == organizerId);
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
    }
}
