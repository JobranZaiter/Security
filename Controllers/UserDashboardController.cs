using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class UserDashboardController : Controller
{
    private readonly ITaskRepository _taskRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly ILogService _logService;
    private readonly JwtValidationService _jwtValidationService;

    public UserDashboardController(
        ITaskRepository taskRepository, 
        IHttpContextAccessor httpContextAccessor, 
        IUserRepository userRepository,
        ILogService logService,
        JwtValidationService jwtValidationService)
    {
        _taskRepository = taskRepository;
        _httpContextAccessor = httpContextAccessor;
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
            _logService.UnauthorizedAccess(null, "Unknown", "User Dashboard");
            return RedirectToAction("Index", "Login");
        }

        var principal = _jwtValidationService.ValidateToken(token);
        if(principal==null){
            _logService.UnauthorizedAccess(null, "Unknown", "User Dashboard");
            return Unauthorized();
        }
        var id = _jwtValidationService.GetIdFromToken();
        var permissed = _jwtValidationService.VerifyRole("User");
        if(!permissed){
            _logService.UnauthorizedAccess(id, "Unknown", "User Dashboard");
            return Unauthorized();
        }
        if (id.HasValue)
        {
            var user = _userRepository.GetUserById(id.Value);
            
            if (user == null)
            {
                _logService.UnauthorizedAccess(id, "Unknown", "User Dashboard");
                return RedirectToAction("Index", "Login");
            }

            var tasks = _taskRepository.GetTasksByUserId(user.Id);
            return View(tasks);
        }
        else
        {
            _logService.UnauthorizedAccess(id, "Unknown", "User Dashboard");
            return RedirectToAction("Index", "Login");
        }
    }

    [HttpPost]
    public IActionResult StartTask(int taskId)
    {
        var task = _taskRepository.GetTaskById(taskId);
        if (task == null)
        {
            return NotFound();
        }

        var token = Request.Cookies["authToken"];
        var principal = _jwtValidationService.ValidateToken(token);

        if (principal == null)
        {
            _logService.UnauthorizedAccess(null, "Unknown", $"Start Task/{taskId}");
            return Unauthorized();
        }

        var id = _jwtValidationService.GetIdFromToken();
        var role = _jwtValidationService.GetUserRoleFromToken();

        if (id != task.UserId && role != "Admin" && role != "IT" && role != "Owner")
        {
            _logService.UnauthorizedAccess(id, "Unknown", $"Start Task/{taskId}");
            return Unauthorized();
        }

        task.Status = "Started";
        task.CreatedDate = DateTime.Now;
        _taskRepository.UpdateTask(task);

        _logService.LogTaskModify(id, taskId, "Started");

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult CompleteTask(int taskId)
    {
        var task = _taskRepository.GetTaskById(taskId);
        if (task == null)
        {
            return NotFound();
        }

        var token = Request.Cookies["authToken"];
        var principal = _jwtValidationService.ValidateToken(token);

        if (principal == null)
        {
            _logService.UnauthorizedAccess(null, "Unknown", $"Complete Task/{taskId}");
            return Unauthorized();
        }

        var id = _jwtValidationService.GetIdFromToken();
        var permissed = _jwtValidationService.VerifyRole("User");
        if(!permissed){
            _logService.UnauthorizedAccess(id, "Unknown", "User Dashboard");
        }

        if (id != task.UserId && !permissed)
        {
            _logService.UnauthorizedAccess(id, "Unknown", $"Complete Task/{taskId}");
            return Unauthorized();
        }

        task.Status = "Completed";
        task.FinishedDate = DateTime.Now;
        _taskRepository.UpdateTask(task);

        _logService.LogTaskModify(id, taskId, "Completed");

        return RedirectToAction("Index");
    }

}
