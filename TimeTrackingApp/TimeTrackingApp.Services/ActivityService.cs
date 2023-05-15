using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Helpers;
using TimeTrackingApp.Services.Interfaces;

namespace TimeTrackingApp.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IReadingDatabase _readingDatabase;
        private readonly IExercisingDatabase _exercisingDatabase;
        private readonly IWorkingDatabase _workingDatabase;
        private readonly IHobbyDatabase _hobbyDatabase;
        private readonly IUserManagerService _userManagerService;

        public ActivityService()
        {
        }

        public ActivityService(IReadingDatabase readingDatabase, IExercisingDatabase exercisingDatabase, IWorkingDatabase workingDatabase, IHobbyDatabase hobbyDatabase, IUserManagerService userManagerService)
        {
            _readingDatabase = readingDatabase;
            _exercisingDatabase = exercisingDatabase;
            _workingDatabase = workingDatabase;
            _hobbyDatabase = hobbyDatabase;
            _userManagerService = userManagerService;
        }

        public string GetFavoriteActivity()
        {
            List<ReadingActivity> readingActivities = _readingDatabase.GetActivityByUserId(_userManagerService.CurrentUser.Id);
            List<ExercisingActivity> exercisingActivities = _exercisingDatabase.GetActivityByUserId(_userManagerService.CurrentUser.Id);
            List<WorkingActivity> workingActivities = _workingDatabase.GetActivityByUserId(_userManagerService.CurrentUser.Id);
            List<Hobby> hobbyActivities = _hobbyDatabase.GetActivityByUserId(_userManagerService.CurrentUser.Id);

            Dictionary<string, int> activitiesByCategory = new Dictionary<string, int>
            {
                { "Reading", readingActivities.Count },
                { "Exercising", exercisingActivities.Count },
                { "Working", workingActivities.Count },
                { "Hobby", hobbyActivities.Count }
            };

            int maxCount = activitiesByCategory.Values.Max();
            IEnumerable<string> favoriteCategories = activitiesByCategory.Where(x => x.Value == maxCount).Select(x => x.Key);

            string favoriteActivities = string.Join(", ", favoriteCategories);
            TextHelper.TextGenerator($"User favorite activity: {favoriteActivities}", ConsoleColor.Cyan);

            return favoriteCategories.First();
        }

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