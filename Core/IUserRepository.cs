using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Core
{
    public interface IUserRepository
    {
        User Authenticate(string email, string password);
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<User> GetUser(string email);
        Task<User> CreateUser(User user, string password);
        void UpdateUser (string newPassword, User userToUpdate);
        void DeleteUser(User user);
        Task<bool> UserExists(User user);
        Task<bool> ForgotPassword(User user);
        string CreateToken(User user);
    }
}