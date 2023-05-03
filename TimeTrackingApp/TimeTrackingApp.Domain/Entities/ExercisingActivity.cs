using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.Domain.Entities
{
    public class ExercisingActivity : BaseEntity
    {
        public int UserId { get; set; }
        public int Duration { get; set; }
        public ExercisingType ExercisingType { get; set; }

        public ExercisingActivity(int userId, int duration, ExercisingType exercisingType) : base()
        {
            UserId = userId;
            Duration = duration;
            ExercisingType = exercisingType;
        }
    }
}