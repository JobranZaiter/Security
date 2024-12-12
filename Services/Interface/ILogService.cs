    public interface ILogService
    {
        void LogFailedLogin(string? userName, int? id, string message);
        void LogSuccessfulLogin(int? userId, string userName);
        void LogPageAccess(int? userId, string username, string page);
        void UnauthorizedAccess(int? userId, string username, string page);
        void LogTaskModify(int? userId, int taskId, string action);
        void LogFileAction(int userId, string fileName, string action);
    }