using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly CmsDbContext context;

    public UserRepository(CmsDbContext _context)
    {
        context = _context;
    }
    public IEnumerable<User> GetUsersByRole(string role)
    {
        return context.Users
            .Where(u => u.UserRole == role)
            .ToList();

    }

    public IEnumerable<User> GetAllUsers()
    {
        return context.Users.Include(u => u.UserTasks)
        .Include(u => u.Logs).ToList();
    }

    public User GetUserByName(string username)
    {
        return context.Users
        .Where(u => u.UserName == username)
        .Include(u => u.UserTasks)
        .Include(u => u.Logs)
        .FirstOrDefault();
    }
    public User GetUserById(int id)
    {
        return context.Users
        .Where(u => u.Id == id)
        .Include(u => u.UserTasks)
        .Include(u => u.Logs)
        .FirstOrDefault();
    }


    public void AddUser(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }

    public void DeleteUser(User user)
    {
        context.Users.Remove(user);
        context.SaveChanges();
    }
    public void UpdateUser(User user)
    {
        context.Users.Update(user);
        context.SaveChanges();
    }

}
