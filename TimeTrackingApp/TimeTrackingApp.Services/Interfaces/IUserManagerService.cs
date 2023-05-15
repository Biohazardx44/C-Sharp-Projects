using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.Services.Interfaces
{
    public interface IUserManagerService
    {
        User CurrentUser { get; }

        void Register(string firstName, string lastName, int age, string username, string password);

        void LogIn(string userName, string password);

        void LogOut();

        Task<string> GetCurrentValidInputFromUser(string inputType, string expectedValue, Func<string> inputReader);

        Task<string> GetNewValidInputFromUser(string inputType);
    }
}