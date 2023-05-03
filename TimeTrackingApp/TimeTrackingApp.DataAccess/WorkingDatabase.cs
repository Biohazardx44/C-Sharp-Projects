using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Domain.Enums;

namespace TimeTrackingApp.DataAccess
{
    public class WorkingDatabase : Database<WorkingActivity>, IWorkingDatabase
    {
        public WorkingDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            InsertAsync(new WorkingActivity(1, 4000, Working.Office));
            InsertAsync(new WorkingActivity(2, 2000, Working.Home));
            InsertAsync(new WorkingActivity(1, 1000, Working.Home));
            InsertAsync(new WorkingActivity(3, 5000, Working.Home));
        }

        public WorkingActivity GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public List<WorkingActivity> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }
    }
}