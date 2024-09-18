using Domain.Models;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPersonRepository : IRepo<Person>
    {
        Task<Person> GetPersonWithParticipationsAsync(int personId);
    }
}
