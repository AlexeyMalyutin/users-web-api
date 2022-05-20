using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersWebApi.DTO;
using UsersWebApi.Models;

namespace UsersWebApi.Repositories
{
    public interface IUsersRepository
    {
        Task CreateNewUserAsync(string login, User user);
        Task<User> UpdateUserDataAsync(string login, string loginToFind, User userData);
        Task<User> UpdateUserPasswordAsync(string login, string loginToFind, string newPassword);
        Task<User> UpdateUserLoginAsync(string login, string loginToFind, string newLogin);
        Task<IEnumerable<User>> GetActiveUsersAsync(string login, string password);
        Task<UsersDTO> GetByLoginAsync(string loginToSearch);
        Task<User> GetByLoginAndPasswordAsync(string login, string password);
        Task<IEnumerable<User>> GetOlderThanAgeAsync(int age);
        Task<User> DeleteUserAsync(string login, string loginToDelete, bool isSoft);
        Task<User> RecoverUserAsync(string login, string loginToFind);
    }
}
