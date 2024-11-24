using Data;
using Data.Models;
using LingvoriaAPI.Engine;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LingvoriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LingvoriaDbContext _context;
        private readonly PasswordHasher _passwordHasher;

        public UsersController()
        {
            _context = new LingvoriaDbContext("mongodb://localhost:27017", "LingvoriaDb");
            _passwordHasher = new PasswordHasher();
        }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> CreateUser([FromForm] User form, string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string hashedPassword = _passwordHasher.HashPassword(password);

            var user = new User
            {
                AvatarUrl = form.AvatarUrl,
                Username = form.Username,
                NormalizedUsername = form.Username.ToUpper(),
                Email = form.Email,
                NormalizedEmail = form.Email.ToUpper(),
                PasswordHash = hashedPassword,
                PhoneNumber = form.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            await _context.Users.InsertOneAsync(user);

            return Ok(user);
        }
    }

}