using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;

namespace TaxiManagerApp9000.Services.Interfaces
{
    public interface IDriverService
    {
        Driver AddDriver(Driver driver);
        List<Driver> GetAllDrivers();
        List<Driver> GetUnassignedDrivers();
        void AssignDriverToShift(Driver driver, Shift shift, Car car);
        void UnassignDriverFromShift(Driver driver);
        Driver GetDriverById(int id);
        bool CheckLicenseExpiryStatus(Driver driver);
    }
}