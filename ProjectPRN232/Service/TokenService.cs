using Microsoft.IdentityModel.Tokens;
using ProjectPRN232.Models;
using ProjectPRN232.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(SystemAccount account)
    {
        var claims = new[]
        {
           new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
            new Claim(ClaimTypes.Email, account.AccountEmail),
            //new Claim(ClaimTypes.Role, account.AccountRole.ToString())
            new Claim(ClaimTypes.Role, ((Role)account.AccountRole).ToString()) 
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
