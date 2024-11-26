using Data;
using Data.Models;
using LingvoriaAPI.Engine;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

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
        public async Task<IActionResult> CreateUser([FromForm] CreateUser form, string password)
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
                FullName = form.FullName,
                Email = form.Email,
                NormalizedEmail = form.Email.ToUpper(),
                PasswordHash = hashedPassword,
                PhoneNumber = form.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            user.Id = user.Id;

            await _context.Users.InsertOneAsync(user);

            return Ok(user);
        }

        [HttpGet("/verify/user/password")]
        public async Task<IActionResult> VerifyPassword(string username, string password)
        {
            var user = await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
            string hashedPassword = _passwordHasher.HashPassword(password);
            if (Pass) return Ok(true);
            else return Ok(false);
        }
    }

}