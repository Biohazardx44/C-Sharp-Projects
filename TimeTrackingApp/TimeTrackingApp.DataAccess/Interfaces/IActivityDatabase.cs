using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess.Interfaces
{
    public interface IActivityDatabase<T>
    {
        T GetActivityById(int id);

        List<T> GetActivityByUserId(int id);

        Task AddActivityAsync(T activity);

        Task DeleteActivityAsync(int id);

        Task UpdateActivityAsync(T activity);
    }
}