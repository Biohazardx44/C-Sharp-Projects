using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;

namespace TaxiManagerApp9000.Services.Interfaces
{
    public interface ICarService
    {
        List<Car> GetAllCars();
        List<Car> GetAllOperationalCars();
        List<Car> CheckCarLicenseExpiryStatus(LicensePlateStatus status);
    }
}