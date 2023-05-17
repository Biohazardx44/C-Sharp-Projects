namespace TimeTrackingApp.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public bool IsActive = true;

        private const int _validUsernameLenght = 5;
        private const int _validPasswordLenght = 6;
        private const int _validNameLenght = 2;
        private const int _validMinAge = 18;
        private const int _validMaxAge = 120;

        public User(string firstName, string lastName, int age, string username, string password) : base()
        {
            ValidateNameInput(firstName);
            FirstName = firstName;

            ValidateNameInput(lastName);
            LastName = lastName;

            ValidateAge(age);
            Age = age;

            ValidateUsername(username);
            Username = username;

            ValidatePassword(password);
            Password = password;
        }

        /// <summary>
        /// Validates the input for a name.
        /// </summary>
        /// <param name="input">The name input to be validated.</param>
        public void ValidateNameInput(string input)
        {
            if (input.Length < _validNameLenght)
            {
                throw new Exception($"First and Last Name should not be shorter than {_validNameLenght} characters!");
            }

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    throw new Exception("First and Last Name should not contain numbers!");
                }
            }
        }

        /// <summary>
        /// Validates the age input.
        /// </summary>
        /// <param name="age">The age to be validated.</param>
        public void ValidateAge(int age)
        {
            if (age < _validMinAge || age > _validMaxAge)
            {
                throw new Exception($"Age should not be less than {_validMinAge} or over {_validMaxAge} years!");
            }
        }

        /// <summary>
        /// Validates the input for a username.
        /// </summary>
        /// <param name="username">The username input to be validated.</param>
        public void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < _validUsernameLenght)
            {
                throw new Exception($"Username should not be shorter than {_validUsernameLenght} characters!");
            }

            if (!username.All(char.IsLetterOrDigit))
            {
                throw new Exception($"Username should contain only letters and numbers!");
            }
        }

        /// <summary>
        /// Validates the input for a password.
        /// </summary>
        /// <param name="password">The password input to be validated.</param>
        public void ValidatePassword(string password)
        {
            bool hasCapital = false;
            bool hasNum = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                {
                    hasCapital = true;
                }
                else if (char.IsDigit(c))
                {
                    hasNum = true;
                }

                if (hasCapital && hasNum)
                {
                    break;
                }
            }

            if (!hasCapital)
            {
                throw new Exception("Password should contain at least one capital letter!");
            }

            if (!hasNum)
            {
                throw new Exception("Password should contain at least one number!");
            }

            if (password.Length < _validPasswordLenght)
            {
                throw new Exception($"Password should not be shorter than {_validPasswordLenght} characters!");
            }
        }
    }
}