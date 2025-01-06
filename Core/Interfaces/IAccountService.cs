using Core.Models;
using Core.Models.UserModels;

namespace Core.Interfaces;

public interface IAccountService
{
    Task<bool> IsValidPassword(string password, string userId);
    Task<Response> Register(RegisterForm form);
    Task<Response> Login(LoginForm form);
    Task<Response> GetShortUserInfo(string userId);
    Task<Response> UpdateUserInfo(string? avatarUrl, string? username, string? fullName, string? email, string userId);
    Task<Response> GetMyId(string userId);
    Task<Response> GetUsers();
}