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
            await InsertAsync(new User("Test", "Test", 18, "test", "Test"));
            await InsertAsync(new User("NewTest", "Test", 20, "test", "Test"));
            await InsertAsync(new User("AnotherTest", "Test", 22, "test", "Test"));
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
            User existingUser = Items.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Password = user.Password;
                await WriteToFileAsync();
            }
        }
    }
}