using System.Collections.Generic;

public interface IUserRepository
{
    User GetUserById(int id);
    User GetUserByName(string username);
    IEnumerable<User> GetAllUsers();
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
    IEnumerable<User> GetUsersByRole(string role);
}
