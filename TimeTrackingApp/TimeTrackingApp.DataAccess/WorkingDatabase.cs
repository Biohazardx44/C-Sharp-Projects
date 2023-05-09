using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.DataAccess
{
    public class WorkingDatabase : Database<WorkingActivity>, IWorkingDatabase
    {
        public WorkingDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            await InsertAsync(new WorkingActivity(1, 4000, Working.Office));
            await InsertAsync(new WorkingActivity(2, 2000, Working.Home));
            await InsertAsync(new WorkingActivity(1, 1000, Working.Home));
            await InsertAsync(new WorkingActivity(3, 5000, Working.Home));
        }

        public WorkingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public List<WorkingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }

        public async Task AddActivityAsync(WorkingActivity activity)
        {
            await InsertAsync(activity);
        }

        public async Task DeleteActivityAsync(int id)
        {
            WorkingActivity existingActivity = Items.FirstOrDefault(a => a.Id == id);
            if (existingActivity != null)
            {
                Items.Remove(existingActivity);
                await WriteToFileAsync();
            }
        }

        public async Task UpdateActivityAsync(WorkingActivity activity)
        {
            WorkingActivity existingActivity = Items.FirstOrDefault(a => a.Id == activity.Id);
            if (existingActivity != null)
            {
                existingActivity.UserId = activity.UserId;
                existingActivity.Duration = activity.Duration;
                existingActivity.Working = activity.Working;
                await WriteToFileAsync();
            }
        }
    }
}