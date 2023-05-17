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

        /// <summary>
        /// Gets an Working Activity by its id.
        /// </summary>
        /// <param name="id">The id of the activity.</param>
        /// <returns>The Working Activity with the specified id, or null if not found.</returns>
        public WorkingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Gets all Working Activities for a specific user.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>A List of Working Activities for the specified user.</returns>
        public List<WorkingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }

        /// <summary>
        /// Adds a new Working Activity to the database.
        /// </summary>
        /// <param name="activity">The Working Activity to add.</param>
        public async Task AddActivityAsync(WorkingActivity activity)
        {
            await InsertAsync(activity);
        }

        /// <summary>
        /// Deletes a Working Activity from the database.
        /// </summary>
        /// <param name="id">The id of the activity to delete.</param>
        public async Task DeleteActivityAsync(int id)
        {
            WorkingActivity existingActivity = Items.FirstOrDefault(a => a.Id == id);
            if (existingActivity != null)
            {
                Items.Remove(existingActivity);
                await UpdateAsync(existingActivity);
            }
        }

        /// <summary>
        /// Updates a Working Activity in the database.
        /// </summary>
        /// <param name="activity">The updated Working Activity to save.</param>
        public async Task UpdateActivityAsync(WorkingActivity activity)
        {
            WorkingActivity existingActivity = Items.FirstOrDefault(a => a.Id == activity.Id);
            if (existingActivity == null)
            {
                throw new Exception("An error occurred!");
            }
            existingActivity.UserId = activity.UserId;
            existingActivity.Duration = activity.Duration;
            existingActivity.Working = activity.Working;
            await UpdateAsync(existingActivity);
        }
    }
}