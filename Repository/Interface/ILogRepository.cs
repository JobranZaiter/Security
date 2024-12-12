using System.Collections.Generic;

public interface ILogRepository
{
    IEnumerable<Log> GetAllLogs();
    Log GetLogById(int id);
    IEnumerable<Log> GetLogsByUserId(int userId);
    void AddLog(Log log);
    void UpdateLog(Log log);
    void DeleteLog(int id);
}
