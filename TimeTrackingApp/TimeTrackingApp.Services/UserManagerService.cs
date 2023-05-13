using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Services.Interfaces;

namespace TimeTrackingApp.Services
{
    public class UserManagerService : IUserManagerService
    {
        private IUserDatabase _database;

        public User CurrentUser { get; private set; }

        public UserManagerService(IUserDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public void LogIn(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username can't be empty!", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password can't be empty!", nameof(password));
            }

            User currentUser = _database.GetUserByUsernameAndPassword(username, password);

            if (currentUser == null)
            {
                throw new ArgumentException("User does not exist!", nameof(username));
            }

            CurrentUser = currentUser;
        }

        public void Register(string firstName, string lastName, int age, string username, string password)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException("Enter your First Name!", nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException("Enter your Last Name!", nameof(lastName));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("Username can't be empty!", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password can't be empty!", nameof(password));
            }

            User user = new User(firstName, lastName, age, username, password);
            _database.InsertAsync(user);
        }

        public void LogOut()
        {
            CurrentUser = null;
        }
    }
}