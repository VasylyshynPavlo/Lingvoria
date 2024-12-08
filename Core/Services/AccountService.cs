using Core.Interfaces;
using Core.Models;
using Core.Models.UserModels;
using Core.Models.UserModels.Get;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Services;

public class AccountService : IAccountService
    {
        private readonly PasswordHasher _hasher;
        private readonly LingvoriaDbContext _context;
        private readonly JwtService _jwtService;

        public AccountService(PasswordHasher hasher, LingvoriaDbContext context, JwtService jwtService)
        {
            _hasher = hasher;
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<bool> IsValidPassword(string password, string userId)
        {
            var user = await _context.Users.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();
            if (user == null) return false;
            return _hasher.VerifyPassword(password, user.PasswordHash);
        }

        public async Task<Response> IsValidUser(string password, string? username, string? email)
        {
            User? user = null;

            if (!string.IsNullOrEmpty(username))
            {
                user = await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
            }
            else if (!string.IsNullOrEmpty(email))
            {
                user = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
            }

            if (user == null) return new Response(404, "User not found.");

            if (!_hasher.VerifyPassword(password, user.PasswordHash))
                return new Response(403, "Invalid username/email or password.");

            return new Response(200, "User is valid.", user);
        }

        public async Task<Response> Register(RegisterModel model)
        {
            var existingUser = await _context.Users
                .Find(u => u.NormalizedUsername == model.Username.ToUpper() || u.NormalizedEmail == model.Email.ToUpper())
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return new Response(400, "Username or Email is already in use.");
            }

            var newUser = new User
            {
                Id = ObjectId.GenerateNewId(),
                AvatarUrl = model.AvatarUrl,
                Username = model.Username,
                NormalizedUsername = model.Username.ToUpper(),
                FullName = model.FullName,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                EmailConfirmed = false,
                PasswordHash = _hasher.HashPassword(model.Password),
                LockoutEnabled = false,
                LockoutEnd = null,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            await _context.Users.InsertOneAsync(newUser);
            return new Response(200, "User registered", newUser);
        }

        public async Task<Response> Login(LoginForm form)
        {
            var result = await IsValidUser(form.Password, form.Username, form.Email);

            if (result.Code == 400 || result.Code == 403)
                return result;

            if (result.Data is not User user)
                return new Response(404, "User does not exist.");

            var claims = _jwtService.GetClaims(user);
            var token = _jwtService.CreateToken(claims);

            return new Response(200, "Login success", token);
        }

        public async Task<Response> GetUsers()
        {
            var users = await _context.Users.Find(u => true).ToListAsync();
            var result = users.Select(user => new GetUser
            {
                Id = user.Id.ToString(),
                Avatar = user.AvatarUrl,
                Username = user.Username,
                FullName = user.FullName,
            }).ToList();

            return new Response(200, "Users retrieved", result);
        }
    }