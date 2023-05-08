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

            if (age < _validMinAge || age > _validMaxAge)
            {
                throw new Exception($"Age should not be less than {_validMinAge} or over {_validMaxAge} years!");
            }
            Age = age;

            if (username.Length < _validUsernameLenght)
            {
                throw new Exception($"Username should not be shorter than {_validUsernameLenght} characters!");
            }

            ValidatePassword(password);

            Password = password;
            Username = username;
        }

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
    }
}