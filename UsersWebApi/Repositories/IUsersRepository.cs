using System.Collections.Generic;
using System.Threading.Tasks;
using UsersWebApi.DTO;
using UsersWebApi.Models;

namespace UsersWebApi.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<Users>> GetAll();
        Task CreateNewUserAsync(string login, UsersCreateDTO user);
        Task<Users> UpdateUserDataAsync(string login, string loginToFind, UsersUpdateDTO userData);
        Task<Users> UpdateUserPasswordAsync(string login, string loginToFind, string newPassword);
        Task<Users> UpdateUserLoginAsync(string login, string loginToFind, string newLogin);
        Task<IEnumerable<Users>> GetActiveUsersAsync(string login, string password);
        Task<UsersDTO> GetByLoginAsync(string loginToSearch);
        Task<Users> GetByLoginAndPasswordAsync(string login, string password);
        Task<IEnumerable<Users>> GetOlderThanAgeAsync(int age);
        Task<Users> DeleteUserAsync(string login, string loginToDelete, bool isSoft);
        Task<Users> RecoverUserAsync(string login, string loginToFind);
    }
}
