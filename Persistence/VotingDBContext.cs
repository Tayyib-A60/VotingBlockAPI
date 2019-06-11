using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Persistence
{
    public class VotingDBContext: DbContext
    {
        public DbSet<User> Users {get; set;}
        public DbSet<Election> Elections { get; set; }
        public VotingDBContext (DbContextOptions<VotingDBContext> options) : base (options) {
        }
    }
}