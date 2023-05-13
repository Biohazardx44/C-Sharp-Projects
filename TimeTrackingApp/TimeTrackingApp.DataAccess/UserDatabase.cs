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

        public User GetUserByUsernameAndPassword(string username, string password)
        {
            return Items.FirstOrDefault(user => user.Username == username && user.Password == password);
        }

        public bool CheckUsernameAvailable(string username)
        {
            return Items.Any(user => user.Username == username);
        }

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