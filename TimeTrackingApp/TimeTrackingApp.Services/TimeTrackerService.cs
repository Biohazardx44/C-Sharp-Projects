using System.Timers;
using TimeTrackingApp.Services.Interfaces;

namespace TimeTrackingApp.Services
{
    public class TimeTrackerService : ITimerTrackerService
    {
        private readonly System.Timers.Timer _timer = new(interval: 1000);
        private int _seconds = 0;

        public void StartTimer()
        {
            _seconds = 0;

            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            SetAndClear();
            HandleTimer();
        }

        private void HandleTimer()
        {
            _seconds += 1;
            Console.WriteLine(_seconds.ToString());
        }

        public string GetTimeInMinutes()
        {
            return TimeSpan.FromSeconds(_seconds).ToString() + " seconds";
        }

        private void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void SetAndClear()
        {
            if (_seconds >= 1)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
            }
        }
    }
}