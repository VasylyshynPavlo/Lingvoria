using System.Security.Claims;
using Core.Models;

namespace Core.Interfaces;

public interface IJwtService
{
    IEnumerable<Claim> GetClaims(UserModel user);
    string CreateToken(IEnumerable<Claim> claims);
}