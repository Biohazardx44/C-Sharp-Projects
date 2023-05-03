namespace TimeTrackingApp.Domain.Entities
{
    public abstract class BaseActivity
    {
        public int Id { get; set; }
        public TimeSpan Duration { get; set; }

        protected BaseActivity(int id, TimeSpan duration)
        {
            Id = id;
            Duration = duration;
        }
    }
}