

using Microsoft.EntityFrameworkCore;

public class TaskRepository : ITaskRepository
{

    private readonly CmsDbContext context;

    public TaskRepository(CmsDbContext _context)
    {
        context = _context;
    }

    public void AddTask(UserTask task)
    {
        context.UserTasks.Add(task);
        context.SaveChanges();
    }

    public void DeleteTask(UserTask task)
    {
        context.UserTasks.Remove(task);
        context.SaveChanges();
    }

    public IEnumerable<UserTask> GetAllTasks()
    {
        return context.UserTasks
                .Include(t => t.User)
                .ToList();
    }

    public UserTask GetTaskById(int id)
    {
        return context.UserTasks
        .Where((t => t.Id == id))
        .FirstOrDefault();
    }

    public IEnumerable<UserTask> GetTasksByUserName(string username)
    {
        return context.UserTasks
        .Where(t => t.User.UserName == username)
        .ToList();
    }
    public IEnumerable<UserTask> GetTasksByUserId(int id)
    {
        return context.UserTasks
        .Where(t => t.User.Id == id)
        .ToList();
    }

    public void UpdateTask(UserTask task)
    {
        context.UserTasks.Update(task);
        context.SaveChanges();
    }
}