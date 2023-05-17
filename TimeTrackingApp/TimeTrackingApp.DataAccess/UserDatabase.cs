using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess
{
    public class UserDatabase : Database<User>, IUserDatabase
    {
        public UserDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            await InsertAsync(new User("Test", "Test", 18, "Test1", "A12345"));
            await InsertAsync(new User("admin", "admin", 28, "admin", "A12345"));
            await InsertAsync(new User("newuser", "newuser", 20, "newuser", "A12345"));
        }

        /// <summary>
        /// Gets a user by their username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The user with the matching username and password, or null if not found.</returns>
        public User GetUserByUsernameAndPassword(string username, string password)
        {
            return Items.FirstOrDefault(user => user.Username == username && user.Password == password);
        }

        /// <summary>
        /// Checks if a username is available.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>True if the username is available, false otherwise.</returns>
        public bool CheckUsernameAvailable(string username)
        {
            return Items.Any(user => user.Username == username);
        }

        /// <summary>
        /// Updates a user's password.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateUserAsync(User user)
        {
            User existingUser = await GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new Exception ("An error occurred!");
            }
            existingUser.Password = user.Password;
            await UpdateAsync(existingUser);
        }
    }
}