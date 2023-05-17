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

        /// <summary>
        /// Gets an Exercising Activity by its id.
        /// </summary>
        /// <param name="id">The id of the activity.</param>
        /// <returns>The Exercising Activity with the specified id, or null if not found.</returns>
        public ExercisingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Gets all Exercising Activities for a specific user.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>A List of Exercising Activities for the specified user.</returns>
        public List<ExercisingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }

        /// <summary>
        /// Adds a new Exercising Activity to the database.
        /// </summary>
        /// <param name="activity">The Exercising Activity to add.</param>
        public async Task AddActivityAsync(ExercisingActivity activity)
        {
            await InsertAsync(activity);
        }

        /// <summary>
        /// Deletes an Exercising Activity from the database.
        /// </summary>
        /// <param name="id">The id of the activity to delete.</param>
        public async Task DeleteActivityAsync(int id)
        {
            ExercisingActivity existingActivity = Items.FirstOrDefault(a => a.Id == id);
            if (existingActivity != null)
            {
                Items.Remove(existingActivity);
                await UpdateAsync(existingActivity);
            }
        }

        /// <summary>
        /// Updates an Exercising Activity in the database.
        /// </summary>
        /// <param name="activity">The updated Exercising Activity to save.</param>
        public async Task UpdateActivityAsync(ExercisingActivity activity)
        {
            ExercisingActivity existingActivity = Items.FirstOrDefault(a => a.Id == activity.Id);
            if (existingActivity == null)
            {
                throw new Exception("An error occurred!");
            }
            existingActivity.UserId = activity.UserId;
            existingActivity.Duration = activity.Duration;
            existingActivity.ExercisingType = activity.ExercisingType;
            await UpdateAsync(existingActivity);
        }
    }
}