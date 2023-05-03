using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess.Interfaces
{
    public interface IHobbyDatabase : IDatabase<Hobby>, IActivityDatabase<Hobby>
    {
    }
}