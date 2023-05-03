namespace TimeTrackingApp.DataAccess.Interfaces
{
    public interface IActivityDatabase<T>
    {
        T GetActivityById(int id);

        List<T> GetActivityByUserId(int id);
    }
}