using TimeTrackingApp.DataAccess;
using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Helpers;
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

        /// <summary>
        /// Registers a new user with the specified first name, last name, age, username, and password.
        /// </summary>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="age">The age of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters are null or empty.</exception>
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

        /// <summary>
        /// Logs in the user with the specified username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters are null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified user does not exist.</exception>
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

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        public void LogOut()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Gets the current valid input from the user of the specified type, expected value, and input reader function.
        /// </summary>
        /// <param name="inputType">The type of the input (e.g., "username").</param>
        /// <param name="expectedValue">The expected value of the input.</param>
        /// <param name="inputReader">The function used to read the user input.</param>
        /// <returns>The current valid input from the user.</returns>
        /// <exception cref="ArgumentException">Thrown when the user enters an incorrect input value more than 3 times.</exception>
        public async Task<string> GetCurrentValidInputFromUser(string inputType, string expectedValue, Func<string> inputReader)
        {
            int incorrectAttempts = 0;

            while (incorrectAttempts < 3)
            {
                TextHelper.TextGenerator($"\nEnter your current {inputType}:", ConsoleColor.Cyan);
                string currentValue = inputReader();

                if (currentValue != expectedValue)
                {
                    TextHelper.TextGenerator($"The current {inputType} you entered is incorrect.\n", ConsoleColor.Red);
                    incorrectAttempts++;
                }
                else
                {
                    return currentValue;
                }
            }

            TextHelper.TextGenerator($"\nYou have tried to enter your current {inputType} 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
            Environment.Exit(0);
            return null;
        }

        /// <summary>
        /// Gets the new valid input from the user of the specified type.
        /// </summary>
        /// <param name="inputType">The type of the input (e.g., "username").</param>
        /// <returns>The new valid input from the user.</returns>
        /// <exception cref="ArgumentException">Thrown when the user enters an incorrect input value more than 3 times.</exception>
        public async Task<string> GetNewValidInputFromUser(string inputType)
        {
            int incorrectAttempts = 0;
            string currentValue;

            while (incorrectAttempts < 3)
            {
                TextHelper.TextGenerator($"Enter the new {inputType}:", ConsoleColor.Cyan);
                currentValue = Console.ReadLine();

                try
                {
                    switch (inputType)
                    {
                        case "first name":
                            CurrentUser.ValidateNameInput(currentValue);
                            break;
                        case "last name":
                            CurrentUser.ValidateNameInput(currentValue);
                            break;
                        case "age":
                            if (int.TryParse(currentValue, out int age))
                            {
                                CurrentUser.ValidateAge(age);
                            }
                            else
                            {
                                throw new ArgumentException("Invalid age format!");
                            }
                            break;
                        case "username":
                            if (!CurrentUser.Username.Equals(currentValue, StringComparison.OrdinalIgnoreCase) && _database.CheckUsernameAvailable(currentValue))
                            {
                                throw new ArgumentException("Username already exists. Please choose a different username.");
                            }
                            CurrentUser.Username = currentValue;
                            CurrentUser.ValidateUsername(currentValue);
                            break;
                        case "password":
                            CurrentUser.ValidatePassword(currentValue);
                            break;
                        default:
                            throw new ArgumentException($"Invalid input type: {inputType}");
                    }

                    return currentValue;
                }
                catch (Exception ex)
                {
                    TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
                    incorrectAttempts++;
                }

                if (incorrectAttempts == 3)
                {
                    TextHelper.TextGenerator($"\nYou have tried to enter a valid {inputType} 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
                    Environment.Exit(0);
                }
            }

            return null;
        }
    }
}