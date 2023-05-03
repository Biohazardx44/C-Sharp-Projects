using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess.Interfaces
{
    public interface IUserDatabase : IDatabase<User>
    {
        User GetUserByUsernameAndPassword(string username, string password);

        bool CheckUsernameAvailable(string username);
    }
}