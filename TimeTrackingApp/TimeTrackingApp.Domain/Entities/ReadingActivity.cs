using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.Domain.Entities
{
    public class ReadingActivity : BaseEntity
    {
        public int UserId { get; set; }
        public int PageCount { get; set; }
        public ReadingType Type { get; set; }
        public int Duration { get; set; }

        public ReadingActivity(int userId, int duration, int pageCount, ReadingType type) : base()
        {
            PageCount = pageCount;
            Type = type;
            Duration = duration;
            UserId = userId;
        }
    }
}