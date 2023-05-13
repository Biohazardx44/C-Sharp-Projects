using System.Timers;
using TimeTrackingApp.Services.Interfaces;

namespace TimeTrackingApp.Services
{
    public class TimeTrackerService : ITimerTrackerService
    {
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(1000);
        private int _seconds;

        public void StartTimer()
        {
            _seconds = 0;
            _timer.Elapsed -= OnTimerElapsed;

            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ClearConsoleLineIfNecessary();
            IncrementTimer();
            PrintTimeInSeconds();
        }

        private void ClearConsoleLineIfNecessary()
        {
            if (_seconds >= 1)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }

        private void IncrementTimer()
        {
            _seconds++;
        }

        private void PrintTimeInSeconds()
        {
            Console.WriteLine(_seconds);
        }

        public int GetTimeInSeconds()
        {
            return _seconds;
        }

        public string GetTimeInMinutes()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_seconds);
            return $"{(int)timeSpan.TotalMinutes} minutes & {timeSpan.Seconds:D2} seconds.";
        }

        public string GetTimeInHours()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_seconds);
            return $"{(int)timeSpan.TotalHours} hours, {timeSpan.Minutes:D2} minutes & {timeSpan.Seconds:D2} seconds.";
        }
    }
}