namespace TimeTrackingApp.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        private const int VALID_USERNAME_LENGHT = 5;
        private const int VALID_PASSWORD_LENGHT = 6;
        private const int VALID_NAME_LENGHT = 2;
        private const int VALID_MIN_AGE = 18;
        private const int VALID_MAX_AGE = 120;

        public User(string firstName, string lastName, int age, string username, string password) : base()
        {
            ValidateNameInput(firstName);
            FirstName = firstName;

            ValidateNameInput(lastName);
            LastName = lastName;

            if (age < VALID_MIN_AGE || age > VALID_MAX_AGE)
            {
                throw new Exception("Not in valid age group!");
            }
            Age = age;

            if (username.Length < VALID_USERNAME_LENGHT)
            {
                throw new Exception($"Username should be more then {VALID_USERNAME_LENGHT}");
            }

            if (username.Length < VALID_USERNAME_LENGHT)
            {
                throw new Exception($"Password should be more then {VALID_USERNAME_LENGHT}");
            }
            ValidatePassword(password);
            Password = password;

            Username = username;
        }

        private void ValidatePassword(string password)
        {
            bool hasCapital = false;
            bool hasNum = false;

            if (!hasCapital)
            {
                //throw new Exception("Password should have atleast one capital leater!");
            }

            if (!hasNum)
            {
                //throw new Exception("Password should have atleast one number!");
            }
        }

        private void ValidateNameInput(string input)
        {
            if (input.Length < VALID_NAME_LENGHT)
            {
                throw new Exception("First and Last Name should not be shorter then two chars!");
            }

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    //throw new Exception("First and Last Name should not contain numbers!");
                }
            }
        }
    }
}