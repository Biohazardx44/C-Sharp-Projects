using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess.Interfaces
{
    public interface IReadingDatabase : IDatabase<ReadingActivity>, IActivityDatabase<ReadingActivity>
    {
    }
}