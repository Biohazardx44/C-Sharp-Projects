using System.Timers;
using TimeTrackingApp.Helpers;
using TimeTrackingApp.Services.Interfaces;

namespace TimeTrackingApp.Services
{
    public class TimeTrackerService : ITimeTrackerService
    {
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(1000);
        private int _seconds;

        /// <summary>
        /// Starts the timer and sets the seconds count to 0.
        /// </summary>
        public void StartTimer()
        {
            _seconds = 0;
            _timer.Elapsed -= OnTimerElapsed;

            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Callback function for the timer elapsed event. Increases the seconds count and prints it to the console.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ClearConsoleLineIfNecessary();
            _seconds++;
            Console.WriteLine(_seconds);
        }

        /// <summary>
        /// Clears the current console line if the timer has been running for at least 1 second.
        /// </summary>
        private void ClearConsoleLineIfNecessary()
        {
            if (_seconds >= 1)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }

        /// <summary>
        /// Gets the current time count in seconds.
        /// </summary>
        /// <returns>The current time count in seconds.</returns>
        public int GetTimeInSeconds()
        {
            return _seconds;
        }

        /// <summary>
        /// Gets the current time count in minutes and seconds.
        /// </summary>
        /// <param name="durationInSeconds">The duration of the activity in seconds.</param>
        /// <returns>A string representing the current time count in minutes and seconds.</returns>
        public string GetTimeInMinutes(int durationInSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(durationInSeconds);
            return $"{(int)timeSpan.TotalMinutes} minutes & {timeSpan.Seconds:D2} seconds";
        }

        /// <summary>
        /// Gets the current time count in hours, minutes and seconds.
        /// </summary>
        /// <param name="durationInSeconds">The duration of the activity in seconds.</param>
        /// <returns>A string representing the current time count in hours, minutes, and seconds.</returns>
        public string GetTimeInHours(int durationInSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(durationInSeconds);
            return $"{(int)timeSpan.TotalHours} hours, {timeSpan.Minutes:D2} minutes & {timeSpan.Seconds:D2} seconds";
        }

        /// <summary>
        /// Starts the timer and displays a message indicating that the activity has begun.
        /// Stops the timer and displays a message indicating that the activity has ended when the user presses ENTER.
        /// </summary>
        /// <param name="activity">The name of the activity to track.</param>
        public void ActivityTimeTracker(string activity)
        {
            TextHelper.TextGenerator($"\nTimer has started & {activity} has begun!", ConsoleColor.Green);
            StartTimer();

            TextHelper.TextGenerator($"Press ENTER when you want to stop {activity}", ConsoleColor.Cyan);
            Console.ReadLine();
            StopTimer();
        }
    }
}