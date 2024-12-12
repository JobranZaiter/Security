using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

public class JwtValidationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<JwtValidationService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtValidationService(IConfiguration config, ILogger<JwtValidationService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly List<string> _roleHierarchy = new List<string> { "User", "It", "Admin", "Owner" };

    public ClaimsPrincipal ValidateToken(string token)
    {
        try
        {
            var secretKey = _config["JwtSettings:Secret"];
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = key
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Token validation failed: {ex.Message}");
            return null;
        }
    }

    public string GetUserRoleFromToken()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["authToken"];
        var principal = ValidateToken(token);

        if (principal == null)
            return "";

        var roleClaim = principal.FindFirst("Role");
        return roleClaim?.Value ?? "";
    }

    public int? GetIdFromToken()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["authToken"];

        if (string.IsNullOrEmpty(token))
        {
            return null;
        }
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return null;
            }

            var idClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "sub");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId) || userId <= 0)
            {
                return null;
            }
            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error extracting ID from token: {ex.Message}");
            return null;
        }
    }

    public bool VerifyRole(string requiredRole)
    {
        var userRole = GetUserRoleFromToken();
        int userRoleIndex = _roleHierarchy.IndexOf(userRole);
        if (userRoleIndex == -1)
        {
            _logger.LogError($"Role '{userRole}' is not found in the role hierarchy.");
            return false;
        }
        int requiredRoleIndex = _roleHierarchy.IndexOf(requiredRole);
        return userRoleIndex >= requiredRoleIndex;
    }

    public bool VerifyUser() => VerifyRole("User");
    public bool VerifyIT() => VerifyRole("It");
    public bool VerifyAdmin() => VerifyRole("Admin");
    public bool VerifyOwner() => VerifyRole("Owner");
}
