using TimeTrackingApp.DataAccess;
using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Enums;
using TimeTrackingApp.Helpers;
using TimeTrackingApp.Services;
using TimeTrackingApp.Services.Interfaces;

IUserDatabase database = new UserDatabase();
IUserManagerService userManagerService = new UserManagerService(database);
ITimerTrackerService timerService = new TimeTrackerService();

StartAppAsync();

async void StartAppAsync()
{
    TextHelper.TextGenerator("Welcome to Time Tracking APP", ConsoleColor.Green);

    if (userManagerService.CurrentUser == null)
    {
        ShowAuthMenu(userManagerService);
    }
    else
    {
        ShowMainMenu(userManagerService);
    }


}

void ShowMainMenu(IUserManagerService userManagerService)
{
    Console.WriteLine($"{UserLogOut.LOG_OUT}.Log Out\n{UserLogOut.TRACK}.Track");
    string actionChoise = Console.ReadLine();

    switch (actionChoise)
    {
        case UserLogOut.LOG_OUT:
            TextHelper.TextGenerator("Loging Out... Byee!", ConsoleColor.Yellow);
            userManagerService.LogOut();
            StartAppAsync();
            break;
        case UserLogOut.TRACK:
            ShowTrack(userManagerService);
            StartAppAsync();
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            ShowMainMenu(userManagerService);
            break;
    }
}

void ShowTrack(IUserManagerService userManagerService)
{
    TextHelper.TextGenerator(
        $"{TrackOptions.READING}.Reading\n" +
        $"{TrackOptions.EXERCISING}.Exercising\n" +
        $"{TrackOptions.WORKING}.Working\n" +
        $"{TrackOptions.OTHER_HOBBY}.Other Hobby"
        , ConsoleColor.Cyan);
    string trackChosen = Console.ReadLine();

    switch (trackChosen)
    {
        case TrackOptions.READING:
            ShowReadingActivity(userManagerService, timerService);
            break;
        case TrackOptions.EXERCISING:
            ShowExercisingActivity(userManagerService);
            break;
        case TrackOptions.WORKING:
            ShowWorkingActivity(userManagerService);
            break;
        case TrackOptions.OTHER_HOBBY:
            ShowHobbyActivity(userManagerService);
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            ShowMainMenu(userManagerService);
            break;
    }
}

void ShowHobbyActivity(IUserManagerService userManagerService)
{
    throw new NotImplementedException();
}

void ShowWorkingActivity(IUserManagerService userManagerService)
{
    throw new NotImplementedException();
}

void ShowExercisingActivity(IUserManagerService userManagerService)
{
    throw new NotImplementedException();
}

void ShowReadingActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
{
    TextHelper.TextGenerator("Timer is started", ConsoleColor.Green);
    timerService.StartTimer();

    TextHelper.TextGenerator("PressEnter when you want to stop the timer", ConsoleColor.Cyan);

    Console.ReadLine();

    timerService.StopTimer();

    TextHelper.TextGenerator("Enter the number of pages:", ConsoleColor.Cyan);
    int.TryParse(Console.ReadLine(), out int pagesCount);

    TextHelper.TextGenerator("Enter the type of book:", ConsoleColor.Cyan);
    Console.WriteLine
        (
            $"{ReadingType.Fiction}.Fiction\n" +
            $"{ReadingType.ProfessionalLiterature}.Professional Literature\n" +
            $"{ReadingType.BellesLettres}.Belles Lettres"
        );
    string type = Console.ReadLine();
}

void ShowAuthMenu(IUserManagerService userManagerService)
{
    Console.WriteLine($"{UserLogIn.LOG_IN}.Log In\n{UserLogIn.REGISTER_USER}.Register");
    string authChoice = Console.ReadLine();

    switch (authChoice)
    {
        case UserLogIn.LOG_IN:
            ShowLogIn(userManagerService);
            break;
        case UserLogIn.REGISTER_USER:
            ShowRegister(userManagerService);
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            ShowAuthMenu(userManagerService);
            break;
    }

}

void ShowRegister(IUserManagerService userManagerService)
{
    TextHelper.TextGenerator("Enter your First Name:", ConsoleColor.Cyan);
    string firstName = Console.ReadLine();

    TextHelper.TextGenerator("Enter your Last Name:", ConsoleColor.Cyan);
    string lastName = Console.ReadLine();

    TextHelper.TextGenerator("Enter your age:", ConsoleColor.Cyan);
    string age = Console.ReadLine();

    TextHelper.TextGenerator("Enter username:", ConsoleColor.Cyan);
    string username = Console.ReadLine();

    TextHelper.TextGenerator("Enter password:", ConsoleColor.Cyan);
    string password = Console.ReadLine();

    if (!int.TryParse(age, out int intAge))
    {
        TextHelper.TextGenerator("Enter a valid Age!", ConsoleColor.Red);
        StartAppAsync();
    }
    else
    {
        try
        {
            userManagerService.Register(firstName, lastName, intAge, username, password);
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            StartAppAsync();
        }
    }

}

void ShowLogIn(IUserManagerService authService)
{
    for (int i = 0; i < 3; i++)
    {
        TextHelper.TextGenerator("Enter your username:", ConsoleColor.Cyan);
        string username = Console.ReadLine();

        TextHelper.TextGenerator("Enter your password:", ConsoleColor.Cyan);
        string password = Console.ReadLine();

        try
        {
            authService.LogIn(username, password);

            TextHelper.TextGenerator($"Success! Welcome {authService.CurrentUser.FirstName}.", ConsoleColor.Green);

            StartAppAsync();
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);

            TextHelper.TextGenerator("Unsuccessful login! Try again...", ConsoleColor.Red);
        }
    }

    TextHelper.TextGenerator("Surpassed tries...\nClosing APP...\nGoodbye...", ConsoleColor.Red);
    Environment.Exit(0);
}