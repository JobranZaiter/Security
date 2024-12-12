using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Linq;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ILogService _logService;
    private readonly JwtValidationService _jwtValidationService;

    public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, ILogService logService, JwtValidationService jwtValidationService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _logService = logService;
        _jwtValidationService = jwtValidationService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var token = Request.Cookies["authToken"];

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Token NULL");
            _logService.UnauthorizedAccess((int?)null, "Unknown", "Home Page");
            return RedirectToAction("Index", "Login");
        }
        var principal = _jwtValidationService.ValidateToken(token);
        if(principal==null)
        {
            _logger.LogWarning("Principal NULL");
            _logService.UnauthorizedAccess((int?)null, "Unknown", "Home Page");
            return Unauthorized();
        }

        var userRole = _jwtValidationService.GetUserRoleFromToken();
        var userId = _jwtValidationService.GetIdFromToken();

        if (!userId.HasValue || string.IsNullOrEmpty(userRole))
        {
            _logger.LogWarning("ID NULL");
            _logService.UnauthorizedAccess(null, "Unknown", "Home Page");
            return Unauthorized();
        }

        var user = _userRepository.GetUserById(userId.Value);
        if (user == null)
        {
            _logger.LogWarning("USER NULL");
            _logService.UnauthorizedAccess(null, "Unknown", "Home Page");
            return Unauthorized();
        }

        _logService.LogPageAccess(user.Id, user.UserName, "Home Page");

        if (userRole == "User")
        {
            return RedirectToAction("Index", "UserDashboard");
        }
        else if (userRole == "It")
        {
            return RedirectToAction("Index", "ITDashboard");
        }
        else if (userRole == "Admin" || userRole == "Owner")
        {
            return RedirectToAction("Index", "AdminDashboard");
        }
        else
        {
            _logger.LogWarning("Role not formatted");
            _logService.UnauthorizedAccess(user.Id, user.UserName, "Home Page");
            return Unauthorized();
        }
    }
}
