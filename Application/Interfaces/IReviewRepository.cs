using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReviewRepository: IRepo<Review>
    {
    Task<IEnumerable<Review>> GetReviewsByPersonAsync(int personId);
        Task<IEnumerable<Review>> GetReviewsByGameNightAsync(int gameNightId);
        Task<(int count, double average)> GetReviewStatsForOrganizerAsync(int organizerId);

    }
}
