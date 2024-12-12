using System.Collections.Generic;

public interface ITaskRepository
{
    UserTask GetTaskById(int id);
    IEnumerable<UserTask> GetTasksByUserName(string username);
    IEnumerable<UserTask> GetTasksByUserId(int id);
    IEnumerable<UserTask> GetAllTasks();
    void AddTask(UserTask task);
    void UpdateTask(UserTask task);
    void DeleteTask(UserTask task);
}
