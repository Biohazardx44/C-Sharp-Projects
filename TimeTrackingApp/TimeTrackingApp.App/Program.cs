using TimeTrackingApp.DataAccess;
using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Domain.Enums;
using TimeTrackingApp.Helpers;
using TimeTrackingApp.Services;
using TimeTrackingApp.Services.Interfaces;

// Time Tracking App
// Created by Nikola Ilievski
// Version: 1.0.0 Stable

IUserDatabase database = new UserDatabase();
IReadingDatabase readingDatabase = new ReadingDatabase();
IExercisingDatabase exercisingDatabase = new ExercisingDatabase();
IWorkingDatabase workingDatabase = new WorkingDatabase();
IHobbyDatabase hobbyDatabase = new HobbyDatabase();

IActivityService activityService = new ActivityService();
ITimeTrackerService timerService = new TimeTrackerService();
IUserManagerService userManagerService = new UserManagerService(database);
IUserService userService = new UserService(readingDatabase, exercisingDatabase, workingDatabase, hobbyDatabase, userManagerService);

// Displays a welcome message and either shows the login screen or takes the user to the main menu, depending on whether the user is logged in or not
async Task StartAppAsync()
{
    // Display welcome message
    TextHelper.TextGenerator("Welcome to Time Tracking APP", ConsoleColor.Green);

    // Determine whether user is logged in
    if (userManagerService.CurrentUser == null)
    {
        // Show login screen if user is not logged in
        await ShowLoginMenu(userManagerService);
    }
    else
    {
        // Take user to main menu if user is logged in
        await ShowMainMenu(userManagerService);
    }
}

await StartAppAsync();

// Displays the login screen and prompts the user to either log in, register a new user, or exit the app
async Task ShowLoginMenu(IUserManagerService userManagerService)
{
    // Display the available options to the user
    Console.WriteLine($"\n{UserLogIn.LOG_IN}.Log In\n{UserLogIn.REGISTER_USER}.Register\n{UserLogIn.EXIT_APP}.Exit App");
    string authChoice = Console.ReadLine();

    // Handle user's choice
    switch (authChoice)
    {
        case UserLogIn.LOG_IN:
            await ShowLogIn(userManagerService);
            break;
        case UserLogIn.REGISTER_USER:
            await ShowRegister(userManagerService);
            break;
        case UserLogIn.EXIT_APP:
            Environment.Exit(0);
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            await ShowLoginMenu(userManagerService);
            break;
    }
}

// Asks the user for their username and password and attempts to log them in
async Task ShowLogIn(IUserManagerService userManagerService)
{
    for (int i = 0; i < 3; i++)
    {
        // Prompt the user for their username and password
        TextHelper.TextGenerator("Enter your username:", ConsoleColor.Cyan);
        string username = Console.ReadLine();

        TextHelper.TextGenerator("Enter your password:", ConsoleColor.Cyan);
        string password = Console.ReadLine();

        try
        {
            // Attempt to log in the user using the provided credentials
            userManagerService.LogIn(username, password);

            // If the user's account is deactivated, prompt them to reactivate it
            if (!userManagerService.CurrentUser.IsActive)
            {
                TextHelper.TextGenerator("Your account is deactivated. Do you want to reactivate it? (Y/N)", ConsoleColor.Cyan);

                string userInput = Console.ReadLine().ToUpper();
                while (userInput != "Y" && userInput != "N")
                {
                    TextHelper.TextGenerator("Invalid input. Please enter Y or N.", ConsoleColor.Red);
                    userInput = Console.ReadLine().ToUpper();
                }

                if (userInput == "N")
                {
                    // If the user chooses not to reactivate their account, log them out and show the login menu again
                    userManagerService.LogOut();
                    await ShowLoginMenu(userManagerService);
                    return;
                }
                else if (userInput == "Y")
                {
                    // If the user chooses to reactivate their account, set IsActive to true and update the user in the database
                    userManagerService.CurrentUser.IsActive = true;
                    await database.UpdateUserAsync(userManagerService.CurrentUser);
                }
            }

            // If the user is successfully logged in, show the main menu
            TextHelper.TextGenerator($"\nSuccess! Welcome {userManagerService.CurrentUser.FirstName} {userManagerService.CurrentUser.LastName}.", ConsoleColor.Green);

            await StartAppAsync();
        }
        catch (Exception ex)
        {
            // If the login attempt is unsuccessful, show an error message and allow the user to try again
            TextHelper.TextGenerator($"Unsuccessful login! Try again...", ConsoleColor.Red);
        }
    }

    // If the user has failed to log in three times, exit the application
    TextHelper.TextGenerator("\nYou have tried to login 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
    Environment.Exit(0);
}

// Asks the user for their information and attempts to register a new account
async Task ShowRegister(IUserManagerService userManagerService)
{
    // Prompt the user for their information
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
        // If the user enters an invalid age, show an error message and return to the start menu
        TextHelper.TextGenerator("Enter a valid Age!", ConsoleColor.Red);
        await StartAppAsync();
    }
    else if (database.CheckUsernameAvailable(username))
    {
        // If the username is not available, show an error message and return to the start menu
        TextHelper.TextGenerator("Username is already taken. Please choose another one.", ConsoleColor.Red);
        await StartAppAsync();
    }
    else
    {
        try
        {
            // Attempt to register a new user with the provided information
            userManagerService.Register(firstName, lastName, intAge, username, password);

            // Show a success message to the user
            TextHelper.TextGenerator("You have successfully registered!", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            // If there was an error during registration, show an error message and return to the start menu
            TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            await StartAppAsync();
        }
    }

    // Return to the start menu
    await StartAppAsync();
}

async Task ShowMainMenu(IUserManagerService userManagerService)
{
    // Display the available options to the user
    Console.WriteLine($"{UserLogOut.LOG_OUT}.Log Out\n{UserLogOut.TRACK}.Track Activity\n{UserLogOut.STATISTICS}.Statistics\n{UserLogOut.MANAGE_ACCOUNT}.Manage Account");
    string actionChoise = Console.ReadLine();

    // Handle the user's choice
    switch (actionChoise)
    {
        case UserLogOut.LOG_OUT:
            TextHelper.TextGenerator("You have been logged out!", ConsoleColor.Yellow);
            userManagerService.LogOut();
            await StartAppAsync();
            break;
        case UserLogOut.TRACK:
            await ShowTrack(userManagerService);
            await StartAppAsync();
            break;
        case UserLogOut.STATISTICS:
            await ShowStatistics(userManagerService);
            await StartAppAsync();
            break;
        case UserLogOut.MANAGE_ACCOUNT:
            await ShowManageAccountAsync(userManagerService, database);
            await StartAppAsync();
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            await ShowMainMenu(userManagerService);
            break;
    }
}

async Task ShowTrack(IUserManagerService userManagerService)
{
    // Display the available track options to the user
    TextHelper.TextGenerator(
        $"{TrackOptions.READING}.Reading\n" +
        $"{TrackOptions.EXERCISING}.Exercising\n" +
        $"{TrackOptions.WORKING}.Working\n" +
        $"{TrackOptions.OTHER_HOBBY}.Other Hobby\n" +
        $"{TrackOptions.BACK_TO_MAIN_MENU}.Back to main menu"
        , ConsoleColor.Cyan);
    string trackChosen = Console.ReadLine();

    // Handle the user's choice
    switch (trackChosen)
    {
        case TrackOptions.READING:
            await ShowReadingActivity(userManagerService, timerService);
            break;
        case TrackOptions.EXERCISING:
            await ShowExercisingActivity(userManagerService, timerService);
            break;
        case TrackOptions.WORKING:
            await ShowWorkingActivity(userManagerService, timerService);
            break;
        case TrackOptions.OTHER_HOBBY:
            await ShowHobbyActivity(userManagerService, timerService);
            break;
        case TrackOptions.BACK_TO_MAIN_MENU:
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            await ShowTrack(userManagerService);
            break;
    }
}

// This method is responsible for showing the reading activity menu and adding a reading activity to the database
async Task ShowReadingActivity(IUserManagerService userManagerService, ITimeTrackerService timerService)
{
    timerService.ActivityTimeTracker("reading");

    TextHelper.TextGenerator("Enter the number of pages you have read:", ConsoleColor.Cyan);
    int pagesCount;
    while (!int.TryParse(Console.ReadLine(), out pagesCount))
    {
        TextHelper.TextGenerator("Invalid input, please enter a valid integer:", ConsoleColor.Yellow);
    }
    int pages = pagesCount;

    ReadingType bookType = activityService.GetActivityType<ReadingType>(
        "Enter the type of book you are reading:",
        new List<ReadingType> { ReadingType.Romance, ReadingType.Fiction, ReadingType.Fantasy },
        "Invalid input, please enter a valid book type (1-3):"
    );

    int durationInSeconds = timerService.GetTimeInSeconds();
    string timeInMinutes = timerService.GetTimeInMinutes();
    TextHelper.TextGenerator($"Time spent: {timeInMinutes}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    int currentUser = userManagerService.CurrentUser.Id;
    List<ReadingActivity> readingActivities = readingDatabase.GetActivityByUserId(currentUser);

    ReadingActivity readingActivity = new ReadingActivity(currentUser, durationInSeconds, pages, bookType);
    readingActivities.Add(readingActivity);

    await readingDatabase.AddActivityAsync(readingActivity);

    Console.ReadLine();
    await ShowTrack(userManagerService);
}

// This method is responsible for showing the exercising activity menu and adding an exercising activity to the database
async Task ShowExercisingActivity(IUserManagerService userManagerService, ITimeTrackerService timerService)
{
    timerService.ActivityTimeTracker("exercising");

    ExercisingType exercisingType = activityService.GetActivityType<ExercisingType>(
        "Enter the type of exercise you are doing:",
        new List<ExercisingType> { ExercisingType.Yoga, ExercisingType.Running, ExercisingType.Swimming },
        "Invalid input, please enter a valid exercise type (1-3):"
    );

    int durationInSeconds = timerService.GetTimeInSeconds();
    string timeInMinutes = timerService.GetTimeInMinutes();
    TextHelper.TextGenerator($"Time spent: {timeInMinutes}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    int currentUser = userManagerService.CurrentUser.Id;
    List<ExercisingActivity> exercisingActivities = exercisingDatabase.GetActivityByUserId(currentUser);

    ExercisingActivity exercisingActivity = new ExercisingActivity(currentUser, durationInSeconds, exercisingType);
    exercisingActivities.Add(exercisingActivity);

    await exercisingDatabase.AddActivityAsync(exercisingActivity);

    Console.ReadLine();
    await ShowTrack(userManagerService);
}

// This method is responsible for showing the working activity menu and adding a working activity to the database
async Task ShowWorkingActivity(IUserManagerService userManagerService, ITimeTrackerService timerService)
{
    timerService.ActivityTimeTracker("work");

    Working workingPlace = activityService.GetActivityType<Working>(
        "Enter where you are working from:",
        new List<Working> { Working.Office, Working.Home },
        "Invalid input, please enter a valid work place (1-2):"
    );

    int durationInSeconds = timerService.GetTimeInSeconds();
    string timeInMinutes = timerService.GetTimeInMinutes();
    TextHelper.TextGenerator($"Time spent: {timeInMinutes}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    int currentUser = userManagerService.CurrentUser.Id;
    List<WorkingActivity> workingActivities = workingDatabase.GetActivityByUserId(currentUser);

    WorkingActivity workingActivity = new WorkingActivity(currentUser, durationInSeconds, workingPlace);
    workingActivities.Add(workingActivity);

    await workingDatabase.AddActivityAsync(workingActivity);

    Console.ReadLine();
    await ShowTrack(userManagerService);
}

// This method is responsible for showing the hobby activity menu and adding a hobby activity to the database
async Task ShowHobbyActivity(IUserManagerService userManagerService, ITimeTrackerService timerService)
{
    timerService.ActivityTimeTracker("your hobby");

    TextHelper.TextGenerator("Enter the name of the hobby you were doing:", ConsoleColor.Cyan);
    string hobby = Console.ReadLine();
    while (string.IsNullOrWhiteSpace(hobby))
    {
        TextHelper.TextGenerator("Invalid input, please enter a valid hobby:", ConsoleColor.Yellow);
        hobby = Console.ReadLine();
    }
    string currentHobby = hobby;

    int durationInSeconds = timerService.GetTimeInSeconds();
    string timeInMinutes = timerService.GetTimeInMinutes();
    TextHelper.TextGenerator($"Time spent: {timeInMinutes}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    int currentUser = userManagerService.CurrentUser.Id;
    List<Hobby> hobbyActivities = hobbyDatabase.GetActivityByUserId(currentUser);

    Hobby hobbyActivity = new Hobby(currentUser, durationInSeconds, currentHobby);
    hobbyActivities.Add(hobbyActivity);

    await hobbyDatabase.AddActivityAsync(hobbyActivity);

    Console.ReadLine();
    await ShowTrack(userManagerService);
}

async Task ShowStatistics(IUserManagerService userManagerService)
{
    // Display the available track options to the user
    TextHelper.TextGenerator(
        $"{StatisticsOptions.READING}.Reading\n" +
        $"{StatisticsOptions.EXERCISING}.Exercising\n" +
        $"{StatisticsOptions.WORKING}.Working\n" +
        $"{StatisticsOptions.HOBBIES}.Hobbies\n" +
        $"{StatisticsOptions.GLOBAL}.Global\n" +
        $"{StatisticsOptions.BACK_TO_MAIN_MENU}.Back to main menu"
        , ConsoleColor.Cyan);
    string statisticChosen = Console.ReadLine();

    // Handle the user's choice
    switch (statisticChosen)
    {
        case StatisticsOptions.READING:
            await ShowReadingStatistics(userManagerService, readingDatabase, timerService);
            break;
        case StatisticsOptions.EXERCISING:
            await ShowExercisingStatistics(userManagerService, exercisingDatabase, timerService);
            break;
        case StatisticsOptions.WORKING:
            await ShowWorkingStatistics(userManagerService, workingDatabase, timerService);
            break;
        case StatisticsOptions.HOBBIES:
            await ShowHobbiesStatistics(userManagerService, hobbyDatabase, timerService);
            break;
        case StatisticsOptions.GLOBAL:
            await ShowGlobalStatistics(userManagerService, readingDatabase, exercisingDatabase, workingDatabase, hobbyDatabase, timerService);
            break;
        case StatisticsOptions.BACK_TO_MAIN_MENU:
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            await ShowStatistics(userManagerService);
            break;
    }
}

async Task ShowReadingStatistics(IUserManagerService userManagerService, IReadingDatabase readingDatabase, ITimeTrackerService timerService)
{
    List<ReadingActivity> readingActivities = readingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    if (readingActivities == null || readingActivities.Count == 0)
    {
        TextHelper.TextGenerator($"User does not have any reading activities yet!", ConsoleColor.Yellow);
        TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
        Console.ReadLine();
        await ShowStatistics(userManagerService);
        return;
    }

    int totalDurationInSeconds = readingActivities.Sum(x => x.Duration);
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    int totalHours = (int)totalDuration.TotalHours;
    int remainingMinutes = totalDuration.Minutes;
    int remainingSeconds = totalDuration.Seconds;

    TextHelper.TextGenerator($"Total time: {totalHours} hours, {remainingMinutes} minutes, and {remainingSeconds} seconds", ConsoleColor.Cyan);

    int averageDurationInSeconds = totalDurationInSeconds / readingActivities.Count;
    TimeSpan averageDuration = TimeSpan.FromSeconds(averageDurationInSeconds);

    int averageMinutes = (int)averageDuration.TotalMinutes;
    int averageSeconds = averageDuration.Seconds;

    TextHelper.TextGenerator($"Average of all activity records: {averageMinutes} minutes, and {averageSeconds} seconds", ConsoleColor.Cyan);

    int totalPageCount = 0;

    foreach (ReadingActivity activity in readingActivities)
    {
        totalPageCount += activity.PageCount;
    }
    TextHelper.TextGenerator($"Total number of pages: {totalPageCount}", ConsoleColor.Cyan);

    Dictionary<ReadingType, int> typeCounts = new Dictionary<ReadingType, int>();
    foreach (ReadingActivity activity in readingActivities)
    {
        if (typeCounts.ContainsKey(activity.ReadingType))
        {
            typeCounts[activity.ReadingType]++;
        }
        else
        {
            typeCounts[activity.ReadingType] = 1;
        }
    }

    int maxCount = 0;
    List<ReadingType> favoriteTypes = new List<ReadingType>();
    foreach (var key in typeCounts)
    {
        if (key.Value > maxCount)
        {
            maxCount = key.Value;
            favoriteTypes.Clear();
            favoriteTypes.Add(key.Key);
        }
        else if (key.Value == maxCount)
        {
            favoriteTypes.Add(key.Key);
        }
    }

    if (favoriteTypes.Count > 0)
    {
        string typesString = String.Join(", ", favoriteTypes);
        TextHelper.TextGenerator($"Favorite Types: {typesString}", ConsoleColor.Cyan);
    }
    else
    {
        TextHelper.TextGenerator($"Favorite Types: User does not have a favorite type!", ConsoleColor.Red);
    }

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
    Console.ReadLine();
    await ShowStatistics(userManagerService);
}

async Task ShowExercisingStatistics(IUserManagerService userManagerService, IExercisingDatabase exercisingDatabase, ITimeTrackerService timerService)
{
    List<ExercisingActivity> exercisingActivities = exercisingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    if (exercisingActivities == null || exercisingActivities.Count == 0)
    {
        TextHelper.TextGenerator($"User does not have any reading activities yet!", ConsoleColor.Yellow);
        TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
        Console.ReadLine();
        await ShowStatistics(userManagerService);
        return;
    }

    int totalDurationInSeconds = exercisingActivities.Sum(x => x.Duration);
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    int totalHours = (int)totalDuration.TotalHours;
    int remainingMinutes = totalDuration.Minutes;
    int remainingSeconds = totalDuration.Seconds;

    TextHelper.TextGenerator($"Total time: {totalHours} hours, {remainingMinutes} minutes, and {remainingSeconds} seconds", ConsoleColor.Cyan);

    int averageDurationInSeconds = totalDurationInSeconds / exercisingActivities.Count;
    TimeSpan averageDuration = TimeSpan.FromSeconds(averageDurationInSeconds);

    int averageMinutes = (int)averageDuration.TotalMinutes;
    int averageSeconds = averageDuration.Seconds;

    TextHelper.TextGenerator($"Average of all activity records: {averageMinutes} minutes, and {averageSeconds} seconds", ConsoleColor.Cyan);

    Dictionary<ExercisingType, int> typeCounts = new Dictionary<ExercisingType, int>();
    foreach (ExercisingActivity activity in exercisingActivities)
    {
        if (typeCounts.ContainsKey(activity.ExercisingType))
        {
            typeCounts[activity.ExercisingType]++;
        }
        else
        {
            typeCounts[activity.ExercisingType] = 1;
        }
    }

    int maxCount = 0;
    List<ExercisingType> favoriteTypes = new List<ExercisingType>();
    foreach (var key in typeCounts)
    {
        if (key.Value > maxCount)
        {
            maxCount = key.Value;
            favoriteTypes.Clear();
            favoriteTypes.Add(key.Key);
        }
        else if (key.Value == maxCount)
        {
            favoriteTypes.Add(key.Key);
        }
    }

    if (favoriteTypes.Count > 0)
    {
        string typesString = String.Join(", ", favoriteTypes);
        TextHelper.TextGenerator($"Favorite Types: {typesString}", ConsoleColor.Cyan);
    }
    else
    {
        TextHelper.TextGenerator($"Favorite Types: User does not have a favorite type!", ConsoleColor.Red);
    }

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
    Console.ReadLine();
    await ShowStatistics(userManagerService);
}

async Task ShowWorkingStatistics(IUserManagerService userManagerService, IWorkingDatabase workingDatabase, ITimeTrackerService timerService)
{
    List<WorkingActivity> workingActivities = workingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    if (workingActivities == null || workingActivities.Count == 0)
    {
        TextHelper.TextGenerator($"User does not have any reading activities yet!", ConsoleColor.Yellow);
        TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
        Console.ReadLine();
        await ShowStatistics(userManagerService);
        return;
    }

    int totalDurationInSeconds = workingActivities.Sum(x => x.Duration);
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    int totalHours = (int)totalDuration.TotalHours;
    int remainingMinutes = totalDuration.Minutes;
    int remainingSeconds = totalDuration.Seconds;

    TextHelper.TextGenerator($"Total time: {totalHours} hours, {remainingMinutes} minutes, and {remainingSeconds} seconds", ConsoleColor.Cyan);

    int averageDurationInSeconds = totalDurationInSeconds / workingActivities.Count;
    TimeSpan averageDuration = TimeSpan.FromSeconds(averageDurationInSeconds);

    int averageMinutes = (int)averageDuration.TotalMinutes;
    int averageSeconds = averageDuration.Seconds;

    TextHelper.TextGenerator($"Average of all activity records: {averageMinutes} minutes, and {averageSeconds} seconds", ConsoleColor.Cyan);

    int totalDurationAtOffice = workingActivities.Where(x => x.Working == Working.Office).Sum(x => x.Duration);
    TimeSpan totalDurationAtOfficeTimeSpan = TimeSpan.FromSeconds(totalDurationAtOffice);

    int totalDurationAtHome = workingActivities.Where(x => x.Working == Working.Home).Sum(x => x.Duration);
    TimeSpan totalDurationAtHomeTimeSpan = TimeSpan.FromSeconds(totalDurationAtHome);

    int totalOfficeHours = (int)totalDurationAtOfficeTimeSpan.TotalHours;
    int totaHomeHours = (int)totalDurationAtHomeTimeSpan.TotalHours;

    string officeTime = $"{totalOfficeHours} hours, {totalDurationAtOfficeTimeSpan.Minutes} minutes, and {totalDurationAtOfficeTimeSpan.Seconds} seconds";
    string homeTime = $"{totaHomeHours} hours, {totalDurationAtHomeTimeSpan.Minutes} minutes, and {totalDurationAtHomeTimeSpan.Seconds} seconds";

    TextHelper.TextGenerator($"Total working time: Home({homeTime}) VS Office({officeTime})", ConsoleColor.Cyan);

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
    Console.ReadLine();
    await ShowStatistics(userManagerService);
}

async Task ShowHobbiesStatistics(IUserManagerService userManagerService, IHobbyDatabase hobbyDatabase, ITimeTrackerService timerService)
{
    List<Hobby> hobbies = hobbyDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    if (hobbies == null || hobbies.Count == 0)
    {
        TextHelper.TextGenerator($"User does not have any reading activities yet!", ConsoleColor.Yellow);
        TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
        Console.ReadLine();
        await ShowStatistics(userManagerService);
        return;
    }

    int totalDurationInSeconds = hobbies.Sum(x => x.Duration);
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    int totalHours = (int)totalDuration.TotalHours;
    int remainingMinutes = totalDuration.Minutes;
    int remainingSeconds = totalDuration.Seconds;

    TextHelper.TextGenerator($"Total time: {totalHours} hours, {remainingMinutes} minutes, and {remainingSeconds} seconds", ConsoleColor.Cyan);

    List<string> distinctNames = hobbies.Select(h => h.HobbyName).Distinct().ToList();
    if (distinctNames.Count > 0)
    {
        TextHelper.TextGenerator($"List of all hobbies: ", ConsoleColor.Cyan);
        foreach (string name in distinctNames)
        {
            TextHelper.TextGenerator($"- {name}", ConsoleColor.Blue);
        }
    }
    else
    {
        TextHelper.TextGenerator($"List of all hobbies: User does not have any recorded hobbies!", ConsoleColor.Red);
    }

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
    Console.ReadLine();
    await ShowStatistics(userManagerService);
}

async Task ShowGlobalStatistics(IUserManagerService userManagerService, IReadingDatabase readingDatabase, IExercisingDatabase exercisingDatabase, IWorkingDatabase workingDatabase, IHobbyDatabase hobbyDatabase, ITimeTrackerService timerService)
{
    List<ReadingActivity> readingActivities = readingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);
    int readingTotalDurationInSeconds = readingActivities.Sum(x => x.Duration);

    List<ExercisingActivity> exercisingActivities = exercisingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);
    int exercisingTotalDurationInSeconds = exercisingActivities.Sum(x => x.Duration);

    List<WorkingActivity> workingActivities = workingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);
    int workingTotalDurationInSeconds = workingActivities.Sum(x => x.Duration);

    List<Hobby> hobbyActivities = hobbyDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);
    int hobbyTotalDurationInSeconds = hobbyActivities.Sum(x => x.Duration);

    int totalDurationInSeconds = readingTotalDurationInSeconds + exercisingTotalDurationInSeconds + workingTotalDurationInSeconds + hobbyTotalDurationInSeconds;
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    int totalHours = (int)totalDuration.TotalHours;
    int remainingMinutes = totalDuration.Minutes;
    int remainingSeconds = totalDuration.Seconds;

    TextHelper.TextGenerator($"Total time of all activities: {totalHours} hours, {remainingMinutes} minutes, and {remainingSeconds} seconds", ConsoleColor.Cyan);

    string favoriteActivity = userService.GetFavoriteActivity();

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
    Console.ReadLine();
    await ShowStatistics(userManagerService);
}

async Task ShowManageAccountAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    // Display the available options to the user
    TextHelper.TextGenerator(
    $"{ManageAccountOptions.CHANGE_PASSWORD}.Change password\n" +
    $"{ManageAccountOptions.CHANGE_FIRST_NAME}.Change First Name\n" +
    $"{ManageAccountOptions.CHANGE_LAST_NAME}.Change Last Name\n" +
    $"{ManageAccountOptions.DEACTIVATE_ACCOUNT}.Deactivate account\n" +
    $"{ManageAccountOptions.BACK_TO_MAIN_MENU}.Back to main menu"
    , ConsoleColor.Cyan);
    string accountOptionChosen = Console.ReadLine();

    // Handle the user's choice
    switch (accountOptionChosen)
    {
        case ManageAccountOptions.CHANGE_PASSWORD:
            await AccountChangePasswordAsync(userManagerService, database);
            break;
        case ManageAccountOptions.CHANGE_FIRST_NAME:
            await AccountChangeFirstNameAsync(userManagerService, database);
            break;
        case ManageAccountOptions.CHANGE_LAST_NAME:
            await AccountChangeLastNameAsync(userManagerService, database);
            break;
        case ManageAccountOptions.DEACTIVATE_ACCOUNT:
            await AccountDeactivationAsync(userManagerService, database);
            break;
        case ManageAccountOptions.BACK_TO_MAIN_MENU:
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            await ShowManageAccountAsync(userManagerService, database);
            break;
    }
}

async Task AccountChangePasswordAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;
    int incorrectAttempts = 0;

    while (incorrectAttempts < 3)
    {
        TextHelper.TextGenerator("Enter your current password:", ConsoleColor.Cyan);
        string currentPassword = Console.ReadLine();

        if (currentUser.Password != currentPassword)
        {
            TextHelper.TextGenerator("The current password you entered is incorrect.", ConsoleColor.Red);
            incorrectAttempts++;
        }
        else
        {
            break;
        }
    }

    if (incorrectAttempts == 3)
    {
        TextHelper.TextGenerator("\nYou have tried to enter your current password 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
        Environment.Exit(0);
    }

    string newPassword;
    while (true)
    {
        TextHelper.TextGenerator("Enter the new password:", ConsoleColor.Cyan);
        newPassword = Console.ReadLine();

        try
        {
            currentUser.ValidatePassword(newPassword);
            break;
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            incorrectAttempts++;
        }

        if (incorrectAttempts == 3)
        {
            TextHelper.TextGenerator("\nYou have tried to enter a valid password 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
            Environment.Exit(0);
        }
    }

    currentUser.Password = newPassword;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("Password updated successfully!", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

async Task AccountChangeFirstNameAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;
    int incorrectAttempts = 0;

    while (incorrectAttempts < 3)
    {
        TextHelper.TextGenerator("Enter your current first name:", ConsoleColor.Cyan);
        string currentFirstName = Console.ReadLine();

        if (currentUser.FirstName != currentFirstName)
        {
            TextHelper.TextGenerator("The current first name you entered is incorrect.", ConsoleColor.Red);
            incorrectAttempts++;
        }
        else
        {
            break;
        }
    }

    if (incorrectAttempts == 3)
    {
        TextHelper.TextGenerator("\nYou have tried to enter your current first name 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
        Environment.Exit(0);
    }

    string newFirstName;
    while (true)
    {
        TextHelper.TextGenerator("Enter the new first name:", ConsoleColor.Cyan);
        newFirstName = Console.ReadLine();

        try
        {
            currentUser.ValidateNameInput(newFirstName);
            break;
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            incorrectAttempts++;
        }

        if (incorrectAttempts == 3)
        {
            TextHelper.TextGenerator("\nYou have tried to enter a valid first name 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
            Environment.Exit(0);
        }
    }

    currentUser.FirstName = newFirstName;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("First name updated successfully!", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

async Task AccountChangeLastNameAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;
    int incorrectAttempts = 0;

    while (incorrectAttempts < 3)
    {
        TextHelper.TextGenerator("Enter your current last name:", ConsoleColor.Cyan);
        string currentLastName = Console.ReadLine();

        if (currentUser.LastName != currentLastName)
        {
            TextHelper.TextGenerator("The current last name you entered is incorrect.", ConsoleColor.Red);
            incorrectAttempts++;
        }
        else
        {
            break;
        }
    }

    if (incorrectAttempts == 3)
    {
        TextHelper.TextGenerator("\nYou have tried to enter your current last name 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
        Environment.Exit(0);
    }

    string newLastName;
    while (true)
    {
        TextHelper.TextGenerator("Enter the new last name:", ConsoleColor.Cyan);
        newLastName = Console.ReadLine();

        try
        {
            currentUser.ValidateNameInput(newLastName);
            break;
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            incorrectAttempts++;
        }

        if (incorrectAttempts == 3)
        {
            TextHelper.TextGenerator("\nYou have tried to enter a valid last name 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
            Environment.Exit(0);
        }
    }

    currentUser.LastName = newLastName;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("Last name updated successfully!", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

async Task AccountDeactivationAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;
    currentUser.IsActive = false;

    TextHelper.TextGenerator("Do you want to deactivate your account? (Y/N)", ConsoleColor.Cyan);

    string userInput = Console.ReadLine().ToUpper();
    while (userInput != "Y" && userInput != "N")
    {
        TextHelper.TextGenerator("Invalid input. Please enter Y or N.", ConsoleColor.Red);
        userInput = Console.ReadLine().ToUpper();
    }

    if (userInput == "N")
    {
        currentUser.IsActive = true;
        await database.UpdateUserAsync(currentUser);
        TextHelper.TextGenerator("Account deactivation cancelled.", ConsoleColor.Green);
        Console.ReadKey();
        await ShowManageAccountAsync(userManagerService, database);
    }
    else if (userInput == "Y")
    {
        await database.UpdateUserAsync(currentUser);
        TextHelper.TextGenerator("Your account has been deactivated.", ConsoleColor.Red);
        Console.ReadKey();
        await ShowLoginMenu(userManagerService);
    }
}