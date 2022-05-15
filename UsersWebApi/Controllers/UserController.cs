using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersWebApi.Models;

namespace UsersWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static List<User> users = new List<User>
        {
            new User
            {
                Guid = Guid.NewGuid(),
                Login = "Login",
                Password = "Password",
                Name = "Admin",
                Gender = 2,
                Admin = true,
                CreatedOn = DateTime.Now
            }
        };

        //GET: api/<UserController>
        [HttpGet("{login}/{password}", Name = "GetActiveUsers")]
        public IActionResult Get(string login, string password)
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

        //// GET api/<UserController>/5
        //[HttpGet("{id}")]
        //public IActionResult Get(Guid id)
        //{
        //    var user = users.SingleOrDefault(u => u.Guid == id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(user);
        //}

        // POST api/<UserController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<UserController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
