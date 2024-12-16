using Core.Models;

namespace Core.Interfaces;

public interface IEmailService
{
    Task<Response> SendVerificationEmail(string userId);
    Task<Response> VerificateEmail(string code, string userId);
}