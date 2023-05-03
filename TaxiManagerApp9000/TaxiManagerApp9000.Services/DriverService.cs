using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;
using TaxiManagerApp9000.Services.Interfaces;

namespace TaxiManagerApp9000.Services
{
    public class DriverService : BaseService<Driver>, IDriverService
    {
        public void AssignDriverToShift(Driver driver, Shift shift, Car car)
        {
            throw new NotImplementedException();
        }

        public List<Driver> CheckDriverLicenseExpiryStatus(LicensePlateStatus status)
        {
            throw new NotImplementedException();
        }

        public List<Driver> GetAllDrivers()
        {
            throw new NotImplementedException();
        }

        public void UnassignDriverFromShift(Driver driver)
        {
            throw new NotImplementedException();
        }
    }
}