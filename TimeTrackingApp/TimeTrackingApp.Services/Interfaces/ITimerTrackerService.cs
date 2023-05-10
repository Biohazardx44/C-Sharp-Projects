namespace TimeTrackingApp.Services.Interfaces
{
    public interface ITimerTrackerService
    {
        void StartTimer();

        void StopTimer();

        int GetTimeInSeconds();

        string GetTimeInMinutes();

        string GetTimeInHours();
    }
}