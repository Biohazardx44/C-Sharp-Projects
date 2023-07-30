#region Setup
using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;
using TaxiManagerApp9000.Helpers;
using TaxiManagerApp9000.Services;
using TaxiManagerApp9000.Services.Interfaces;

ICarService carService = new CarService();
IDriverService driverService = new DriverService();
IUserService userService = new UserService();
IUIService uiService = new UIService();
ILogger logger = new Logger();

InitializeStartingData();
#endregion

while (true)
{
    Console.Clear();
    if (userService.CurrentUser == null)
    {
        try
        {
            User loginUser = uiService.LogIn();
            userService.Login(loginUser.Username, loginUser.Password);

            TextHelper.TextGenerator($"Login successful! Welcome [{userService.CurrentUser.Role}] user!", ConsoleColor.Green);
            TextHelper.TextGenerator($"\nPress ENTER to go to the main menu!", ConsoleColor.Green);
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            logger.Log("Error", ex.Message, ex.StackTrace, "not logged in");
            TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            TextHelper.TextGenerator($"\nPress ENTER to try again!", ConsoleColor.Red);
            Console.ReadLine();
            continue;
        }
    }

    bool loopActive = true;
    while (loopActive)
    {
        Console.Clear();
        int selectedItem = uiService.MainMenu(userService.CurrentUser.Role);
        if (selectedItem == -1)
        {
            TextHelper.TextGenerator("Invalid input selected", ConsoleColor.Red);
            Console.ReadLine();
            continue;
        }
        MenuOptions choise = uiService.MenuChoice[selectedItem - 1];
        switch (choise)
        {
            case MenuOptions.AddNewUser:
                {
                    try
                    {
                        string username = TextHelper.GetInput("Username:");
                        string password = TextHelper.GetInput("Password:");
                        List<string> roles = new List<string>() { "Administrator", "Manager", "Maintenance" };
                        int enumInt = uiService.ChooseMenu(roles);

                        if (enumInt < 0 || enumInt > 2)
                        {
                            TextHelper.TextGenerator("Invalid role selection!", ConsoleColor.Red);
                            break;
                        }

                        Role role = (Role)enumInt;
                        User user = new User(username, password, role);
                        userService.Add(user);

                        TextHelper.TextGenerator("New User Added", ConsoleColor.Green);
                    }
                    catch (Exception ex)
                    {
                        TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
                    }
                    break;
                }
            case MenuOptions.RemoveExistingUser:
                {
                    try
                    {
                        Console.WriteLine("Select User For Removal (insert number in front of username):");
                        var usersForRemoval = userService.GetUsersForRemoval();
                        int selectedUser = uiService.ChooseEntityMenu(usersForRemoval);
                        if (selectedUser == -1)
                        {
                            TextHelper.TextGenerator("Wrong option Selected", ConsoleColor.Red);
                            continue;
                        }

                        if (userService.Remove(usersForRemoval[selectedUser - 1].Id))
                        {
                            TextHelper.TextGenerator("User removed", ConsoleColor.Yellow);
                        }
                    }
                    catch (Exception ex)
                    {
                        TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
                    }
                    break;
                }
            case MenuOptions.ListAllDrivers:
                {
                    var allData = driverService.GetAll();
                    AdvertisementHelper.DiscountAdd();
                    await allData;
                    allData.Result.ForEach(x => Console.WriteLine(x.Print()));
                    Console.ReadLine();
                    break;
                }
            case MenuOptions.TaxiLicenseStatus:
                {
                    carService.GetAll().Result.ForEach(x =>
                    {
                        var status = x.IsLicenseExpired();

                        switch (status)
                        {
                            case ExpiryStatus.Expired:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case ExpiryStatus.Valid:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case ExpiryStatus.Warning:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                        }

                        Console.WriteLine(x.Print());
                        Console.ResetColor();
                    });
                    Console.ReadLine();
                    break;
                }
            case MenuOptions.DriverManager:
                {
                    var driverManagerMenu = new List<DriverManagerChoice>() { DriverManagerChoice.AssignDriver, DriverManagerChoice.UnassignDriver };
                    int driverManagerChoice = uiService.ChooseMenu(driverManagerMenu);
                    var availableDrivers = driverService.GetAll(x => driverService.IsAvailableDriver(x));

                    if (driverManagerChoice == 1)
                    {
                        var availableForAssigningDrivers = availableDrivers
                            .Where(x => x.CarId == null)
                            .ToList();
                        int assigningDrvierChoice = uiService
                            .ChooseEntityMenu<Driver>(availableForAssigningDrivers);
                        if (assigningDrvierChoice == -1) continue;

                        var availableCarsForAssigning = carService
                            .GetAll(x => carService.IsAvailableCar(x))
                            .ToList();
                        var assigningCarChoice = uiService
                            .ChooseEntityMenu<Car>(availableCarsForAssigning);
                        if (assigningCarChoice == -1) continue;

                        driverService.AssignDriver(
                            availableForAssigningDrivers[assigningDrvierChoice - 1],
                            availableCarsForAssigning[assigningCarChoice - 1]
                            );
                        carService.AssignDriver(availableForAssigningDrivers[assigningDrvierChoice - 1],
                            availableCarsForAssigning[assigningCarChoice - 1]
                            );
                    }
                    else if (driverManagerChoice == 2)
                    {
                        var availableForUnassigningDrivers = availableDrivers
                            .Where(x => x.CarId != null)
                            .ToList();
                        var unassigningDrvierChoice = uiService
                            .ChooseEntityMenu<Driver>(availableForUnassigningDrivers);
                        if (unassigningDrvierChoice == -1) continue;

                        driverService.Unassign(availableForUnassigningDrivers[unassigningDrvierChoice - 1]);
                    }
                    break;
                }
            case MenuOptions.ListAllCars:
                {
                    var taskData = carService.GetAll();
                    AdvertisementHelper.DiscountAdd();
                    await taskData;
                    taskData.Result.ForEach(x => Console.WriteLine(x.Print()));
                    Console.ReadLine();
                    break;
                }
            case MenuOptions.LicensePlateStatus:
                {
                    foreach (var car in carService.GetAll().Result)
                    {
                        ExpiryStatus status = car.IsLicenseExpired();
                        switch (status)
                        {
                            case ExpiryStatus.Expired:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case ExpiryStatus.Valid:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case ExpiryStatus.Warning:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                        }
                        Console.WriteLine($"Car Id: {car.Id} - Plate: {car.LicensePlate} with expiery date: {car.LicensePlateExpiryDate}");
                        Console.ResetColor();
                    }
                    Console.ReadLine();
                    break;
                }
            case MenuOptions.ChangePassword:
                {
                    string oldPass = TextHelper.GetInput("Insert old password: ");
                    string newPass = TextHelper.GetInput("Insert new password: ");
                    if (userService.ChangePassword(userService.CurrentUser.Id, oldPass, newPass))
                    {
                        TextHelper.TextGenerator("Password changed!", ConsoleColor.Green);
                    }
                    break;
                }
            case MenuOptions.Logout:
                {
                    userService.CurrentUser = null;
                    loopActive = false;
                    break;
                }
            case MenuOptions.Exit:
                {
                    Environment.Exit(0);
                    break;
                }
        }
    }
}

#region Seeding
void InitializeStartingData()
{
    User administrator = new User("administrator", "administrator", Role.Administrator);
    User manager = new User("manager", "manager", Role.Manager);
    User maintenances = new User("maintenance", "maintenance", Role.Maintenance);
    List<User> seedUsers = new List<User>() { administrator, manager, maintenances };
    userService.Seed(seedUsers);

    Car car1 = new Car("Toyota Supra", "AFW950", new DateTime(2023, 12, 1));
    Car car2 = new Car("Honda Civic", "CKE480", new DateTime(2021, 10, 15));
    Car car3 = new Car("Ford Mustang", "GZDR69", new DateTime(2024, 5, 30));
    List<Car> seedCars = new List<Car>() { car1, car2, car3 };
    carService.Seed(seedCars);

    Driver driver1 = new Driver("John", "Smith", Shift.NoShift, null, "LC12456123", new DateTime(2023, 11, 5));
    Driver driver2 = new Driver("Sarah", "Johnson", Shift.Morning, car1.Id, "LC54435234", new DateTime(2022, 1, 12));
    Driver driver3 = new Driver("Michael", "Brown", Shift.Evening, car2.Id, "LC65803245", new DateTime(2022, 5, 19));
    Driver driver4 = new Driver("Emily", "Davis", Shift.Afternoon, car3.Id, "LC20897583", new DateTime(2023, 9, 28));
    List<Driver> seedDrivers = new List<Driver>() { driver1, driver2, driver3, driver4 };
    driverService.Seed(seedDrivers);
}
#endregion