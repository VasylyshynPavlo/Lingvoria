using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core;
using Data;
using Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace LingvoriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LingvoriaDbContext _context;
        private readonly PasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        
        public UsersController(IConfiguration configuration)
        {
            _context = new LingvoriaDbContext("mongodb://localhost:27017", "LingvoriaDb");
            _passwordHasher = new PasswordHasher();
            _configuration = configuration;
        }

        //--------------------P---O---S---T-----------------------
        #region Post
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            var user = await _context.Users.Find(u => u.Username == model.Username).FirstOrDefaultAsync();
            if (user == null || !_passwordHasher.VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
        
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserModel form, string password)
        {
            if (!ModelState.IsValid) return BadRequest();
            var existingUser = await _context.Users.Find(u => u.Username == form.Username).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return BadRequest("Username already taken.");
            }
            var user = new UserModel
            {
                AvatarUrl = form.AvatarUrl,
                Username = form.Username,
                NormalizedUsername = form.Username.ToUpper(),
                FullName = form.FullName,
                Email = form.Email,
                NormalizedEmail = form.Email.ToUpper(),
                PasswordHash = _passwordHasher.HashPassword(password),
                PhoneNumber = form.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            user.Id = user.Id;

            await _context.Users.InsertOneAsync(user);

            return Ok(user);
        }

        #endregion

        //------------------------G---E---T-----------------------
        #region Get 
        
        [HttpGet("/verify/user/password")]
        public async Task<IActionResult> VerifyPassword(string username, string password)
        {
            var user = await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (_passwordHasher.VerifyPassword(password, user.PasswordHash)) return Ok(true);
            else return Ok(false);
        }
        
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Отримуємо username із токена
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Invalid token or user not found.");
            }

            // Шукаємо користувача за username у базі даних
            var user = await _context.Users
                .Find(u => u.NormalizedUsername == username.ToUpper())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Повертаємо профіль користувача, приховавши конфіденційну інформацію
            var userProfile = new
            {
                user.Id,
                user.Username,
                user.AvatarUrl,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.TwoFactorEnabled
            };

            return Ok(userProfile);
        }

        
        #endregion
        
    }

}