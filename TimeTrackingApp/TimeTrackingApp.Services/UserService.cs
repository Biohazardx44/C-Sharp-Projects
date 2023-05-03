using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Services.Interfaces;

namespace TimeTrackingApp.Services
{
    public class UserService : IUserService<ReadingActivity>
    {
        public void AddActivity(ReadingActivity activity)
        {
            throw new NotImplementedException();
        }
    }
}