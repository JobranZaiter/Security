using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class JwtToken
{
    private readonly IConfiguration config;
    private readonly ILogger<JwtToken> logger;

    public JwtToken(IConfiguration _config, ILogger<JwtToken> _logger)
    {
        config = _config;
        logger = _logger;
    }

    public string GenerateToken(int Id, string role)
    {
        var secretKey = config["JwtSettings:Secret"];
        var issuer = config["JwtSettings:Issuer"];
        var audience = config["JwtSettings:Audience"];
        var expiryInMinutes = int.Parse(config["JwtSettings:ExpiryInMinutes"]);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]{
            new Claim(JwtRegisteredClaimNames.Sub, Id.ToString()),
            new Claim("Role", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: sign
        );
        logger.LogInformation("Token" + token);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}