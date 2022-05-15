using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersWebApi.Models;

namespace UsersWebApi.Controllers
{
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

        [HttpGet]
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

            return Ok(users.Where(u => u.RevokedOn == null).OrderBy(u => u.CreatedOn));
        }

        [HttpGet("{loginToSearch}")]
        public IActionResult GetByLogin(string login,string password, string loginToSearch)
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
                .Where(u => u.Login == loginToSearch)
                .Select(x => new { x.Name, x.Gender, x.Birthday, x.RevokedOn });

            return Ok(userToFind);
        }

        [HttpGet]
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

        [HttpGet]
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

        [HttpPost]
        public IActionResult CreateNewUser(string login, string password, string newLogin, 
            string newPassword, string newName, int newGender, DateTime newBirthday, bool isAdmin)
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

            var userToAdd = new User
            {
                Guid = Guid.NewGuid(),
                Login = newLogin,
                Password = newPassword,
                Name = newName,
                Gender = newGender,
                Birthday = newBirthday,
                Admin = isAdmin,
                CreatedOn = DateTime.Now,
                CreatedBy = login
            };

            users.Add(userToAdd);
            return StatusCode(201);
        }

        //// PUT
        //[HttpPut]
        //public IActionResult Put(string login, string password, string loginToFind)
        //{
        //    var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    if (!user.Admin)
        //    {
        //        if(login != loginToFind || user.RevokedOn != null)
        //            return BadRequest();
        //    }

        //    ///Изменить имя, пол и дату рождения.
        //}


        //// DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
