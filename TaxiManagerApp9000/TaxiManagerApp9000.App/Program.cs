using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;
using TaxiManagerApp9000.Helpers;
using TaxiManagerApp9000.Services;

UserService userService = new();

List<User> users = new List<User>
{
    new User("admin", "admin", Role.Administrator),
    new User("manager", "manager", Role.Manager),
    new User("maintenance", "maintenance", Role.Maintenance)
};

void InitializeData()
{
    userService.Add(users[0]);
    userService.Add(users[1]);
    userService.Add(users[2]);
}

InitializeData();

while (true)
{
    User loggedInUser = LogInUser();
    switch (loggedInUser.Role)
    {
        case Role.Administrator:
            TextHelper.TextGenerator($"Successful Login! Welcome [{Role.Administrator}] user!", ConsoleColor.Green);
            Console.ReadKey();
            AdminMenu();
            break;
        case Role.Maintenance:
            TextHelper.TextGenerator($"Successful Login! Welcome [{Role.Maintenance}] user!", ConsoleColor.Green);
            Console.ReadKey();
            MaintananceMainMenu();
            break;
        case Role.Manager:
            TextHelper.TextGenerator($"Successful Login! Welcome [{Role.Manager}] user!", ConsoleColor.Green);
            Console.ReadKey();
            ManagerMainMenu();
            break;
    }
}

User LogInUser()
{
    while (true)
    {
        Console.Clear();
        TextHelper.TextGenerator("Taxi Manager 9000", ConsoleColor.Cyan);
        TextHelper.TextGenerator("Log in:", ConsoleColor.Cyan);
        TextHelper.TextGenerator("\nUsername: ", ConsoleColor.Cyan);
        string usernameInput = Console.ReadLine();
        if (string.IsNullOrEmpty(usernameInput))
        {
            TextHelper.TextGenerator("Invalid input. Try Again!", ConsoleColor.Red);
            Console.ReadKey();
            continue;
        }

        TextHelper.TextGenerator("Password: ", ConsoleColor.Cyan);
        string passwordInput = Console.ReadLine();

        if (string.IsNullOrEmpty(passwordInput))
        {
            TextHelper.TextGenerator("Invalid input. Try Again!", ConsoleColor.Red);
            Console.ReadKey();
            continue;
        }

        User loggedInUser = userService.Login(usernameInput, passwordInput);

        if (loggedInUser == null)
        {
            TextHelper.TextGenerator("Login unsuccessful. Please try again", ConsoleColor.Red);
            Console.ReadKey();
            continue;
        }

        return loggedInUser;
    }
}

void AdminMenu()
{
    while (true)
    {
        Console.Clear();
        TextHelper.TextGenerator("1.)[Admin] New User", ConsoleColor.Cyan);
        TextHelper.TextGenerator("2.)[Admin] Terminate User", ConsoleColor.Cyan);
        if (MainMenu()) break;
    }
}

void MaintananceMainMenu()
{
    while (true)
    {
        Console.Clear();
        TextHelper.TextGenerator("1.)[Maintenance] List all vehicles", ConsoleColor.Cyan);
        TextHelper.TextGenerator("2.)[Maintenance] License Plate Status", ConsoleColor.Cyan);
        if (MainMenu()) break;
    }
}

void ManagerMainMenu()
{
    while (true)
    {
        Console.Clear();
        TextHelper.TextGenerator("1.)[Manager] List all drivers", ConsoleColor.Cyan);
        TextHelper.TextGenerator("2.)[Manager] Taxi License Status", ConsoleColor.Cyan);
        TextHelper.TextGenerator("3.)[Manager] Driver Manager", ConsoleColor.Cyan);
        if (MainMenu()) break;
    }
}

bool MainMenu()
{
    while (true)
    {
        TextHelper.TextGenerator("8.)[All] Change Password", ConsoleColor.Cyan);
        TextHelper.TextGenerator("9.)[All] Back to Main Menu", ConsoleColor.Cyan);
        TextHelper.TextGenerator("0.)[All] Exit", ConsoleColor.Cyan);

        int choice = InputValidation("Invalid input, try again!");

        if (choice == 9)
        {
            return true;
        }
        else if (choice == 0)
        {
            return true;
        }
        else if (choice > 0 || choice < 0)
        {
            return false;
        }
    }
}

int InputValidation(string errorMessage)
{
    while (true)
    {
        string input = Console.ReadLine();
        bool inputValidation = int.TryParse(input, out int choice);

        if (!inputValidation)
        {
            TextHelper.TextGenerator(errorMessage, ConsoleColor.Red);
            Console.ReadKey();
        }
        else
        {
            return choice;
        }
    }
}