#region Setup
using TimeTrackingApp.DataAccess;
using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;
using TimeTrackingApp.Domain.Enums;
using TimeTrackingApp.Helpers;
using TimeTrackingApp.Services;
using TimeTrackingApp.Services.Interfaces;

IUserDatabase database = new UserDatabase();
IReadingDatabase readingDatabase = new ReadingDatabase();
IExercisingDatabase exercisingDatabase = new ExercisingDatabase();
IWorkingDatabase workingDatabase = new WorkingDatabase();
IHobbyDatabase hobbyDatabase = new HobbyDatabase();

IUserManagerService userManagerService = new UserManagerService(database);
IActivityService activityService = new ActivityService();
IActivityService newActivityService = new ActivityService(readingDatabase, exercisingDatabase, workingDatabase, hobbyDatabase, userManagerService);
ITimeTrackerService timerService = new TimeTrackerService();
#endregion

////////////////////////////////////////
// * Time Tracking App                //
// * Created by Nikola Ilievski       //
// * Version: 1.1.0 Stable            //
////////////////////////////////////////

async Task StartAppAsync()
{
    TextHelper.TextGenerator("Welcome to Time Tracking APP", ConsoleColor.Green);

    if (userManagerService.CurrentUser == null)
    {
        await ShowLoginMenu(userManagerService);
    }
    else
    {
        await ShowMainMenu(userManagerService);
    }
}

await StartAppAsync();

async Task ShowLoginMenu(IUserManagerService userManagerService)
{
    Console.WriteLine($"\n{UserLogIn.LOG_IN}.Log In\n{UserLogIn.REGISTER_USER}.Register\n{UserLogIn.EXIT_APP}.Exit App");
    string authChoice = Console.ReadLine();

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

async Task ShowLogIn(IUserManagerService userManagerService)
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
                    userManagerService.LogOut();
                    await ShowLoginMenu(userManagerService);
                    return;
                }
                else if (userInput == "Y")
                {
                    userManagerService.CurrentUser.IsActive = true;
                    await database.UpdateUserAsync(userManagerService.CurrentUser);
                }
            }

            TextHelper.TextGenerator($"\nSuccess! Welcome {userManagerService.CurrentUser.FirstName} {userManagerService.CurrentUser.LastName}.", ConsoleColor.Green);

            await StartAppAsync();
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator($"Unsuccessful login! Try again...", ConsoleColor.Red);
        }
    }

    TextHelper.TextGenerator("\nYou have tried to login 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
    Environment.Exit(0);
}

async Task ShowRegister(IUserManagerService userManagerService)
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
        await StartAppAsync();
    }
    else if (database.CheckUsernameAvailable(username))
    {
        TextHelper.TextGenerator("Username is already taken. Please choose another one.", ConsoleColor.Red);
        await StartAppAsync();
    }
    else
    {
        try
        {
            userManagerService.Register(firstName, lastName, intAge, username, password);

            TextHelper.TextGenerator("You have successfully registered!", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator(ex.Message, ConsoleColor.Red);
            await StartAppAsync();
        }
    }

    await StartAppAsync();
}

async Task ShowMainMenu(IUserManagerService userManagerService)
{
    Console.WriteLine($"{UserLogOut.LOG_OUT}.Log Out\n{UserLogOut.TRACK}.Track Activity\n{UserLogOut.STATISTICS}.Statistics\n{UserLogOut.MANAGE_ACCOUNT}.Manage Account");
    string actionChoise = Console.ReadLine();

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
    TextHelper.TextGenerator(
        $"{StatisticsOptions.READING}.Reading\n" +
        $"{StatisticsOptions.EXERCISING}.Exercising\n" +
        $"{StatisticsOptions.WORKING}.Working\n" +
        $"{StatisticsOptions.HOBBIES}.Hobbies\n" +
        $"{StatisticsOptions.GLOBAL}.Global\n" +
        $"{StatisticsOptions.BACK_TO_MAIN_MENU}.Back to main menu"
        , ConsoleColor.Cyan);
    string statisticChosen = Console.ReadLine();

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

    string favoriteActivity = newActivityService.GetFavoriteActivity();

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
    Console.ReadLine();
    await ShowStatistics(userManagerService);
}

async Task ShowManageAccountAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    TextHelper.TextGenerator(
    $"{ManageAccountOptions.CHANGE_FIRST_NAME}.Change First Name\n" +
    $"{ManageAccountOptions.CHANGE_LAST_NAME}.Change Last Name\n" +
    $"{ManageAccountOptions.CHANGE_AGE}.Change Age\n" +
    $"{ManageAccountOptions.CHANGE_USERNAME}.Change Username\n" +
    $"{ManageAccountOptions.CHANGE_PASSWORD}.Change Password\n" +
    $"{ManageAccountOptions.DEACTIVATE_ACCOUNT}.Deactivate Account\n" +
    $"{ManageAccountOptions.BACK_TO_MAIN_MENU}.Back to main menu"
    , ConsoleColor.Cyan);
    string accountOptionChosen = Console.ReadLine();

    switch (accountOptionChosen)
    {
        case ManageAccountOptions.CHANGE_FIRST_NAME:
            await AccountChangeFirstNameAsync(userManagerService, database);
            break;
        case ManageAccountOptions.CHANGE_LAST_NAME:
            await AccountChangeLastNameAsync(userManagerService, database);
            break;
        case ManageAccountOptions.CHANGE_AGE:
            await AccountChangeAgeAsync(userManagerService, database);
            break;
        case ManageAccountOptions.CHANGE_USERNAME:
            await AccountChangeUsernameAsync(userManagerService, database);
            break;
        case ManageAccountOptions.CHANGE_PASSWORD:
            await AccountChangePasswordAsync(userManagerService, database);
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

async Task AccountChangeFirstNameAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentFirstName = userManagerService.GetCurrentValidInputFromUser("first name", currentUser.FirstName, Console.ReadLine);

    string newFirstName = await userManagerService.GetNewValidInputFromUser("first name");

    currentUser.FirstName = newFirstName;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("First name updated successfully!", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

async Task AccountChangeLastNameAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentLastName = userManagerService.GetCurrentValidInputFromUser("last name", currentUser.LastName, Console.ReadLine);

    string newLastName = await userManagerService.GetNewValidInputFromUser("last name");

    currentUser.LastName = newLastName;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("Last name updated successfully!", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

async Task AccountChangeAgeAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentAge = userManagerService.GetCurrentValidInputFromUser("age", currentUser.Age.ToString(), Console.ReadLine);

    string newAge = await userManagerService.GetNewValidInputFromUser("age");

    currentUser.Age = int.Parse(newAge);
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("Age updated successfully!", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

async Task AccountChangeUsernameAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentUsername = userManagerService.GetCurrentValidInputFromUser("username", currentUser.Username, Console.ReadLine);

    string newUsername = await userManagerService.GetNewValidInputFromUser("username");

    currentUser.Username = newUsername;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("Username updated successfully!", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

async Task AccountChangePasswordAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentPassword = userManagerService.GetCurrentValidInputFromUser("password", currentUser.Password, Console.ReadLine);

    string newPassword = await userManagerService.GetNewValidInputFromUser("password");

    currentUser.Password = newPassword;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("Password updated successfully!", ConsoleColor.Green);
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