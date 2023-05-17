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

        /// <summary>
        /// Gets a Hobby Activity by its id.
        /// </summary>
        /// <param name="id">The id of the activity.</param>
        /// <returns>The Hobby Activity with the specified id, or null if not found.</returns>
        public Hobby GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Gets all Hobby Activities for a specific user.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>A List of Hobby Activities for the specified user.</returns>
        public List<Hobby> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }

        /// <summary>
        /// Adds a new Hobby Activity to the database.
        /// </summary>
        /// <param name="activity">The Hobby Activity to add.</param>
        public async Task AddActivityAsync(Hobby activity)
        {
            await InsertAsync(activity);
        }

        /// <summary>
        /// Deletes a Hobby Activity from the database.
        /// </summary>
        /// <param name="id">The id of the activity to delete.</param>
        public async Task DeleteActivityAsync(int id)
        {
            Hobby existingActivity = Items.FirstOrDefault(a => a.Id == id);
            if (existingActivity != null)
            {
                Items.Remove(existingActivity);
                await UpdateAsync(existingActivity);
            }
        }

        /// <summary>
        /// Updates a Hobby Activity in the database.
        /// </summary>
        /// <param name="activity">The updated Hobby Activity to save.</param>
        public async Task UpdateActivityAsync(Hobby activity)
        {
            Hobby existingActivity = Items.FirstOrDefault(a => a.Id == activity.Id);
            if (existingActivity == null)
            {
                throw new Exception("An error occurred!");
            }
            existingActivity.UserId = activity.UserId;
            existingActivity.Duration = activity.Duration;
            existingActivity.HobbyName = activity.HobbyName;
            await UpdateAsync(existingActivity);
        }
    }
}