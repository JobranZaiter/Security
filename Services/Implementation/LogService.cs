public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public LogService(ILogRepository logRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _logRepository = logRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public void LogFailedLogin(string? userName, int? id, string message)
    {
        var log = new Log
        {
            Entry = userName == null
                ? $"Unknown user tried to access account {message} {GetIp()}"
                : $"User {userName} tried to access account {message} {GetIp()}"
        };
        _logRepository.AddLog(log);
    }

    public void LogSuccessfulLogin(int? userId, string userName)
    {
        if (!userId.HasValue)
        {
            var log = new Log
            {
                Entry = $"Unknown user logged in {GetIp()}"
            };
            _logRepository.AddLog(log);
            return;
        }

        var user = _userRepository.GetUserById(userId.Value);
        if (user == null)
        {
            var log = new Log
            {
                Entry = $"Unknown user logged in {GetIp()}"
            };
            _logRepository.AddLog(log);
        }
        else
        {
            var log = new Log
            {
                Entry = $"User {user.UserName} with id {user.Id} logged in {GetIp()}",
                UserId = userId.Value,
            };
            _logRepository.AddLog(log);
        }
    }

    public void LogPageAccess(int? userId, string username, string page)
    {
        if (!userId.HasValue)
        {
            var log = new Log
            {
                Entry = $"Unknown user accessed page {page} {GetIp()}"
            };
            _logRepository.AddLog(log);
            return;
        }

        var user = _userRepository.GetUserById(userId.Value);
        if (user == null)
        {
            var log = new Log
            {
                Entry = $"Unknown user accessed page {page} {GetIp()}"
            };
            _logRepository.AddLog(log);
        }
        else
        {
            var log = new Log
            {
                Entry = $"User {username} accessed page {page} {GetIp()}",
                UserId = userId.Value,
            };
            _logRepository.AddLog(log);
        }
    }

    public void UnauthorizedAccess(int? userId, string username, string page)
    {
        if (!userId.HasValue)
        {
            var log = new Log
            {
                Entry = $"Unknown user tried accessing page {page} {GetIp()}"
            };
            _logRepository.AddLog(log);
            return;
        }

        var user = _userRepository.GetUserById(userId.Value);
        if (user == null)
        {
            var log = new Log
            {
                Entry = $"User with id {userId.Value} tried accessing page {page}, but user doesn't exist {GetIp()}"
            };
            _logRepository.AddLog(log);
        }
        else
        {
            var log = new Log
            {
                Entry = $"User {username} tried to access page {page}, but Unauthorized() {GetIp()}",
                UserId = userId.Value,
            };
            _logRepository.AddLog(log);
        }
    }

    public void LogTaskModify(int? userId, int taskId, string action)
    {
        if (!userId.HasValue)
        {
            var log = new Log
            {
                Entry = $"Unknown user performed {action} on task ID {taskId} {GetIp()}"
            };
            _logRepository.AddLog(log);
            return;
        }

        var user = _userRepository.GetUserById(userId.Value);
        if (user == null)
        {
            var log = new Log
            {
                Entry = $"User with id {userId.Value} performed {action} on task ID {taskId}, but user doesn't exist {GetIp()}"
            };
            _logRepository.AddLog(log);
        }
        else
        {
            var log = new Log
            {
                Entry = $"User {user.UserName} with id {userId.Value} performed {action} on task ID {taskId} {GetIp()}",
                UserId = userId.Value,
            };
            _logRepository.AddLog(log);
        }
    }
    public void LogFileAction(int userId, string fileName, string action)
    {
        var log = new Log
        {
            Entry = $"User {userId} {action} the file {fileName}.",
            UserId = userId
        };
        _logRepository.AddLog(log);
    }
    
    private string GetIp()
    {
        var ip = $"IP:({_httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString()})";
        return ip;
    }
}
