using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.Domain.Entities
{
    public class WorkingActivity : BaseEntity
    {
        public int UserId { get; set; }
        public int Duration { get; set; }
        public Working Working { get; set; }

        public WorkingActivity(int userId, int duration, Working working) : base()
        {
            UserId = userId;
            Duration = duration;
            Working = working;
        }
    }
}