using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Interfaces;
using Core.Models;
using Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Core.Services;

public class AccountService(PasswordHasher hasher, LingvoriaDbContext context, JwtService jwt) : IAccountService
{
    private static ConcurrentDictionary<string, string> activeTokens = new ConcurrentDictionary<string, string>();
    
    public async Task<bool> IsValidPassword(string password, string userId)
    {
        var user = await context.Users.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();
        if (user == null) return false;
        return hasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<Result> IsValidUser(string password, string? username, string? email)
    {
        UserModel user;
        if (username != null) user = await context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
        else if (email != null) user = await context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        else return new Result("400", "Username or Email is required.");
        if (!hasher.VerifyPassword(password, user.PasswordHash)) return new Result("403", "(Username or Email) or Password is invalid.");
        return new Result("200", "User is valid.", user);
    }
    public async Task<Result> Register(RegisterModel model)
    {
        var newUser = new UserModel
        {
            Id = ObjectId.GenerateNewId(),
            AvatarUrl = model.AvatarUrl,
            Username = model.Username,
            NormalizedUsername = model.Username.ToUpper(),
            FullName = model.FullName,
            Email = model.Email,
            NormalizedEmail = model.Email.ToUpper(),
            EmailConfirmed = false,
            PasswordHash = hasher.HashPassword(model.Password),
            LockoutEnabled = false,
            LockoutEnd = null,
            PhoneNumber = model.PhoneNumber,
            PhoneNumberConfirmed = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        await context.Users.InsertOneAsync(newUser);
        return new Result("200", "User registered", newUser);
    }

    public async Task<Result> Login(LoginModel model)
    {
        var result = await IsValidUser(model.Password, model.Username, model.Email);
        if (result.Code is "400" or "403") return result;
        else if(result.Code is not "200") return new Result("400", "Something went wrong.");
        var user = result.Data as UserModel;
        if (user == null) return new Result("404", "User does not exist.");
        
        var claims = jwt.GetClaims(user);
        
        var token = jwt.CreateToken(claims);
        activeTokens[user.Id.ToString()] = token;
        return new Result("200", "Login success", token);
    }

    public void Logout(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
    
        if (jsonToken == null)
        {
            throw new ArgumentException("Invalid token");
        }

        var userIdClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    
        if (userIdClaim == null)
        {
            throw new ArgumentException("Token does not contain a valid user ID.");
        }
    
        var userId = userIdClaim.Value;

        activeTokens.TryRemove(userId, out _);
    }
    
    public bool IsTokenActive(string userId)
    {
        return activeTokens.ContainsKey(userId);
    }
}