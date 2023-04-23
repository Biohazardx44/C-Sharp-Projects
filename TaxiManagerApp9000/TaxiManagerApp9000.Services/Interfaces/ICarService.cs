using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;

namespace TaxiManagerApp9000.Services.Interfaces
{
    public interface ICarService
    {
        Car AddCar(Car car);
        List<Car> GetAllCars();
        List<Car> GetAllOperationalCars();
        List<Car> GetCarsByLicenseStatus(LicensePlateStatus status);
        void UpdateCar(Car car);
    }
}