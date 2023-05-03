namespace TimeTrackingApp.Services.Interfaces
{
    public interface IUserService<T>
    {
        void AddActivity(T activity);
    }
}