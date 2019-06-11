using System.Threading.Tasks;
using API.Core;

namespace API.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private VotingDBContext _context { get; }
        public UnitOfWork (VotingDBContext context) {
            _context = context;
        }
        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}