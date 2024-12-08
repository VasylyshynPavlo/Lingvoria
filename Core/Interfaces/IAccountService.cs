using Core.Models;
using Core.Models.UserModels;

namespace Core.Interfaces;

public interface IAccountService
{
    Task<bool> IsValidPassword(string password, string userId);
    Task<Response> Register(RegisterModel model);
    Task<Response> Login(LoginForm form);

    Task<Response> GetUsers();
}