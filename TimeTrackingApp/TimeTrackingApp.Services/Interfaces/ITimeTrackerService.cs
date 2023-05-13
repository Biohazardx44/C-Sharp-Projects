namespace TimeTrackingApp.Services.Interfaces
{
    public interface ITimeTrackerService
    {
        void StartTimer();

        void StopTimer();

        int GetTimeInSeconds();

        string GetTimeInMinutes();

        string GetTimeInHours();
    }
}