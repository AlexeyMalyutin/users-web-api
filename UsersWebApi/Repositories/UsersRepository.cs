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
        private readonly List<Users> users = new List<Users>
        {
            new Users
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
        public async Task CreateNewUserAsync(string login, UsersCreateDTO newUser)
        {
            Users userToCreate = new Users
            {
                Login = newUser.Login,
                Password = newUser.Password,
                Name = newUser.Name,
                Gender = newUser.Gender,
                Birthday = newUser.Birthday,
                Admin = newUser.Admin,
                Guid = Guid.NewGuid(),
                CreatedBy = login,
                CreatedOn = DateTime.Now,
                RevokedBy = null,
                RevokedOn = null,
                ModifiedBy = null,
                ModifiedOn = null
            };

            users.Add(userToCreate);

            await Task.CompletedTask;
        }

        public async Task<Users> DeleteUserAsync(string login, string loginToDelete, bool isSoft)
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

        public async Task<IEnumerable<Users>> GetActiveUsersAsync(string login, string password)
        {
            var activeUsers = await Task.FromResult(users
                .Where(u => u.RevokedOn == null)
                .OrderByDescending(u => u.CreatedOn));

            return activeUsers;
        }

        public async Task<IEnumerable<Users>> GetAll()
        {
            return await Task.FromResult(users);
        }

        public async Task<Users> GetByLoginAndPasswordAsync(string login, string password)
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

        public async Task<IEnumerable<Users>> GetOlderThanAgeAsync(int age)
        {
            var usersOlderThanAge = await Task.FromResult(users
                .Where(u => (u.Birthday.HasValue ? u.Birthday.Value : DateTime.MaxValue) < DateTime.Today.AddYears(-age)));

            return usersOlderThanAge;
        }

        public async Task<Users> RecoverUserAsync(string login, string loginToFind)
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

        public async Task<Users> UpdateUserDataAsync(string login, string loginToFind, UsersUpdateDTO userData)
        {
            var userToUpdate = await Task.FromResult(users.FirstOrDefault(u => u.Login == loginToFind));
            if (userToUpdate != null)
            {
                if(userData.Name!=null) userToUpdate.Name = userData.Name;
                if (userData.Gender != null) userToUpdate.Gender = (int)userData.Gender;
                if (userData.Birthday != null) userToUpdate.Birthday = userData.Birthday;
                userToUpdate.ModifiedBy = login;
                userToUpdate.ModifiedOn = DateTime.Now;
            }

            return userToUpdate;
        }

        public async Task<Users> UpdateUserLoginAsync(string login, string loginToFind, string newLogin)
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

        public async Task<Users> UpdateUserPasswordAsync(string login, string loginToFind, string newPassword)
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
