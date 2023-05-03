using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.DataAccess
{
    public class ExercisingDatabase : Database<ExercisingActivity>, IExercisingDatabase
    {
        public ExercisingDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            InsertAsync(new ExercisingActivity(1, 200, ExercisingType.Running));
            InsertAsync(new ExercisingActivity(1, 4500, ExercisingType.Swimming));
            InsertAsync(new ExercisingActivity(2, 2500, ExercisingType.Yoga));
            InsertAsync(new ExercisingActivity(5, 2500, ExercisingType.Swimming));
            InsertAsync(new ExercisingActivity(1, 2030, ExercisingType.Running));
        }

        public ExercisingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public List<ExercisingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }
    }
}