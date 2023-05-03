using TaxiManagerApp9000.Domain.Entities;
using TaxiManagerApp9000.Domain.Enums;
using TaxiManagerApp9000.Helpers;
using TaxiManagerApp9000.Services;

UserService userService = new UserService();

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
            TextHelper.TextGenerator("Login unsuccessful. Please try again!", ConsoleColor.Red);
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
        TextHelper.TextGenerator("3.)[All] Change Password", ConsoleColor.Cyan);
        TextHelper.TextGenerator("4.)[All] Back to Main Menu", ConsoleColor.Cyan);
        TextHelper.TextGenerator("5.)[All] Exit", ConsoleColor.Cyan);

        int choice = InputValidation("Invalid input, try again!");

        if (choice == 1)
        {
            //TextHelper.TextGenerator("Enter new username (must be at least 5 characters): ", ConsoleColor.Cyan);
            //string username = Console.ReadLine();

            //TextHelper.TextGenerator("Enter new password (must be at least 5 characters and contain 1 number): ", ConsoleColor.Cyan);
            //string password = Console.ReadLine();

            //TextHelper.TextGenerator("Choose role:\n1.Admin\n2.Manager\n3.Maintenance): ", ConsoleColor.Cyan);
            //int role = InputValidation("Invalid input, try again!");

            //bool success = userService.CreateUser(username, password, (UserRole)role);

            //if (success)
            //{
            //    TextHelper.TextGenerator($"Successful creation of a {(UserRole)role} user!", ConsoleColor.Green);
            //    Console.ReadKey();
            //}
            //else
            //{
            //    TextHelper.TextGenerator("Creation unsuccessful. Please try again.", ConsoleColor.Red);
            //    continue;
            //}
        }
        else if (choice == 2)
        {
            //TextHelper.TextGenerator("Enter user id to terminate: ", ConsoleColor.Cyan);
            //int userId = InputValidation("Invalid input, try again!");

            //bool success = userService.TerminateUser(userId);

            //if (success)
            //{
            //    TextHelper.TextGenerator("User terminated successfully!", ConsoleColor.Green);
            //    Console.ReadKey();
            //}
            //else
            //{
            //    TextHelper.TextGenerator("Termination unsuccessful. Please try again.", ConsoleColor.Red);
            //    continue;
            //}
        }
        else if (choice == 3)
        {
            ChangePassword(userService);
        }
        else if (choice == 4)
        {
            return;
        }
        else if (choice == 5)
        {
            Environment.Exit(0);
        }
        else
        {
            TextHelper.TextGenerator("Invalid input, try again!", ConsoleColor.Red);
        }
    }
}

void MaintananceMainMenu()
{
    while (true)
    {
        Console.Clear();
        TextHelper.TextGenerator("1.)[Maintenance] List all vehicles", ConsoleColor.Cyan);
        TextHelper.TextGenerator("2.)[Maintenance] Car License Plate Status", ConsoleColor.Cyan);
        TextHelper.TextGenerator("3.)[All] Change Password", ConsoleColor.Cyan);
        TextHelper.TextGenerator("4.)[All] Back to Main Menu", ConsoleColor.Cyan);
        TextHelper.TextGenerator("5.)[All] Exit", ConsoleColor.Cyan);

        int choice = InputValidation("Invalid input, try again!");

        if (choice == 1)
        {

        }
        else if (choice == 2)
        {

        }
        else if (choice == 3)
        {
            ChangePassword(userService);
        }
        else if (choice == 4)
        {
            return;
        }
        else if (choice == 5)
        {
            Environment.Exit(0);
        }
        else
        {
            TextHelper.TextGenerator("Invalid input, try again!", ConsoleColor.Red);
        }
    }
}

void ManagerMainMenu()
{
    while (true)
    {
        Console.Clear();
        TextHelper.TextGenerator("1.)[Manager] List all drivers", ConsoleColor.Cyan);
        TextHelper.TextGenerator("2.)[Manager] Driver License Plate Status", ConsoleColor.Cyan);
        TextHelper.TextGenerator("3.)[Manager] Driver Manager", ConsoleColor.Cyan);
        TextHelper.TextGenerator("4.)[All] Change Password", ConsoleColor.Cyan);
        TextHelper.TextGenerator("5.)[All] Back to Main Menu", ConsoleColor.Cyan);
        TextHelper.TextGenerator("6.)[All] Exit", ConsoleColor.Cyan);

        int choice = InputValidation("Invalid input, try again!");

        if (choice == 1)
        {

        }
        else if (choice == 2)
        {

        }
        else if (choice == 3)
        {

        }
        else if (choice == 4)
        {
            ChangePassword(userService);
        }
        else if (choice == 5)
        {
            return;
        }
        else if (choice == 6)
        {
            Environment.Exit(0);
        }
        else
        {
            TextHelper.TextGenerator("Invalid input, try again!", ConsoleColor.Red);
        }
    }
}

int InputValidation(string errorMessage)
{
    while (true)
    {
        string input = Console.ReadLine();
        bool inputValidation = int.TryParse(input, out int choice);

        if (!inputValidation || choice < 1 || choice > 6)
        {
            TextHelper.TextGenerator(errorMessage, ConsoleColor.Red);
        }
        else
        {
            return choice;
        }
    }
}

void ChangePassword(UserService userService)
{
    while (true)
    {
        TextHelper.TextGenerator("Enter your user id: ", ConsoleColor.Cyan);
        int userId = InputValidation("Invalid input, try again!");

        TextHelper.TextGenerator("Enter your old password: ", ConsoleColor.Cyan);
        string oldPassword = Console.ReadLine();

        TextHelper.TextGenerator("Enter your new password: ", ConsoleColor.Cyan);
        string newPassword = Console.ReadLine();

        bool success = userService.ChangePassword(userId, oldPassword, newPassword);

        if (success)
        {
            TextHelper.TextGenerator("Password changed successfully!", ConsoleColor.Green);
            Console.ReadKey();
            break;
        }
        else
        {
            TextHelper.TextGenerator("Password change failed.", ConsoleColor.Red);
            continue;
        }
    }
}