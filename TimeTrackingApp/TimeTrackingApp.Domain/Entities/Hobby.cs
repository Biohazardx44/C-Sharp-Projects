namespace TimeTrackingApp.Domain.Entities
{
    public class Hobby : BaseEntity
    {
        public int UserId { get; set; }
        public int Duration { get; set; }
        public string HobbyName { get; set; } = string.Empty;

        public Hobby(int userId, int duration, string hobbyName) : base()
        {
            UserId = userId;
            Duration = duration;
            HobbyName = hobbyName;
        }
    }
}