using MovieStore.Domain.Enums;

namespace MovieStore.Domain.Entities
{
    public class User : Member
    {
        public int MemberNumber { get; set; }
        public SubscriptionType TypeOfSubscription { get; set; }
        public Movie[] Movies { get; set; }

        public User(string firstName, string lastName, int age, string userName, string password, string phoneNumber,
            DateTime dateOfRegistration, Role role, int memberNumber, SubscriptionType subscriptionType) : base(firstName, lastName, age, userName, password, phoneNumber, dateOfRegistration, role)
        {
            MemberNumber = memberNumber;
            TypeOfSubscription = subscriptionType;
        }
    }
}