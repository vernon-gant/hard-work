using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MotorPool.API.Tests;

public static class AuthHelper
{
    public static readonly string SecretKey = "test-secret-key-1234567890-1234567890-1234567890-1234567890";
    public static readonly string Issuer = "test-issuer";
    public static readonly string Audience = "test-audience";

    public static string GenerateJwtToken(int managerId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
                              {
                                  Subject = new ClaimsIdentity([
                                      new Claim("ManagerId", managerId.ToString())
                                  ]),
                                  Expires = DateTime.UtcNow.AddHours(1),
                                  Issuer = Issuer,
                                  Audience = Audience,
                                  SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                              };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}