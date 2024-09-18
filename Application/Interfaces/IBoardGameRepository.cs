using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBoardGameRepository: IRepo<BoardGame>
    {
        Task AddParticipant(int id, Person person);
        Task<IEnumerable<BoardGame>> GetByIdsAsync(IEnumerable<int> ids);
        Task RemoveParticipant(int id, int personId);
    }
}
