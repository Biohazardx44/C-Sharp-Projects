using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.Domain.Entities
{
    public class ReadingActivity : BaseEntity
    {
        public int UserId { get; set; }
        public int PageCount { get; set; }
        public int Duration { get; set; }
        public ReadingType ReadingType { get; set; }

        public ReadingActivity(int userId, int duration, int pageCount, ReadingType readingType) : base()
        {
            PageCount = pageCount;
            ReadingType = readingType;
            Duration = duration;
            UserId = userId;
        }
    }
}