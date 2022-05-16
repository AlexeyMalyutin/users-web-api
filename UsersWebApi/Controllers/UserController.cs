using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UsersWebApi.Models;

namespace UsersWebApi.Controllers
{
    /// <summary>
    /// Controller to perfoms CRUD operation on the user entity
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static List<User> users = new List<User>
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

        /// <summary>
        /// Get all active users
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult GetActiveUsers(string login, string password)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.Admin)
            {
                return BadRequest();
            }

            return Ok(users.Where(u => u.RevokedOn == null).OrderByDescending(u => u.CreatedOn));
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult GetByLogin(string login,string password, string loginToSearch) ///А если такого пользователя не нашлось(((
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if(user==null)
            {
                return NotFound();
            }
            if(!user.Admin)
            {
                return BadRequest();
            }

            var userToFind = users
                .Where(u => u.Login == loginToSearch) //проверить........логина такого может не быть(
                .Select(x => new { x.Name, x.Gender, x.Birthday, x.RevokedOn });

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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult GetByLoginAndPassword(string login, string password)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if(user == null)
            {
                return NotFound();
            }
            if (user.RevokedOn != null)
            {
                return BadRequest();
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult GetOlderThanAge(string login, string password, int age)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (user.RevokedOn != null || age < 0)
            {
                return BadRequest();
            }

            var usersOlderThanAge = users
                .Where(u => (u.Birthday.HasValue ? u.Birthday.Value : DateTime.MaxValue) < DateTime.Today.AddYears(-age));

            return Ok(usersOlderThanAge);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="newUser">..............</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        /* string newLogin, string newPassword, string newName, int newGender, DateTime newBirthday, bool isAdmin)
        [Bind("Login","Password","Name","Gender","Birthday","Admin")]*/
        public IActionResult CreateNewUser(string login, string password,[FromQuery, Required] User newUser)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.Admin)
            {
                return BadRequest();
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            newUser.Guid = Guid.NewGuid();
            newUser.CreatedBy = login;
            newUser.CreatedOn = DateTime.Now;
            newUser.RevokedBy = null;
            newUser.RevokedOn = null;
            newUser.ModifiedBy = null;
            newUser.ModifiedOn = null;

            users.Add(newUser);
            return Created("",newUser);
        }

        /// <summary>
        /// Update user's name, gender or birthday
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToFind">Login of the user that we need to find</param>
        /// <param name="newName">New user's name</param>
        /// <param name="Gender">New user's gender</param>
        /// <param name="Birthday">New user's Birthday</param>
        /// <returns></returns>
        [HttpPut("{loginToFind}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult UpdateUserData(string login, string password, string loginToFind, string newName, int? Gender, DateTime? Birthday)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.Admin)
            {
                if (login != loginToFind || user.RevokedOn != null)
                    return BadRequest();
            }

            var userToUpdate = users.FirstOrDefault(u => u.Login == loginToFind);
            if(newName!=null) userToUpdate.Name = newName;
            if(Gender!=null) userToUpdate.Gender = (int)Gender;
            if(Birthday!=null) userToUpdate.Birthday = Birthday;
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult UpdateUserPassword(string login, string password, string loginToFind, string newPassword)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.Admin)
            {
                if (login != loginToFind || user.RevokedOn != null)
                    return BadRequest();
            }

            var userToUpdate = users.FirstOrDefault(u => u.Login == loginToFind);
            userToUpdate.Password = newPassword;
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
        [HttpPut("{loginToFild}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult UpdateUserLogin(string login, string password, string loginToFind, string newLogin)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.Admin)
            {
                if (login != loginToFind || user.RevokedOn != null)
                    return BadRequest();
            }

            var userToUpdate = users.FirstOrDefault(u => u.Login == loginToFind);
            userToUpdate.Login = newLogin;
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult RecoverUser(string login, string password, string loginToFind)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.Admin)
            {
                return BadRequest();
            }

            var userToRecover = users.FirstOrDefault(u => u.Login == loginToFind);
            userToRecover.RevokedOn = null;
            userToRecover.RevokedBy = null;
            return NoContent();
        }

        /// <summary>
        /// Delete the user softly (change his activity)
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToDelete">Login of the user that we need to find and delete</param>
        /// <returns></returns>
        [HttpDelete("{loginToDelete}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult DeleteSoftly(string login, string password, string loginToDelete)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.Admin)
            {
                return BadRequest();
            }

            var userToDelete = users.FirstOrDefault(u => u.Login == loginToDelete);
            if(userToDelete==null)
            {
                return BadRequest();
            }

            userToDelete.RevokedOn = DateTime.Now;
            userToDelete.RevokedBy = login;
            return Ok(users);
        }

        /// <summary>
        /// Delete the user
        /// </summary>
        /// <param name="login">Login of the person executing the request</param>
        /// <param name="password">Password of the person executing the request</param>
        /// <param name="loginToDelete">Login of the user that we need to find and delete</param>
        /// <returns></returns>
        [HttpDelete("{loginToDelete}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult DeleteCompletely(string login, string password, string loginToDelete)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return NotFound();
            }
            if (!user.Admin)
            {
                return BadRequest();
            }

            var userToDelete = users.FirstOrDefault(u => u.Login == loginToDelete);
            if (userToDelete == null)
            {
                return BadRequest();
            }

            users.Remove(userToDelete);
            return NoContent();
        }
    }
}
