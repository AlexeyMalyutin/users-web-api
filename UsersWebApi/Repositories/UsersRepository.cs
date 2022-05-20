using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersWebApi.DTO;
using UsersWebApi.Models;

namespace UsersWebApi.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly List<User> users = new List<User>
        {
            new User
            {
                Guid = Guid.NewGuid(),
                Login = "Admin",
                Password = "Admin",
                Name = "Admin",
                Gender = 2,
                Admin = true,
                CreatedOn = DateTime.Now
            }
        };
        public async Task CreateNewUserAsync(string login, User newUser)
        {
            newUser.Guid = Guid.NewGuid();
            newUser.CreatedBy = login;
            newUser.CreatedOn = DateTime.Now;
            newUser.RevokedBy = null;
            newUser.RevokedOn = null;
            newUser.ModifiedBy = null;
            newUser.ModifiedOn = null;
            users.Add(newUser);

            await Task.CompletedTask;
        }

        public async Task<User> DeleteUserAsync(string login, string loginToDelete, bool isSoft)
        {
            var userToDelete = await Task.FromResult(users.FirstOrDefault(u => u.Login == loginToDelete));
            if (userToDelete != null)
            {
                if (isSoft)
                {
                    userToDelete.RevokedBy = login;
                    userToDelete.RevokedOn = DateTime.Now;
                }
                else
                {
                    users.Remove(userToDelete);
                }
            }

            return userToDelete;
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync(string login, string password)
        {
            var activeUsers = await Task.FromResult(users
                .Where(u => u.RevokedOn == null)
                .OrderByDescending(u => u.CreatedOn));

            return activeUsers;
        }

        public async Task<User> GetByLoginAndPasswordAsync(string login, string password)
        {
            var user = await Task.FromResult(users.
                FirstOrDefault(u => u.Login == login && u.Password == password));
            return user;
        }

        public async Task<UsersDTO> GetByLoginAsync(string loginToSearch)
        {
            var userToFind = await Task.FromResult(users
                    .Where(u => u.Login == loginToSearch)
                    .Select(x => new UsersDTO
                    {
                        Name = x.Name,
                        Gender = x.Gender,
                        Birthday = x.Birthday,
                        RevokedOn = x.RevokedOn
                    })
                    .SingleOrDefault());

            return userToFind;
        }

        public async Task<IEnumerable<User>> GetOlderThanAgeAsync(int age)
        {
            var usersOlderThanAge = await Task.FromResult(users
                .Where(u => (u.Birthday.HasValue ? u.Birthday.Value : DateTime.MaxValue) < DateTime.Today.AddYears(-age)));

            return usersOlderThanAge;
        }

        public async Task<User> RecoverUserAsync(string login, string loginToFind)
        {
            var userToRecover = await Task.FromResult(users.FirstOrDefault(u => u.Login == loginToFind));
            if (userToRecover != null)
            {
                userToRecover.RevokedBy = null;
                userToRecover.RevokedOn = null;
                userToRecover.ModifiedBy = login;
                userToRecover.ModifiedOn = DateTime.Now;
            }

            return userToRecover;
        }

        public async Task<User> UpdateUserDataAsync(string login, string loginToFind, User userData)
        {
            var userToUpdate = await Task.FromResult(users.FirstOrDefault(u => u.Login == loginToFind));
            if (userToUpdate != null)
            {
                userToUpdate.Name = userData.Name;
                userToUpdate.Gender = userData.Gender;
                userToUpdate.Birthday = userData.Birthday;
                userToUpdate.ModifiedBy = login;
                userToUpdate.ModifiedOn = DateTime.Now;
            }

            return userToUpdate;
        }

        public async Task<User> UpdateUserLoginAsync(string login, string loginToFind, string newLogin)
        {
            var userToUpdate = await Task.FromResult(users.FirstOrDefault(u => u.Login == loginToFind));
            if (userToUpdate != null)
            {
                userToUpdate.Login = newLogin;
                userToUpdate.ModifiedBy = login;
                userToUpdate.ModifiedOn = DateTime.Now;
            }

            return userToUpdate;
        }

        public async Task<User> UpdateUserPasswordAsync(string login, string loginToFind, string newPassword)
        {
            var userToUpdate = await Task.FromResult(users.FirstOrDefault(u => u.Login == loginToFind));
            if (userToUpdate != null)
            {
                userToUpdate.Password = newPassword;
                userToUpdate.ModifiedBy = login;
                userToUpdate.ModifiedOn = DateTime.Now;
            }

            return userToUpdate;
        }
    }
}
