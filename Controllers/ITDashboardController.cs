using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ITDashboardController : Controller
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogService _logService;
    private readonly JwtValidationService _jwtValidationService;

    public ITDashboardController(ITaskRepository taskRepository, IUserRepository userRepository, ILogService logService, JwtValidationService jwtValidationService)
    {
        _taskRepository = taskRepository;
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
            return RedirectToAction("Index", "Login");
        }

        var principal = _jwtValidationService.ValidateToken(token);
        if (principal == null)
        {
            return Unauthorized();
        }

        var permissed = _jwtValidationService.VerifyRole("It");
        if (!permissed)
        {
            return Unauthorized();
        }

        var tasks = _taskRepository.GetAllTasks();
        var users = _userRepository.GetAllUsers();

        ViewBag.Tasks = tasks;
        ViewBag.Users = users;

        return View(tasks);
    }

    [HttpPost]
    public IActionResult AddTask(UserTask userTask)
    {
        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }

        var principal = _jwtValidationService.ValidateToken(token);
        if (principal == null)
        {
            return Unauthorized();
        }

        var permissed = _jwtValidationService.VerifyRole("It");
        if (!permissed)
        {
            return Unauthorized();
        }

        userTask.CreatedDate = DateTime.Now;
        userTask.Slug = userTask.Title.ToLower().Replace(" ", "-");
        _taskRepository.AddTask(userTask);

        return RedirectToAction("Index", "ITDashboard");
    }

    [HttpPost]
    public IActionResult EditTask(int taskId, int userId)
    {
        var task = _taskRepository.GetTaskById(taskId);
        if (task == null)
        {
            return NotFound();
        }

        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }

        var principal = _jwtValidationService.ValidateToken(token);
        if (principal == null)
        {
            return Unauthorized();
        }

        var permissed = _jwtValidationService.VerifyRole("It");
        if (!permissed)
        {
            return Unauthorized();
        }

        task.UserId = userId;
        _taskRepository.UpdateTask(task);

        _logService.LogTaskModify(userId, taskId, "Edit");

        return RedirectToAction("Index", "ITDashboard");
    }

    [HttpPost]
    public IActionResult RemoveTask(int taskId)
    {
        var task = _taskRepository.GetTaskById(taskId);
        if (task == null)
        {
            return NotFound();
        }

        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }

        var principal = _jwtValidationService.ValidateToken(token);
        if (principal == null)
        {
            return Unauthorized();
        }

        var permissed = _jwtValidationService.VerifyRole("It");
        if (!permissed)
        {
            return Unauthorized();
        }

        _taskRepository.DeleteTask(task);

        _logService.LogTaskModify(task.UserId, taskId, "Remove");

        return RedirectToAction("Index", "ITDashboard");
    }
}
