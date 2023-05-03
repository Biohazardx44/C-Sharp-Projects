using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;

namespace TaxiManagerApp9000.Services.Interfaces
{
    public interface IDriverService
    {
        List<Driver> GetAllDrivers();
        void AssignDriverToShift(Driver driver, Shift shift, Car car);
        void UnassignDriverFromShift(Driver driver);
        List<Driver> CheckDriverLicenseExpiryStatus(LicensePlateStatus status);
    }
}