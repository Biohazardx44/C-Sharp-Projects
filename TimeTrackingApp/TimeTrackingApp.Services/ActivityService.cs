using TimeTrackingApp.Helpers;
using TimeTrackingApp.Services.Interfaces;

namespace TimeTrackingApp.Services
{
    public class ActivityService : IActivityService
    {
        public T GetActivityType<T>(string prompt, List<T> options, string errorMessage) where T : Enum
        {
            TextHelper.TextGenerator(prompt, ConsoleColor.Cyan);
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}){options[i]}");
            }

            int typeValue;
            while (!int.TryParse(Console.ReadLine(), out typeValue) || !Enum.IsDefined(typeof(T), typeValue))
            {
                TextHelper.TextGenerator(errorMessage, ConsoleColor.Yellow);
            }

            return (T)(object)typeValue;
        }
    }
}