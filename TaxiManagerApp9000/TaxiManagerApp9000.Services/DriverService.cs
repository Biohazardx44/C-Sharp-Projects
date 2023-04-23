using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;
using TaxiManagerApp9000.Services.Interfaces;

namespace TaxiManagerApp9000.Services
{
    public class DriverService : BaseService<Driver>, IDriverService
    {
        private List<Driver> _drivers = new List<Driver>();

        public Driver AddDriver(Driver driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if (string.IsNullOrEmpty(driver.FirstName) || string.IsNullOrEmpty(driver.LastName))
            {
                throw new ArgumentException("Driver name cannot be empty or null.");
            }

            if (string.IsNullOrEmpty(driver.License) || driver.LicenseExpiryDate == default)
            {
                throw new ArgumentException("License information is required.");
            }

            if (driver.LicenseExpiryDate < DateTime.Now)
            {
                throw new ArgumentException("License has already expired.");
            }

            _drivers.Add(driver);

            return driver;
        }

        public List<Driver> GetAllDrivers()
        {
            return _drivers;
        }

        public Driver GetDriverById(int id)
        {
            return _drivers.FirstOrDefault(d => d.Id == id);
        }

        public List<Driver> GetUnassignedDrivers()
        {
            return _drivers.Where(d => d.Car == null).ToList();
        }

        public bool CheckLicenseExpiryStatus(Driver driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            return driver.LicenseExpiryDate < DateTime.Now;
        }

        public void AssignDriverToShift(Driver driver, Shift shift, Car car)
        {
            throw new NotImplementedException();
        }

        public void UnassignDriverFromShift(Driver driver)
        {
            throw new NotImplementedException();
        }
    }
}