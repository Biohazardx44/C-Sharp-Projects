using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.DataAccess
{
    public class ReadingDatabase : Database<ReadingActivity>, IReadingDatabase
    {
        public ReadingDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            InsertAsync(new ReadingActivity(1, 1000, 10, ReadingType.Fiction));
            InsertAsync(new ReadingActivity(2, 2000, 20, ReadingType.Romance));
            InsertAsync(new ReadingActivity(3, 3000, 30, ReadingType.Fantasy));
            InsertAsync(new ReadingActivity(4, 4000, 40, ReadingType.Fantasy));
            InsertAsync(new ReadingActivity(5, 5000, 50, ReadingType.Fiction));
            InsertAsync(new ReadingActivity(1, 6000, 60, ReadingType.Romance));
        }

        public ReadingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public List<ReadingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }
    }
}