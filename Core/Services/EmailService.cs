using System.Text;
using Core.Interfaces;
using Core.Models;
using Core.Models.UserModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Services;

public class EmailService(LingvoriaDbContext context) : IEmailService
{
    public async Task<Response> SendVerificationEmail(string userId)
    {
        var user = await context.Users.Find(u => u.Id.ToString().Equals(userId)).FirstOrDefaultAsync();
        if (user == null) return new Response(404, "User not found");
        var code = await GenerateRandomCode(6);
        await context.EmailCodes.InsertOneAsync(new EmailCode(userId, code));
        return new Response(200, "Email code send", code);
    }

    public async Task<Response> VerificateEmail(string code, string userId)
    {
        var user = await context.Users.Find(u => u.Id.ToString() == userId).FirstOrDefaultAsync();

        if (user == null) return new Response(404, "User not found");
        var Code = await context.EmailCodes.Find(emailCode => emailCode.Code == code).FirstOrDefaultAsync();
        //var originalCode = await context.EmailCodes.Find(c => c.UserId == userId).FirstOrDefaultAsync();
        // if (originalCode == null) return new Response(404, "Email confirmation failed");
        // if (originalCode.ValidUntil < DateTime.Now) return new Response(404, "Email confirmation failed");
        // if (originalCode.UserId != userId) return new Response(404, "Verification failed");
        // if (originalCode.Code != code) return new Response(403, "Verification failed");
        // user.EmailConfirmed = true;
        // await context.Users.ReplaceOneAsync(u => u.Id.ToString().Equals(userId), user);
        return new Response(200, "Email verified");
    }

    private Task<string> GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder result = new StringBuilder(length);
        Random random = new Random();

        for (var i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return Task.FromResult(result.ToString());
    }
}