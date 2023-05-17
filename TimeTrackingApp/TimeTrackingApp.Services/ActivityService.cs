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

        /// <summary>
        /// Returns the favorite activity of the current user based on the count of each activity category
        /// retrieved from different databases such as Reading, Exercising, Working, and Hobby databases.
        /// </summary>
        /// <returns>A string containing the user's favorite activity category.</returns>
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

            if (activitiesByCategory.Values.All(count => count == 0))
            {
                TextHelper.TextGenerator("User favorite activity: User does not have any recorded global activities!", ConsoleColor.Red);
                return null;
            }

            int maxCount = activitiesByCategory.Values.Max();
            IEnumerable<string> favoriteCategories = activitiesByCategory.Where(x => x.Value == maxCount).Select(x => x.Key);

            string favoriteActivities = string.Join(", ", favoriteCategories);
            TextHelper.TextGenerator($"User favorite activity: {favoriteActivities}", ConsoleColor.Cyan);

            return favoriteCategories.First();
        }

        /// <summary>
        /// Prompts the user to select an activity type from a list of options.
        /// </summary>
        /// <typeparam name="T">The type of the activity option.</typeparam>
        /// <param name="prompt">The message to display prompting the user to select an activity type.</param>
        /// <param name="options">The list of activity options to display to the user.</param>
        /// <param name="errorMessage">The message to display if the user enters an invalid selection.</param>
        /// <returns>The selected activity type.</returns>
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

        /// <summary>
        /// Displays the favorite types of a collection of activities, based on a selector function that extracts the type from each activity object.
        /// </summary>
        /// <typeparam name="T">The type of the activity object.</typeparam>
        /// <param name="activities">The collection of activities.</param>
        /// <param name="typeSelector">A function that extracts the type from each activity object.</param>
        /// <param name="activityName">The name of the activity.</param>
        public void DisplayFavoriteTypes<T>(IEnumerable<T> activities, Func<T, object> typeSelector, string activityName)
        {
            Dictionary<object, int> typeCounts = new Dictionary<object, int>();
            foreach (var activity in activities)
            {
                var type = typeSelector(activity);
                if (typeCounts.ContainsKey(type))
                {
                    typeCounts[type]++;
                }
                else
                {
                    typeCounts[type] = 1;
                }
            }

            int maxCount = 0;
            List<object> favoriteTypes = new List<object>();
            foreach (var key in typeCounts)
            {
                if (key.Value > maxCount)
                {
                    maxCount = key.Value;
                    favoriteTypes.Clear();
                    favoriteTypes.Add(key.Key);
                }
                else if (key.Value == maxCount)
                {
                    favoriteTypes.Add(key.Key);
                }
            }

            if (favoriteTypes.Count > 0)
            {
                string typesString = string.Join(", ", favoriteTypes);
                TextHelper.TextGenerator($"Favorite {activityName} types: {typesString}", ConsoleColor.Cyan);
            }
            else
            {
                TextHelper.TextGenerator($"User does not have any {activityName} activities!", ConsoleColor.Red);
            }
        }
    }
}