#region Setup
using System.Data;
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
// * Version: 1.2.0 Stable            //
////////////////////////////////////////

/// <summary>
/// Asynchronously starts the Time Tracking App.
/// </summary>
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

#region Login & Register Menu
/// <summary>
/// Displays the login menu and prompts the user to log in, register, or exit the app.
/// </summary>
/// <param name="userManagerService">An instance of the IUserManagerService interface.</param>
/// <returns>A Task representing the asynchronous operation.</returns>
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
            TextHelper.WaitAndClear();
            await StartAppAsync();
            break;
    }
}

/// <summary>
/// Displays the login menu and prompts the user to enter their username and password to log in.
/// </summary>
/// <param name="userManagerService">An instance of the IUserManagerService interface.</param>
/// <returns>A Task representing the asynchronous operation.</returns>
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
                    TextHelper.TextGenerator("Account activation cancelled.", ConsoleColor.Red);
                    TextHelper.WaitAndClear();
                    TextHelper.TextGenerator("Welcome to Time Tracking APP", ConsoleColor.Green);

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

            TextHelper.TextGenerator($"Success! Welcome {userManagerService.CurrentUser.FirstName} {userManagerService.CurrentUser.LastName}.", ConsoleColor.Green);

            TextHelper.WaitAndClear();
            await StartAppAsync();
        }
        catch (Exception ex)
        {
            TextHelper.TextGenerator($"Unsuccessful login! Try again...\n", ConsoleColor.Red);
        }
    }

    TextHelper.TextGenerator("You have tried to login 3 times! No more attempts left. Exiting application...", ConsoleColor.Red);
    Environment.Exit(0);
}

/// <summary>
/// Shows the register menu to the user and prompts the user to enter their personal details in order to register
/// with the application. Validates the user input and registers the user using the UserManagerService if input is valid.
/// </summary>
/// <param name="userManagerService">The IUserManagerService instance used to register the user.</param>
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
        TextHelper.WaitAndClear();
        await StartAppAsync();
    }
    else if (database.CheckUsernameAvailable(username))
    {
        TextHelper.TextGenerator("Username already exists. Please choose a different username.", ConsoleColor.Red);
        TextHelper.WaitAndClear();
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
            TextHelper.WaitAndClear();
            await StartAppAsync();
        }
    }

    TextHelper.WaitAndClear();
    await StartAppAsync();
}
#endregion

/// <summary>
/// Shows the main menu options to the user and prompts the user to select an action to perform. 
/// Redirects to appropriate methods based on user input.
/// </summary>
/// <param name="userManagerService">The IUserManagerService instance used to manage user information.</param>
async Task ShowMainMenu(IUserManagerService userManagerService)
{
    Console.WriteLine($"\n{UserLogOut.LOG_OUT}.Log Out\n{UserLogOut.TRACK}.Track Activity\n{UserLogOut.STATISTICS}.Statistics\n{UserLogOut.MANAGE_ACCOUNT}.Manage Account");
    string actionChoise = Console.ReadLine();

    switch (actionChoise)
    {
        case UserLogOut.LOG_OUT:
            TextHelper.TextGenerator("You have been logged out!", ConsoleColor.Yellow);
            userManagerService.LogOut();
            TextHelper.WaitAndClear();
            await StartAppAsync();
            break;
        case UserLogOut.TRACK:
            Console.Clear();
            TextHelper.TextGenerator("Welcome to tracking menu\n", ConsoleColor.Green);
            await ShowTrack(userManagerService);
            await StartAppAsync();
            break;
        case UserLogOut.STATISTICS:
            Console.Clear();
            TextHelper.TextGenerator("Welcome to statistics menu\n", ConsoleColor.Green);
            await ShowStatistics(userManagerService);
            await StartAppAsync();
            break;
        case UserLogOut.MANAGE_ACCOUNT:
            Console.Clear();
            TextHelper.TextGenerator("Welcome to account management menu\n", ConsoleColor.Green);
            await ShowManageAccountAsync(userManagerService, database);
            await StartAppAsync();
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            TextHelper.WaitAndClear();
            await StartAppAsync();
            break;
    }
}

/// <summary>
/// Shows the different activity tracking options to the user and prompts the user to select an activity to track. 
/// Redirects to appropriate methods based on user input.
/// </summary>
/// <param name="userManagerService">The IUserManagerService instance used to manage user information.</param>
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
            Console.Clear();
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            TextHelper.WaitAndClearLine();
            TextHelper.TextGenerator("Welcome to tracking menu\n", ConsoleColor.Green);
            await ShowTrack(userManagerService);
            break;
    }
}

#region Tracking Menu
/// <summary>
/// Shows the reading activity menu to the user and prompts the user to enter details about their reading activity.
/// Validates the user input and tracks the activity using the ReadingDatabase if input is valid.
/// </summary>
/// <param name="userManagerService">The IUserManagerService instance used to get the current user.</param>
/// <param name="timerService">The ITimeTrackerService instance used to track the activity duration.</param>
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
        "\nEnter the type of book you are reading:",
        new List<ReadingType> { ReadingType.Romance, ReadingType.Fiction, ReadingType.Fantasy },
        "Invalid input, please enter a valid book type (1-3):"
    );

    int durationInSeconds = timerService.GetTimeInSeconds();
    string timeInMinutes = timerService.GetTimeInMinutes(durationInSeconds);
    TextHelper.TextGenerator($"\nTime spent: {timeInMinutes}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    int currentUser = userManagerService.CurrentUser.Id;
    List<ReadingActivity> readingActivities = readingDatabase.GetActivityByUserId(currentUser);

    ReadingActivity readingActivity = new ReadingActivity(currentUser, durationInSeconds, pages, bookType);
    readingActivities.Add(readingActivity);

    await readingDatabase.AddActivityAsync(readingActivity);

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to tracking menu\n", ConsoleColor.Green);
    await ShowTrack(userManagerService);
}

/// <summary>
/// Shows the exercising activity menu to the user and prompts the user to enter details about their exercising activity.
/// Validates the user input and tracks the activity using the ExercisingDatabase if input is valid.
/// </summary>
/// <param name="userManagerService">The IUserManagerService instance used to get the current user.</param>
/// <param name="timerService">The ITimeTrackerService instance used to track the activity duration.</param>
async Task ShowExercisingActivity(IUserManagerService userManagerService, ITimeTrackerService timerService)
{
    timerService.ActivityTimeTracker("exercising");

    ExercisingType exercisingType = activityService.GetActivityType<ExercisingType>(
        "Enter the type of exercise you are doing:",
        new List<ExercisingType> { ExercisingType.Yoga, ExercisingType.Running, ExercisingType.Swimming },
        "Invalid input, please enter a valid exercise type (1-3):"
    );

    int durationInSeconds = timerService.GetTimeInSeconds();
    string timeInMinutes = timerService.GetTimeInMinutes(durationInSeconds);
    TextHelper.TextGenerator($"\nTime spent: {timeInMinutes}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    int currentUser = userManagerService.CurrentUser.Id;
    List<ExercisingActivity> exercisingActivities = exercisingDatabase.GetActivityByUserId(currentUser);

    ExercisingActivity exercisingActivity = new ExercisingActivity(currentUser, durationInSeconds, exercisingType);
    exercisingActivities.Add(exercisingActivity);

    await exercisingDatabase.AddActivityAsync(exercisingActivity);

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to tracking menu\n", ConsoleColor.Green);
    await ShowTrack(userManagerService);
}

/// <summary>
/// Shows the working activity menu to the user and prompts the user to enter details about their working activity.
/// Validates the user input and tracks the activity using the WorkingDatabase if input is valid.
/// </summary>
/// <param name="userManagerService">The IUserManagerService instance used to get the current user.</param>
/// <param name="timerService">The ITimeTrackerService instance used to track the activity duration.</param>
async Task ShowWorkingActivity(IUserManagerService userManagerService, ITimeTrackerService timerService)
{
    timerService.ActivityTimeTracker("work");

    Working workingPlace = activityService.GetActivityType<Working>(
        "Enter where you are working from:",
        new List<Working> { Working.Office, Working.Home },
        "Invalid input, please enter a valid work place (1-2):"
    );

    int durationInSeconds = timerService.GetTimeInSeconds();
    string timeInMinutes = timerService.GetTimeInMinutes(durationInSeconds);
    TextHelper.TextGenerator($"\nTime spent: {timeInMinutes}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    int currentUser = userManagerService.CurrentUser.Id;
    List<WorkingActivity> workingActivities = workingDatabase.GetActivityByUserId(currentUser);

    WorkingActivity workingActivity = new WorkingActivity(currentUser, durationInSeconds, workingPlace);
    workingActivities.Add(workingActivity);

    await workingDatabase.AddActivityAsync(workingActivity);

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to tracking menu\n", ConsoleColor.Green);
    await ShowTrack(userManagerService);
}

/// <summary>
/// Shows the hobby activity menu to the user and prompts the user to enter details about their hobby activity.
/// Validates the user input and tracks the activity using the HobbyDatabase if input is valid.
/// </summary>
/// <param name="userManagerService">The IUserManagerService instance used to get the current user.</param>
/// <param name="timerService">The ITimeTrackerService instance used to track the activity duration.</param>
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
    string timeInMinutes = timerService.GetTimeInMinutes(durationInSeconds);
    TextHelper.TextGenerator($"\nTime spent: {timeInMinutes}\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    int currentUser = userManagerService.CurrentUser.Id;
    List<Hobby> hobbyActivities = hobbyDatabase.GetActivityByUserId(currentUser);

    Hobby hobbyActivity = new Hobby(currentUser, durationInSeconds, currentHobby);
    hobbyActivities.Add(hobbyActivity);

    await hobbyDatabase.AddActivityAsync(hobbyActivity);

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to tracking menu\n", ConsoleColor.Green);
    await ShowTrack(userManagerService);
}
#endregion

/// <summary>
/// Shows the statistics menu to the user and prompts the user to choose a specific activity to view statistics for.
/// Validates the user input and calls the appropriate method to show the statistics using the corresponding database and ITimeTrackerService instance if input is valid.
/// </summary>
/// <param name="userManagerService">The IUserManagerService instance used to get the current user.</param>
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
            Console.Clear();
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            TextHelper.WaitAndClearLine();
            TextHelper.TextGenerator("Welcome to statistics menu\n", ConsoleColor.Green);
            await ShowStatistics(userManagerService);
            break;
    }
}

#region Statistics Menu
/// <summary>
/// Checks if the specified activity list contains any activities and displays a message if it is empty.
/// </summary>
/// <typeparam name="T">Type of the activity</typeparam>
/// <param name="activities">List of activities to check</param>
/// <param name="activityTypeName">Name of the activity type</param>
/// <param name="userManagerService">An instance of the IUserManagerService</param>
/// <returns>Returns true if the activity list is not empty, false otherwise</returns>
async Task<bool> CheckActivitiesExist<T>(List<T> activities, string activityTypeName, IUserManagerService userManagerService)
{
    if (activities == null || activities.Count == 0)
    {
        TextHelper.TextGenerator($"\nUser does not have any {activityTypeName} activities yet!", ConsoleColor.Red);
        TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

        TextHelper.WaitAndClearLine();
        TextHelper.TextGenerator("Welcome to statistics menu\n", ConsoleColor.Green);
        await ShowStatistics(userManagerService);
        return false;
    }
    return true;
}

/// <summary>
/// Shows reading statistics for the current user.
/// </summary>
/// <param name="userManagerService">The user manager service to get the current user.</param>
/// <param name="readingDatabase">The reading database to get the reading activities.</param>
/// <param name="timerService">The time tracker service to convert the durations to readable strings.</param>
async Task ShowReadingStatistics(IUserManagerService userManagerService, IReadingDatabase readingDatabase, ITimeTrackerService timerService)
{
    List<ReadingActivity> readingActivities = readingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    if (!await CheckActivitiesExist(readingActivities, "reading", userManagerService))
    {
        return;
    }

    int totalDurationInSeconds = readingActivities.Sum(x => x.Duration);
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    string totalTime = timerService.GetTimeInHours((int)totalDuration.TotalSeconds);

    TextHelper.TextGenerator($"\nTotal time: {totalTime}", ConsoleColor.Cyan);

    int averageDurationInSeconds = totalDurationInSeconds / readingActivities.Count;
    string averageDuration = timerService.GetTimeInMinutes(averageDurationInSeconds);

    TextHelper.TextGenerator($"Average of all activity records: {averageDuration}", ConsoleColor.Cyan);

    int totalPageCount = 0;

    foreach (ReadingActivity activity in readingActivities)
    {
        totalPageCount += activity.PageCount;
    }
    TextHelper.TextGenerator($"Total number of pages: {totalPageCount}", ConsoleColor.Cyan);

    activityService.DisplayFavoriteTypes(readingActivities, x => x.ReadingType, "reading");

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to statistics menu\n", ConsoleColor.Green);
    await ShowStatistics(userManagerService);
}

/// <summary>
/// Shows exercising statistics for the current user.
/// </summary>
/// <param name="userManagerService">The user manager service to get the current user.</param>
/// <param name="exercisingDatabase">The exercising database to get the exercising activities.</param>
/// <param name="timerService">The time tracker service to convert the durations to readable strings.</param>
async Task ShowExercisingStatistics(IUserManagerService userManagerService, IExercisingDatabase exercisingDatabase, ITimeTrackerService timerService)
{
    List<ExercisingActivity> exercisingActivities = exercisingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    if (!await CheckActivitiesExist(exercisingActivities, "exercising", userManagerService))
    {
        return;
    }

    int totalDurationInSeconds = exercisingActivities.Sum(x => x.Duration);
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    string totalTime = timerService.GetTimeInHours((int)totalDuration.TotalSeconds);

    TextHelper.TextGenerator($"\nTotal time: {totalTime}", ConsoleColor.Cyan);

    int averageDurationInSeconds = totalDurationInSeconds / exercisingActivities.Count;
    string averageDuration = timerService.GetTimeInMinutes(averageDurationInSeconds);

    TextHelper.TextGenerator($"Average of all activity records: {averageDuration}", ConsoleColor.Cyan);

    activityService.DisplayFavoriteTypes(exercisingActivities, x => x.ExercisingType, "exercising");

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to statistics menu\n", ConsoleColor.Green);
    await ShowStatistics(userManagerService);
}

/// <summary>
/// Shows working statistics for the current user.
/// </summary>
/// <param name="userManagerService">The user manager service to get the current user.</param>
/// <param name="workingDatabase">The working database to get the working activities.</param>
/// <param name="timerService">The time tracker service to convert the durations to readable strings.</param>
async Task ShowWorkingStatistics(IUserManagerService userManagerService, IWorkingDatabase workingDatabase, ITimeTrackerService timerService)
{
    List<WorkingActivity> workingActivities = workingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    if (!await CheckActivitiesExist(workingActivities, "working", userManagerService))
    {
        return;
    }

    int totalDurationInSeconds = workingActivities.Sum(x => x.Duration);
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    string totalTime = timerService.GetTimeInHours((int)totalDuration.TotalSeconds);

    TextHelper.TextGenerator($"\nTotal time: {totalTime}", ConsoleColor.Cyan);

    int averageDurationInSeconds = totalDurationInSeconds / workingActivities.Count;
    string averageDuration = timerService.GetTimeInMinutes(averageDurationInSeconds);

    TextHelper.TextGenerator($"Average of all activity records: {averageDuration}", ConsoleColor.Cyan);

    int totalDurationAtOffice = workingActivities.Where(x => x.Working == Working.Office).Sum(x => x.Duration);
    string officeTime = timerService.GetTimeInHours(totalDurationAtOffice);

    int totalDurationAtHome = workingActivities.Where(x => x.Working == Working.Home).Sum(x => x.Duration);
    string homeTime = timerService.GetTimeInHours(totalDurationAtHome);

    TextHelper.TextGenerator($"Total working time: Home({homeTime}) VS Office({officeTime})", ConsoleColor.Cyan);

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to statistics menu\n", ConsoleColor.Green);
    await ShowStatistics(userManagerService);
}

/// <summary>
/// Shows hobby statistics for the current user.
/// </summary>
/// <param name="userManagerService">The user manager service to get the current user.</param>
/// <param name="hobbyDatabase">The hobby database to get the hobbies.</param>
/// <param name="timerService">The time tracker service to convert the durations to readable strings.</param>
async Task ShowHobbiesStatistics(IUserManagerService userManagerService, IHobbyDatabase hobbyDatabase, ITimeTrackerService timerService)
{
    List<Hobby> hobbies = hobbyDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    int totalDurationInSeconds = hobbies.Sum(x => x.Duration);
    TimeSpan totalDuration = TimeSpan.FromSeconds(totalDurationInSeconds);

    string totalTime = timerService.GetTimeInHours((int)totalDuration.TotalSeconds);

    TextHelper.TextGenerator($"\nTotal time: {totalTime}", ConsoleColor.Cyan);

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

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to statistics menu\n", ConsoleColor.Green);
    await ShowStatistics(userManagerService);
}

/// <summary>
/// Shows the global statistics of all activities for the current user, including the user's favorite activity and overall total time.
/// </summary>
/// <param name="userManagerService">The user manager service to get the current user.</param>
/// <param name="readingDatabase">The reading database to retrieve reading activities from.</param>
/// <param name="exercisingDatabase">The exercising database to retrieve exercising activities from.</param>
/// <param name="workingDatabase">The working database to retrieve working activities from.</param>
/// <param name="hobbyDatabase">The hobby database to retrieve hobby activities from.</param>
/// <param name="timerService">The time tracker service to format the total duration.</param>
/// <returns>A task that represents the asynchronous operation.</returns>
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

    string totalTime = timerService.GetTimeInHours((int)totalDuration.TotalSeconds);

    TextHelper.TextGenerator($"\nTotal time of all activities: {totalTime}", ConsoleColor.Cyan);

    string favoriteActivity = newActivityService.GetFavoriteActivity();

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);

    TextHelper.WaitAndClearLine();
    TextHelper.TextGenerator("Welcome to statistics menu\n", ConsoleColor.Green);
    await ShowStatistics(userManagerService);
}
#endregion

/// <summary>
/// Displays the Manage Account menu and allows the user to select an option to modify their account information.
/// </summary>
/// <param name="userManagerService">The user manager service instance.</param>
/// <param name="database">The user database instance.</param>
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
            Console.Clear();
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            TextHelper.WaitAndClearLine();
            TextHelper.TextGenerator("Welcome to account management menu\n", ConsoleColor.Green);
            await ShowManageAccountAsync(userManagerService, database);
            break;
    }
}

#region Account Management Menu
/// <summary>
/// Allows the user to change their first name and updates the user information in the database.
/// </summary>
/// <param name="userManagerService">The user manager service instance.</param>
/// <param name="database">The user database instance.</param>
async Task AccountChangeFirstNameAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentFirstName = userManagerService.GetCurrentValidInputFromUser("first name", currentUser.FirstName, Console.ReadLine);

    string newFirstName = await userManagerService.GetNewValidInputFromUser("first name");

    currentUser.FirstName = newFirstName;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("\nFirst name updated successfully!", ConsoleColor.Green);

    TextHelper.WaitAndClear();
    TextHelper.TextGenerator("Welcome to account management menu\n", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

/// <summary>
/// Allows the user to change their last name and updates the user information in the database.
/// </summary>
/// <param name="userManagerService">The user manager service instance.</param>
/// <param name="database">The user database instance.</param>
async Task AccountChangeLastNameAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentLastName = userManagerService.GetCurrentValidInputFromUser("last name", currentUser.LastName, Console.ReadLine);

    string newLastName = await userManagerService.GetNewValidInputFromUser("last name");

    currentUser.LastName = newLastName;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("\nLast name updated successfully!", ConsoleColor.Green);

    TextHelper.WaitAndClear();
    TextHelper.TextGenerator("Welcome to account management menu\n", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

/// <summary>
/// Allows the user to change their age and updates the user information in the database.
/// </summary>
/// <param name="userManagerService">The user manager service instance.</param>
/// <param name="database">The user database instance.</param>
async Task AccountChangeAgeAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentAge = userManagerService.GetCurrentValidInputFromUser("age", currentUser.Age.ToString(), Console.ReadLine);

    string newAge = await userManagerService.GetNewValidInputFromUser("age");

    currentUser.Age = int.Parse(newAge);
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("\nAge updated successfully!", ConsoleColor.Green);

    TextHelper.WaitAndClear();
    TextHelper.TextGenerator("Welcome to account management menu\n", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

/// <summary>
/// Allows the user to change their username and updates the user information in the database.
/// </summary>
/// <param name="userManagerService">The user manager service instance.</param>
/// <param name="database">The user database instance.</param>
async Task AccountChangeUsernameAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentUsername = userManagerService.GetCurrentValidInputFromUser("username", currentUser.Username, Console.ReadLine);

    string newUsername = await userManagerService.GetNewValidInputFromUser("username");

    currentUser.Username = newUsername;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("\nUsername updated successfully!", ConsoleColor.Green);

    TextHelper.WaitAndClear();
    TextHelper.TextGenerator("Welcome to account management menu\n", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

/// <summary>
/// Allows the user to change their password and updates the user information in the database.
/// </summary>
/// <param name="userManagerService">The user manager service instance.</param>
/// <param name="database">The user database instance.</param>
async Task AccountChangePasswordAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;

    Task<string> currentPassword = userManagerService.GetCurrentValidInputFromUser("password", currentUser.Password, Console.ReadLine);

    string newPassword = await userManagerService.GetNewValidInputFromUser("password");

    currentUser.Password = newPassword;
    await database.UpdateUserAsync(currentUser);

    TextHelper.TextGenerator("\nPassword updated successfully!", ConsoleColor.Green);

    TextHelper.WaitAndClear();
    TextHelper.TextGenerator("Welcome to account management menu\n", ConsoleColor.Green);
    await ShowManageAccountAsync(userManagerService, database);
}

/// <summary>
/// Deactivates the current user's account after confirmation from the user.
/// </summary>
/// <param name="userManagerService">The user manager service instance.</param>
/// <param name="database">The user database instance.</param>
/// <returns>A Task representing the asynchronous operation.</returns>
async Task AccountDeactivationAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    User currentUser = userManagerService.CurrentUser;
    currentUser.IsActive = false;

    TextHelper.TextGenerator("\nDo you want to deactivate your account? (Y/N)", ConsoleColor.Cyan);

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
        TextHelper.TextGenerator("\nAccount deactivation cancelled.", ConsoleColor.Green);

        TextHelper.WaitAndClear();
        TextHelper.TextGenerator("Welcome to account management menu\n", ConsoleColor.Green);
        await ShowManageAccountAsync(userManagerService, database);
    }
    else if (userInput == "Y")
    {
        await database.UpdateUserAsync(currentUser);
        TextHelper.TextGenerator("\nYour account has been deactivated.", ConsoleColor.Red);

        TextHelper.WaitAndClear();
        TextHelper.TextGenerator("Welcome to Time Tracking APP", ConsoleColor.Green);
        await ShowLoginMenu(userManagerService);
    }
}
#endregion