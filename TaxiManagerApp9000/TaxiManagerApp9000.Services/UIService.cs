using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;
using TaxiManagerApp9000.Helpers;
using TaxiManagerApp9000.Services.Interfaces;

namespace TaxiManagerApp9000.Services
{
    public class UIService : IUIService
    {
        private List<MenuOptions> _menuChoice;

        public List<MenuOptions> MenuChoice
        {
            get => _menuChoice;
            set
            {
                if (_menuChoice != null)
                {
                    _menuChoice.Clear();
                }
                _menuChoice = value;
            }
        }

        public User LogIn()
        {
            TextHelper.TextGenerator("Welcome to Taxi Manager 9000", ConsoleColor.Green);
            TextHelper.TextGenerator("Log In:\n", ConsoleColor.Green);
            Console.ForegroundColor = ConsoleColor.Cyan;
            string username = TextHelper.GetInput("Username:");
            string password = TextHelper.GetInput("Password:");
            Console.ResetColor();
            return new User(username, password);
        }

        public int MainMenu(Role role)
        {
            List<MenuOptions> menuItems = new List<MenuOptions>() { MenuOptions.ChangePassword, MenuOptions.Logout, MenuOptions.Exit };

            switch (role)
            {
                case Role.Administrator:
                    menuItems = menuItems.Prepend(MenuOptions.RemoveExistingUser).ToList();
                    menuItems = menuItems.Prepend(MenuOptions.AddNewUser).ToList();
                    break;
                case Role.Manager:
                    menuItems = menuItems.Prepend(MenuOptions.DriverManager).ToList();
                    menuItems = menuItems.Prepend(MenuOptions.TaxiLicenseStatus).ToList();
                    menuItems = menuItems.Prepend(MenuOptions.ListAllDrivers).ToList();
                    break;
                case Role.Maintenance:
                    menuItems = menuItems.Prepend(MenuOptions.ListAllCars).ToList();
                    menuItems = menuItems.Prepend(MenuOptions.LicensePlateStatus).ToList();
                    break;
            }

            MenuChoice = menuItems;
            return ChooseMenu(menuItems);
        }

        public int ChooseMenu<T>(List<T> items)
        {
            int selectedId = -1;
            try
            {
                Console.Clear();
                Console.WriteLine("Please insert the ID of the element you want to choose:");
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {items[i]}");
                }
                selectedId = StringHelper.ValidateNumber(Console.ReadLine(), items.Count);

            }
            catch (Exception ex)
            {
                TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            }

            return selectedId;
        }

        public int ChooseEntityMenu<T>(List<T>? entities) where T : BaseEntity
        {
            int selecteEntityId = -1;
            try
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {entities[i].Print()}");
                }

                int selected = StringHelper.ValidateNumber(Console.ReadLine(), entities.Count);
                if (selected == -1)
                {
                    return selecteEntityId;
                }
                selecteEntityId = selected;
            }
            catch (Exception ex)
            {
                TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            }

            return selecteEntityId;
        }
    }
}