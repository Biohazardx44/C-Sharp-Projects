using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;
using TaxiManagerApp9000.Services.Interfaces;

namespace TaxiManagerApp9000.Services
{
    public class CarService : BaseService<Car>, ICarService
    {
        public List<Car> CheckCarLicenseExpiryStatus(LicensePlateStatus status)
        {
            throw new NotImplementedException();
        }

        public List<Car> GetAllCars()
        {
            throw new NotImplementedException();
        }

        public List<Car> GetAllOperationalCars()
        {
            throw new NotImplementedException();
        }
    }
}