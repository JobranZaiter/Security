using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

public class LogRepository : ILogRepository
{
    private readonly CmsDbContext _context;

    public LogRepository(CmsDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Log> GetAllLogs()
    {
        return _context.Logs.Include(l => l.User).ToList();
    }

    public Log GetLogById(int id)
    {
        return _context.Logs.Include(l => l.User)
                            .FirstOrDefault(l => l.Id == id);
    }

    public IEnumerable<Log> GetLogsByUserId(int userId)
    {
        return _context.Logs.Include(l => l.User)
                            .Where(l => l.UserId == userId)
                            .ToList();
    }

    public void AddLog(Log log)
    {
        _context.Logs.Add(log);
        _context.SaveChanges();
    }

    public void UpdateLog(Log log)
    {
        _context.Logs.Update(log);
        _context.SaveChanges();
    }

    public void DeleteLog(int id)
    {
        var log = _context.Logs.Find(id);
        if (log != null)
        {
            _context.Logs.Remove(log);
            _context.SaveChanges();
        }
    }
}
