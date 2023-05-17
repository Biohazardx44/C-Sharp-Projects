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

        /// <summary>
        /// Gets a Reading Activity by its id.
        /// </summary>
        /// <param name="id">The id of the activity.</param>
        /// <returns>The Reading Activity with the specified id, or null if not found.</returns>
        public ReadingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Gets all Reading Activities for a specific user.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>A List of Reading Activities for the specified user.</returns>
        public List<ReadingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }

        /// <summary>
        /// Adds a new Reading Activity to the database.
        /// </summary>
        /// <param name="activity">The Reading Activity to add.</param>
        public async Task AddActivityAsync(ReadingActivity activity)
        {
            await InsertAsync(activity);
        }

        /// <summary>
        /// Deletes a Reading Activity from the database.
        /// </summary>
        /// <param name="id">The id of the activity to delete.</param>
        public async Task DeleteActivityAsync(int id)
        {
            ReadingActivity existingActivity = Items.FirstOrDefault(a => a.Id == id);
            if (existingActivity != null)
            {
                Items.Remove(existingActivity);
                await UpdateAsync(existingActivity);
            }
        }

        /// <summary>
        /// Updates a Reading Activity in the database.
        /// </summary>
        /// <param name="activity">The updated Reading Activity to save.</param>
        public async Task UpdateActivityAsync(ReadingActivity activity)
        {
            ReadingActivity existingActivity = Items.FirstOrDefault(a => a.Id == activity.Id);
            if (existingActivity == null)
            {
                throw new Exception("An error occurred!");
            }
            existingActivity.UserId = activity.UserId;
            existingActivity.PageCount = activity.PageCount;
            existingActivity.Duration = activity.Duration;
            existingActivity.ReadingType = activity.ReadingType;
            await UpdateAsync(existingActivity);
        }
    }
}