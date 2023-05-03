namespace TimeTrackingApp.Services.Interfaces
{
    public interface ITimerTrackerService
    {
        void StartTimer();

        void StopTimer();

        string GetTimeInMinutes();
    }
}