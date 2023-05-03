namespace TimeTrackingApp.Domain.Entities
{
    public class Hobby : BaseEntity
    {
        public int UserId { get; set; }
        public int Duration { get; set; }
        public string Name { get; set; } = string.Empty;

        public Hobby(int userId, int duration, string name) : base()
        {
            UserId = userId;
            Duration = duration;
            Name = name;
        }
    }
}