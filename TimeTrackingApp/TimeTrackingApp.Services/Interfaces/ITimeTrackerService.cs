namespace TimeTrackingApp.Services.Interfaces
{
    public interface ITimeTrackerService
    {
        void StartTimer();

        void StopTimer();

        int GetTimeInSeconds();

        string GetTimeInMinutes(int durationInSeconds);

        string GetTimeInHours(int durationInSeconds);

        void ActivityTimeTracker(string activity);
    }
}