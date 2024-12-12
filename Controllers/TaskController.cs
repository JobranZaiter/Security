using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

[Controller]
[Route("Tasks")]
public class TaskController : Controller
{

    private readonly IUserRepository userRepository;
    private readonly ITaskRepository taskRepository;
    private readonly PasswordManager passwordManager;
    private readonly JwtToken jwtTokenGen;

    public TaskController(IUserRepository _userRepository, PasswordManager _passwordManager, JwtToken _jwtTokenGen, ITaskRepository _taskRepository)
    {
        this.userRepository = _userRepository;
        this.taskRepository = _taskRepository;
        this.passwordManager = _passwordManager;
        this.jwtTokenGen = _jwtTokenGen;
    }

    [HttpPost]
    [Authorize]
    public IActionResult StartTask(int taskId)
    {
        var task = taskRepository.GetTaskById(taskId);
        task.Status = "Started";
        taskRepository.UpdateTask(task);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult CompleteTask(int taskId)
    {
        var task = taskRepository.GetTaskById(taskId);
        task.Status = "Completed";
        taskRepository.UpdateTask(task);

        return RedirectToAction("Index", "Home");
    }
}