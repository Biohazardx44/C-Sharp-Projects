using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.DataAccess
{
    public class ReadingDatabase : Database<ReadingActivity>, IReadingDatabase
    {
        public ReadingDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            await InsertAsync(new ReadingActivity(1, 10000, 20, ReadingType.Fiction));
            await InsertAsync(new ReadingActivity(2, 5000, 10, ReadingType.Romance));
            await InsertAsync(new ReadingActivity(1, 3000, 3, ReadingType.Romance));
            await InsertAsync(new ReadingActivity(3, 8000, 11, ReadingType.Fantasy));
        }

        public ReadingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public List<ReadingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }

        public async Task AddActivityAsync(ReadingActivity activity)
        {
            await InsertAsync(activity);
        }

        public async Task DeleteActivityAsync(int id)
        {
            ReadingActivity existingActivity = Items.FirstOrDefault(a => a.Id == id);
            if (existingActivity != null)
            {
                Items.Remove(existingActivity);
                await UpdateAsync(existingActivity);
            }
        }

        public async Task UpdateActivityAsync(ReadingActivity activity)
        {
            ReadingActivity existingActivity = Items.FirstOrDefault(a => a.Id == activity.Id);
            if (existingActivity != null)
            {
                existingActivity.UserId = activity.UserId;
                existingActivity.PageCount = activity.PageCount;
                existingActivity.Duration = activity.Duration;
                existingActivity.ReadingType = activity.ReadingType;
                await UpdateAsync(existingActivity);
            }
        }
    }
}