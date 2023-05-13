using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess
{
    public class HobbyDatabase : Database<Hobby>, IHobbyDatabase
    {
        public HobbyDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            await InsertAsync(new Hobby(1, 2000, "Collecting Rocks"));
            await InsertAsync(new Hobby(2, 4000, "Sleeping"));
            await InsertAsync(new Hobby(1, 3000, "Gaming"));
            await InsertAsync(new Hobby(3, 5000, "Cooking"));
        }

        public Hobby GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public List<Hobby> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }

        public async Task AddActivityAsync(Hobby activity)
        {
            await InsertAsync(activity);
        }

        public async Task DeleteActivityAsync(int id)
        {
            Hobby existingActivity = Items.FirstOrDefault(a => a.Id == id);
            if (existingActivity != null)
            {
                Items.Remove(existingActivity);
                await UpdateAsync(existingActivity);
            }
        }

        public async Task UpdateActivityAsync(Hobby activity)
        {
            Hobby existingActivity = Items.FirstOrDefault(a => a.Id == activity.Id);
            if (existingActivity != null)
            {
                existingActivity.UserId = activity.UserId;
                existingActivity.Duration = activity.Duration;
                existingActivity.HobbyName = activity.HobbyName;
                await UpdateAsync(existingActivity);
            }
        }
    }
}