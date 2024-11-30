using Core.Models;

namespace Core.Interfaces;

public interface IAccountService
{
    Task<bool> IsValidPassword(string password, string userId);
    Task<Result> Register(RegisterModel model);
    Task<Result> Login(LoginModel model);
}