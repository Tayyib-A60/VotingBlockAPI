using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Core
{
    public interface IElectionRepository
    {
        Task<IEnumerable<Election>> GetElections(int userId);
        Task<Election> GetElection(int electionId, int userId);
        void CreateElection(Election election);
        void DeleteElection(Election election);
    }
}