namespace TimeTrackingApp.Services.Interfaces
{
    public interface IActivityService
    {
        T GetActivityType<T>(string prompt, List<T> options, string errorMessage) where T : Enum;
    }
}