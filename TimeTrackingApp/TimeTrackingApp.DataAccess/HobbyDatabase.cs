using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess
{
    public class HobbyDatabase : Database<Hobby>, IHobbyDatabase
    {
        public HobbyDatabase()
        {
            if (Items.Count == 0)
            {
                Task.Run(async () => await SeedAsync()).Wait();
            }
        }

        private async Task SeedAsync()
        {
            InsertAsync(new Hobby(1, 400, "Games"));
            InsertAsync(new Hobby(2, 800, "Trading"));
            InsertAsync(new Hobby(2, 100, "Music"));
        }

        public Hobby GetActivityById(int id)
        {
            return Items.FirstOrDefault(item => item.Id == id);
        }

        public List<Hobby> GetActivityByUserId(int id)
        {
            return Items.Where(item => item.UserId == id).ToList();
        }
    }
}