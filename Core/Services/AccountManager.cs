using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Core.Models.UserModels;
using Core.Models.UserModels.Activity;
using Core.Models.UserModels.Get;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Services;

public class AccountManager(PasswordHasher hasher, LingvoriaDbContext context, JwtService jwtService, IMapper mapper)
    : IAccountService
{
    public async Task<bool> IsValidPassword(string password, string userId)
    {
        var user = await context.Users.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();
        if (user == null) return false;
        return hasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<Response> Register(RegisterForm form)
    {
        var existingUser = await context.Users
            .Find(u => u.NormalizedUsername.Equals(form.Username, StringComparison.CurrentCultureIgnoreCase) ||
                       u.NormalizedEmail.Equals(form.Email, StringComparison.CurrentCultureIgnoreCase))
            .FirstOrDefaultAsync();

        if (existingUser != null) return new Response(400, "Username or Email is already in use.");

        var newUser = new User
        {
            Id = ObjectId.GenerateNewId(),
            AvatarUrl = form.AvatarUrl,
            Username = form.Username,
            NormalizedUsername = form.Username.ToUpper(),
            FullName = form.FullName,
            Email = form.Email,
            NormalizedEmail = form.Email.ToUpper(),
            EmailConfirmed = false,
            PasswordHash = hasher.HashPassword(form.Password),
            LockoutEnabled = false,
            LockoutEnd = null,
            PhoneNumber = form.PhoneNumber,
            PhoneNumberConfirmed = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            Logs = []
        };

        await context.Users.InsertOneAsync(newUser);
        var response = await AddLog(new UserLog("Register"), newUser.Id.ToString());
        return response.Code != 200
            ? new Response(500, "User was created but we can't find anyone.")
            : new Response(200, "User registered", mapper.Map<ShortUser>(newUser));
    }

    public async Task<Response> Login(LoginForm form)
    {
        var result = await IsValidUser(form.Password, form.UsernameOrEmail);

        if (result.Code == 403)
            return result;

        if (result.Data is not User user)
            return new Response(404, "User does not exist.");

        var claims = jwtService.GetClaims(user);
        var token = jwtService.CreateToken(claims);

        var existingUser = await context.Users
                               .Find(u => u.Username.Equals(form.UsernameOrEmail,
                                   StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync() ??
                           await context.Users.Find(u =>
                                   u.Email.Equals(form.UsernameOrEmail, StringComparison.CurrentCultureIgnoreCase))
                               .FirstOrDefaultAsync();
        if (existingUser == null) return new Response(404, "Username not found");
        var response = await AddLog(new UserLog("Login"), existingUser.Id.ToString());
        return response.Code != 200
            ? new Response(500, "User was created but we can't find anyone.")
            : new Response(200, "Login success", token);
    }

    public async Task<Response> GetUsers()
    {
        var users = await context.Users.Find(u => true).ToListAsync();
        var result = users.Select(mapper.Map<ShortUser>).ToList();

        return new Response(200, "Users retrieved", result);
    }

    public async Task<Response> GetShortUserInfo(string userId)
    {
        var user = await context.Users.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();
        if (user == null) return new Response(404, "User does not exist.");
        return new Response(200, "User retrieved", new
        {
            AvatarUrl = user.AvatarUrl,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
        });
    }

    public async Task<Response> UpdateUserInfo(string avatarUrl, string username, string fullName, string email,
        string userId)
    {
        var user = await context.Users.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();
        if (user == null) return new Response(404, "User does not exist.");
        user.AvatarUrl = avatarUrl;
        user.Username = username;
        user.FullName = fullName;
        user.Email = email;
        await context.Users.ReplaceOneAsync(u => u.Id == new ObjectId(userId), user);
        return new Response(200, "User updated");
    }

    private async Task<Response> IsValidUser(string password, string usernameOrEmail)
    {
        User? user = null;
        user = await context.Users
                   .Find(u => u.Username.Equals(usernameOrEmail, StringComparison.CurrentCultureIgnoreCase))
                   .FirstOrDefaultAsync() ??
               await context.Users.Find(u => u.Email.Equals(usernameOrEmail, StringComparison.CurrentCultureIgnoreCase))
                   .FirstOrDefaultAsync();


        if (user == null) return new Response(404, "User not found.");

        return !hasher.VerifyPassword(password, user.PasswordHash)
            ? new Response(403, "Invalid username/email or password.")
            : new Response(200, "User is valid.", user);
    }

    private async Task<Response> AddLog(UserLog log, string userId)
    {
        var user = await context.Users.Find(u => u.Id.ToString() == userId).FirstOrDefaultAsync();
        if (user == null) return new Response(404, "User does not exist.");
        user.Logs.Add(log);
        await context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
        return new Response(200, "Log added", user);
    }
}