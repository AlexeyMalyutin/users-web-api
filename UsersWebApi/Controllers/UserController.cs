using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersWebApi.Models;
using UsersWebApi.Repositories;

namespace UsersWebApi.Controllers
{
    /// <summary>
    /// Controller to perfoms CRUD operation on the user entity
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersRepository _repository;
        public UserController(IUsersRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Get all active users
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActiveUsers(string login, string password)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!user.Admin)
            {
                return BadRequest($"Пользователь {login} не является админом");
            }

            return Ok(await _repository.GetActiveUsersAsync(login, password));
        }

        /// <summary>
        /// Get user by his login
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToSearch">Login of the user that we need to find</param>
        /// <returns></returns>
        [HttpGet("{loginToSearch}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByLogin(string login,string password, string loginToSearch)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if(user==null)
            {
                return Unauthorized();
            }
            if(!user.Admin)
            {
                return BadRequest($"Пользователь {login} не является админом");
            }

            var userToFind = await _repository.GetByLoginAsync(loginToSearch);
            if(userToFind==null)
            {
                return BadRequest($"Пользователь {loginToSearch} не существует");
            }

            return Ok(userToFind);
        }

        /// <summary>
        /// Get user by his login and password
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByLoginAndPassword(string login, string password)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if(user == null)
            {
                return Unauthorized();
            }
            if (user.RevokedOn != null)
            {
                return BadRequest($"Пользователь {login} не активен");
            }

            return Ok(user);
        }

        /// <summary>
        /// Get users who are older than a specific age
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="age">specific age</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOlderThanAge(string login, string password, int age)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.RevokedOn != null || age < 0)
            {
                return BadRequest("Пользователь не активен или возраст введен некорректно");
            }

            var usersOlderThanAge = await _repository.GetOlderThanAgeAsync(age);
            return Ok(usersOlderThanAge);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="newUser">new user's data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateNewUser(string login, string password, User newUser)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!user.Admin)
            {
                return BadRequest($"Пользователь {login} не является админом"); 
            }

            await Task.FromResult(_repository.CreateNewUserAsync(login, newUser));
            return Created("",newUser);
        }

        /// <summary>
        /// Update user's name, gender or birthday
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToFind">Login of the user that we need to find</param>
        /// <param name="userData">User to update parameters</param>
        /// <returns></returns>
        [HttpPut("{loginToFind}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //string login, string password, string loginToFind, string newName, int? Gender, DateTime? Birthday
        public async Task<IActionResult> UpdateUserData(string login, string password, string loginToFind, User userData)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!user.Admin)
            {
                if (login != loginToFind || user.RevokedOn != null)
                    return BadRequest("Пароль может менять админ или лично пользователь, если он активен");
            }

            var userToUpdate = await _repository.UpdateUserDataAsync(login, loginToFind, userData);
            if(userToUpdate==null)
            {
                return NotFound($"Пользователь {loginToFind} не существует");
            }

            return NoContent();
        }

        /// <summary>
        /// Update user's password
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToFind">Login of the user that we need to find</param>
        /// <param name="newPassword">new user's password</param>
        /// <returns></returns>
        [HttpPut("{loginToFind}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserPassword(string login, string password, string loginToFind, string newPassword)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!user.Admin)
            {
                if (login != loginToFind || user.RevokedOn != null)
                {
                    return BadRequest("Пароль может менять админ или лично пользователь, если он активен");
                }
            }

            var userToUpdate = await _repository.UpdateUserPasswordAsync(login, loginToFind, newPassword);
            if (userToUpdate==null)
            {
                return NotFound($"Пользователь {loginToFind} не существует");
            }

            return NoContent();
        }

        /// <summary>
        /// Update user's login
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToFind">Login of the user that we need to find</param>
        /// <param name="newLogin">New user's login</param>
        /// <returns></returns>
        [HttpPut("{loginToFind}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserLogin(string login, string password, string loginToFind, string newLogin)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!user.Admin)
            {
                if (login != loginToFind || user.RevokedOn != null)
                    return BadRequest("Логин может менять админ или лично пользователь, если он активен");
            }

            var userToUpdate = await _repository.UpdateUserLoginAsync(login, loginToFind, newLogin);
            if (userToUpdate==null)
            {
                return NotFound($"Пользователь {loginToFind} не существует");
            }

            return NoContent();
        }

        /// <summary>
        /// Recover user's activity after soft deletion
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToFind">Login of the user that we need to find</param>
        /// <returns></returns>
        [HttpPut("{loginToFind}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RecoverUser(string login, string password, string loginToFind)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!user.Admin)
            {
                return BadRequest($"Пользователь {login} не является админом");
            }

            var userToRecover = await _repository.RecoverUserAsync(login, loginToFind);
            if (userToRecover==null)
            {
                return NotFound($"Пользователь {loginToFind} не существует");
            }

            return NoContent();
        }

        /// <summary>
        /// Delete the user softly (change his activity)
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToDelete">Login of the user that we need to find and delete</param>
        /// <param name="isSoft">Parameter that specifies how to delete the user (softly or completely)</param>
        /// <returns></returns>
        [HttpDelete("{loginToDelete}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(string login, string password, string loginToDelete, bool isSoft)
        {
            var user = await _repository.GetByLoginAndPasswordAsync(login, password);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!user.Admin)
            {
                return BadRequest($"Пользователь {login} не является админом");
            }
            
            var userToDelete = await _repository.DeleteUserAsync(login, loginToDelete, isSoft);
            if(userToDelete==null)
            {
                return NotFound($"Пользователь {loginToDelete} не существует");
            }
            
            return NoContent();
        }
    }
}
