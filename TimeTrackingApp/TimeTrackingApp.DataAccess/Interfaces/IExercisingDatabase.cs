using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess.Interfaces
{
    public interface IExercisingDatabase : IDatabase<ExercisingActivity>, IActivityDatabase<ExercisingActivity>
    {
    }
}