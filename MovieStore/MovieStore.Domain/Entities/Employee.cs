using MovieStore.Domain.Enums;

namespace MovieStore.Domain.Entities
{
    public class Employee : Member
    {
        private double _salary = 300;
        public double Salary
        {
            get { return _salary; }
            private set { _salary = value; }
        }

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
            Bonus = HoursPerMonth > 160 ? 0.3 : null;
        }

        public void SetSalary()
        {
            if (Bonus == null)
            {
                Salary = 300;
            }
            else
            {
                Salary = HoursPerMonth * Bonus.Value;
            }
        }
    }
}