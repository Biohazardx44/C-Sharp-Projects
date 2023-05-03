using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess.Interfaces
{
    public interface IWorkingDatabase : IDatabase<WorkingActivity>, IActivityDatabase<WorkingActivity>
    {
    }
}