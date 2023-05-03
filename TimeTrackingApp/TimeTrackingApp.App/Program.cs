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
        ShowLoginMenu(userManagerService);
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
            TextHelper.TextGenerator("You have been logged out!", ConsoleColor.Yellow);
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
        $"{TrackOptions.OTHER_HOBBY}.Other Hobby\n" +
        $"{TrackOptions.BACK_TO_MAIN_MENU}.Back to main menu"
        , ConsoleColor.Cyan);
    string trackChosen = Console.ReadLine();

    switch (trackChosen)
    {
        case TrackOptions.READING:
            ShowReadingActivity(userManagerService, timerService);
            break;
        case TrackOptions.EXERCISING:
            ShowExercisingActivity(userManagerService, timerService);
            break;
        case TrackOptions.WORKING:
            ShowWorkingActivity(userManagerService, timerService);
            break;
        case TrackOptions.OTHER_HOBBY:
            ShowHobbyActivity(userManagerService, timerService);
            break;
        case TrackOptions.BACK_TO_MAIN_MENU:
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            ShowMainMenu(userManagerService);
            break;
    }
}

void ShowHobbyActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
{
    TextHelper.TextGenerator("Timer has started! Start working on your hobby!", ConsoleColor.Green);
    timerService.StartTimer();

    TextHelper.TextGenerator("Press ENTER when you want to stop the timer", ConsoleColor.Cyan);
    Console.ReadLine();
    timerService.StopTimer();

    TextHelper.TextGenerator("Enter the name of the hobby you were doing:", ConsoleColor.Cyan);
    string hobby = Console.ReadLine();
    while (string.IsNullOrWhiteSpace(hobby))
    {
        TextHelper.TextGenerator("Invalid input, please enter a valid hobby:", ConsoleColor.Yellow);
        hobby = Console.ReadLine();
    }
    string hobbyList = hobby;

    string time = timerService.GetTimeInMinutes();
    TextHelper.TextGenerator($"Time spent: {time}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    Console.ReadLine();
    StartAppAsync();
}

void ShowWorkingActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
{
    TextHelper.TextGenerator("Timer has started & Working has begun!", ConsoleColor.Green);
    timerService.StartTimer();

    TextHelper.TextGenerator("Press ENTER when you want to stop the timer", ConsoleColor.Cyan);
    Console.ReadLine();
    timerService.StopTimer();

    TextHelper.TextGenerator("Enter where you are working from:", ConsoleColor.Cyan);
    Console.WriteLine
        (
            $"1){Working.Office}\n" +
            $"2){Working.Home}"
        );
    int typeValue;
    while (!int.TryParse(Console.ReadLine(), out typeValue) || !Enum.IsDefined(typeof(Working), typeValue))
    {
        TextHelper.TextGenerator("Invalid input, please enter a valid work place (1-2):", ConsoleColor.Yellow);
    }
    Working workingPlace = (Working)typeValue;

    string time = timerService.GetTimeInMinutes();
    TextHelper.TextGenerator($"Time spent: {time}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    Console.ReadLine();
    StartAppAsync();
}

void ShowExercisingActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
{
    TextHelper.TextGenerator("Timer has started & Exercising has begun!", ConsoleColor.Green);
    timerService.StartTimer();

    TextHelper.TextGenerator("Press ENTER when you want to stop the timer", ConsoleColor.Cyan);
    Console.ReadLine();
    timerService.StopTimer();

    TextHelper.TextGenerator("Enter the type of exercise you are doing:", ConsoleColor.Cyan);
    Console.WriteLine
        (
            $"1){ExercisingType.Yoga}\n" +
            $"2){ExercisingType.Running}\n" +
            $"3){ExercisingType.Swimming}"
        );
    int typeValue;
    while (!int.TryParse(Console.ReadLine(), out typeValue) || !Enum.IsDefined(typeof(ExercisingType), typeValue))
    {
        TextHelper.TextGenerator("Invalid input, please enter a valid exercise type (1-3):", ConsoleColor.Yellow);
    }
    ExercisingType exercisingType = (ExercisingType)typeValue;

    string time = timerService.GetTimeInMinutes();
    TextHelper.TextGenerator($"Time spent: {time}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    Console.ReadLine();
    StartAppAsync();
}

void ShowReadingActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
{
    TextHelper.TextGenerator("Timer has started & Reading has begun!", ConsoleColor.Green);
    timerService.StartTimer();

    TextHelper.TextGenerator("Press ENTER when you want to stop the timer", ConsoleColor.Cyan);
    Console.ReadLine();
    timerService.StopTimer();

    TextHelper.TextGenerator("Enter the number of pages you have read:", ConsoleColor.Cyan);
    int pagesCount;
    while (!int.TryParse(Console.ReadLine(), out pagesCount))
    {
        TextHelper.TextGenerator("Invalid input, please enter a valid integer:", ConsoleColor.Yellow);
    }
    int pages = pagesCount;

    TextHelper.TextGenerator("Enter the type of book you are reading:", ConsoleColor.Cyan);
    Console.WriteLine
        (
            $"1){ReadingType.Romance}\n" +
            $"2){ReadingType.Fiction}\n" +
            $"3){ReadingType.Fantasy}"
        );
    int typeValue;
    while (!int.TryParse(Console.ReadLine(), out typeValue) || !Enum.IsDefined(typeof(ReadingType), typeValue))
    {
        TextHelper.TextGenerator("Invalid input, please enter a valid book type (1-3):", ConsoleColor.Yellow);
    }
    ReadingType bookType = (ReadingType)typeValue;

    string time = timerService.GetTimeInMinutes();
    TextHelper.TextGenerator($"Time spent: {time}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    Console.ReadLine();
    StartAppAsync();
}

void ShowLoginMenu(IUserManagerService userManagerService)
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
            ShowLoginMenu(userManagerService);
            break;
    }

}

void ShowRegister(IUserManagerService userManagerService)
{
    TextHelper.TextGenerator("Enter your First Name:", ConsoleColor.Cyan);
    string firstName = Console.ReadLine();

    TextHelper.TextGenerator("Enter your Last Name:", ConsoleColor.Cyan);
    string lastName = Console.ReadLine();

    TextHelper.TextGenerator("Enter your Age:", ConsoleColor.Cyan);
    string age = Console.ReadLine();

    TextHelper.TextGenerator("Enter Username:", ConsoleColor.Cyan);
    string username = Console.ReadLine();

    TextHelper.TextGenerator("Enter Password:", ConsoleColor.Cyan);
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

void ShowLogIn(IUserManagerService userManagerService)
{
    for (int i = 0; i < 3; i++)
    {
        TextHelper.TextGenerator("Enter your username:", ConsoleColor.Cyan);
        string username = Console.ReadLine();

        TextHelper.TextGenerator("Enter your password:", ConsoleColor.Cyan);
        string password = Console.ReadLine();

        try
        {
            userManagerService.LogIn(username, password);

            TextHelper.TextGenerator($"Success! Welcome {userManagerService.CurrentUser.FirstName}.", ConsoleColor.Green);

            StartAppAsync();
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator($"Unsuccessful login! Try again...", ConsoleColor.Red);
        }
    }

    TextHelper.TextGenerator("\nYou have tried to login 3 times! No more attempts left!", ConsoleColor.Red);
    Environment.Exit(0);
}