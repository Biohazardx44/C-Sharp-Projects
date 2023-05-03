using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.Services.Interfaces
{
    public interface IUserManagerService
    {
        User CurrentUser { get; }

        void LogIn(string userName, string password);

        void Register(string firstName, string lastName, int age, string username, string password);

        void LogOut();
    }
}