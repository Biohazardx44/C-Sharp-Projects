using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.DataAccess
{
    public class ExercisingDatabase : Database<ExercisingActivity>, IExercisingDatabase
    {
        public ExercisingDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            await InsertAsync(new ExercisingActivity(1, 6000, ExercisingType.Swimming));
            await InsertAsync(new ExercisingActivity(2, 3000, ExercisingType.Running));
            await InsertAsync(new ExercisingActivity(1, 2000, ExercisingType.Running));
            await InsertAsync(new ExercisingActivity(3, 4000, ExercisingType.Yoga));
        }

        public ExercisingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public List<ExercisingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }

        public async Task AddActivityAsync(ExercisingActivity activity)
        {
            await InsertAsync(activity);
        }

        public async Task DeleteActivityAsync(int id)
        {
            ExercisingActivity existingActivity = Items.FirstOrDefault(a => a.Id == id);
            if (existingActivity != null)
            {
                Items.Remove(existingActivity);
                await WriteToFileAsync();
            }
        }

        public async Task UpdateActivityAsync(ExercisingActivity activity)
        {
            ExercisingActivity existingActivity = Items.FirstOrDefault(a => a.Id == activity.Id);
            if (existingActivity != null)
            {
                existingActivity.UserId = activity.UserId;
                existingActivity.Duration = activity.Duration;
                existingActivity.ExercisingType = activity.ExercisingType;
                await WriteToFileAsync();
            }
        }
    }
}