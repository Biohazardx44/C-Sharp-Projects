using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.Domain.Entities
{
    public class WorkingActivity : BaseEntity
    {
        public int UserId { get; set; }
        public int Duration { get; set; }
        public Working Type { get; set; }

        public WorkingActivity(int userId, int duration, Working type) : base()
        {
            UserId = userId;
            Duration = duration;
            Type = type;
        }
    }
}