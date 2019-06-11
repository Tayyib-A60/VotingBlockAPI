using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Core;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Persistence
{
    public class ElectionRepository : IElectionRepository
    {
        private VotingDBContext _context { get; }
        public ElectionRepository(VotingDBContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<Election>> GetElections(int userId)
        {
            return await _context.Elections
            .Where(e => e.UserId == userId)
            .ToListAsync();
        }

        public async Task<Election> GetElection(int electionId, int userId)
        {
            return await _context.Elections
            .SingleOrDefaultAsync(e => e.UserId == userId && e.ElectionId == electionId);
        }

        public void CreateElection(Election election)
        {
            _context.Entry(election).State = EntityState.Added;
        }

        public void DeleteElection(Election election)
        {
            _context.Entry(election).State = EntityState.Deleted;
        }
    }
}