namespace TimeTrackingApp.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public BaseEntity()
        {
            Id = -1;
        }
    }
}