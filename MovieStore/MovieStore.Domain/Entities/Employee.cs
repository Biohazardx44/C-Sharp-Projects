using MovieStore.Domain.Enums;

namespace MovieStore.Domain.Entities
{
    public class Employee : Member
    {
        private double _salary = 300;
        public int HoursPerMonth { get; set; }
        public double? Bonus { get; private set; }

        public Employee(string firstName, string lastName, int age, string userName, string password, string phoneNumber,
            DateTime dateOfRegistration, Role role, int hoursPerMonth) : base(firstName, lastName, age, userName, password,
            phoneNumber, dateOfRegistration, role)
        {
            HoursPerMonth = hoursPerMonth;
        }

        public void SetBonus()
        {
            if (HoursPerMonth > 160)
            {
                Bonus = 0.3;
            }
            else
            {
                Bonus = null;
            }
        }

        public void SetSalary()
        {
            if (Bonus != null)
            {
                _salary = HoursPerMonth * Bonus.Value;
            }
        }
    }
}