using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Helpers;
using TimeTrackingApp.Services.Interfaces;

namespace TimeTrackingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IReadingDatabase _readingDatabase;
        private readonly IExercisingDatabase _exercisingDatabase;
        private readonly IWorkingDatabase _workingDatabase;
        private readonly IHobbyDatabase _hobbyDatabase;
        private readonly IUserManagerService _userManagerService;

        public UserService(IReadingDatabase readingDatabase, IExercisingDatabase exercisingDatabase, IWorkingDatabase workingDatabase, IHobbyDatabase hobbyDatabase, IUserManagerService userManagerService)
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

            Dictionary<string, int> activitiesByCategory = new Dictionary<string, int>();
            activitiesByCategory.Add("Reading", readingActivities.Count);
            activitiesByCategory.Add("Exercising", exercisingActivities.Count);
            activitiesByCategory.Add("Working", workingActivities.Count);
            activitiesByCategory.Add("Hobby", hobbyActivities.Count);

            int maxCount = activitiesByCategory.Values.Max();
            IEnumerable<string> favoriteCategories = activitiesByCategory.Where(x => x.Value == maxCount).Select(x => x.Key);

            string favoriteActivities = string.Join(", ", favoriteCategories);
            TextHelper.TextGenerator($"User favorite activity: {favoriteActivities}", ConsoleColor.Cyan);

            return favoriteCategories.First();
        }
    }
}