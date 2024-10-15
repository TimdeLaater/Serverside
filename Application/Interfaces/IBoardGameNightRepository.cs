using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBoardGameNightRepository: IRepo<BoardGameNight>
    {
        new Task<BoardGameNight> AddAsync(BoardGameNight boardGameNight); 
        Task DeleteAsync(int id);

        Task<IEnumerable<BoardGameNight>> GetByOrganizerIdAsync(int organizerId);
        Task<BoardGameNight?> GetByPersonAndDateAsync(int personId, DateTime date);
        Task AddParticipant(int id, Person person);
        Task RemoveParticipant(int id, int personId);
    }
}
