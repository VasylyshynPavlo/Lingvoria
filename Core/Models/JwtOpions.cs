namespace Core.Models;

public class JwtOpions
{
    public string Key { get; set; }
    public int LifetimeInMinutes { get; set; }
    public string Issuer { get; set; }
}