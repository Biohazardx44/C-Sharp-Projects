using MovieStore.Domain.Enums;

namespace MovieStore.Domain.Entities
{
    public class Member
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfRegistration { get; set; }
        public Role Role { get; set; }

        public Member(string firstName, string lastName, int age, string userName, string password, string phoneNumber,
           DateTime dateOfRegistration, Role role)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            UserName = userName;
            Password = password;
            PhoneNumber = phoneNumber;
            DateOfRegistration = dateOfRegistration;
            Role = role;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"{FirstName} {LastName} | Registered on: {DateOfRegistration}");
        }
    }
}