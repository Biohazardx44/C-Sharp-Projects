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
IHobbyDatabase hobbyDatabase = new HobbyDatabase();
IWorkingDatabase workingDatabase = new WorkingDatabase();
IUserManagerService userManagerService = new UserManagerService(database);
ITimerTrackerService timerService = new TimeTrackerService();
IReadingService<ReadingActivity> readingService = new ReadingService();

await StartAppAsync();

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

async Task ShowManageAccountAsync(IUserManagerService userManagerService, IUserDatabase database)
{
    TextHelper.TextGenerator(
    $"{ManageAccountOptions.CHANGE_PASSWORD}.Change password\n" +
    $"{ManageAccountOptions.CHANGE_FIRST_NAME}.Change First Name\n" +
    $"{ManageAccountOptions.CHANGE_LAST_NAME}.Change Last Name\n" +
    $"{ManageAccountOptions.DEACTIVATE_ACCOUNT}.Deactivate account\n" +
    $"{ManageAccountOptions.BACK_TO_MAIN_MENU}.Back to main menu"
    , ConsoleColor.Cyan);
    string accountOptionChosen = Console.ReadLine();

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
            await ShowReadingStatistics(userManagerService, readingDatabase);
            break;
        case StatisticsOptions.EXERCISING:
            await ShowExercisingStatistics(userManagerService, exercisingDatabase);
            break;
        case StatisticsOptions.WORKING:
            await ShowWorkingStatistics(userManagerService, workingDatabase);
            break;
        case StatisticsOptions.HOBBIES:
            await ShowHobbiesStatistics(userManagerService, hobbyDatabase);
            break;
        case StatisticsOptions.GLOBAL:
            await ShowGlobalStatistics(userManagerService);
            break;
        case StatisticsOptions.BACK_TO_MAIN_MENU:
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            await ShowStatistics(userManagerService);
            break;
    }
}

async Task ShowReadingStatistics(IUserManagerService userManagerService, IReadingDatabase readingDatabase)
{
    TextHelper.TextGenerator($"Total time: ", ConsoleColor.Cyan);
    TextHelper.TextGenerator($"Average of all activity records: ", ConsoleColor.Cyan);

    int totalPageCount = 0;
    List<ReadingActivity> activities = readingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    foreach (ReadingActivity activity in activities)
    {
        totalPageCount += activity.PageCount;
    }
    TextHelper.TextGenerator($"Total number of pages: {totalPageCount}", ConsoleColor.Cyan);

    Dictionary<ReadingType, int> typeCounts = new Dictionary<ReadingType, int>();
    foreach (ReadingActivity activity in activities)
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

async Task ShowExercisingStatistics(IUserManagerService userManagerService, IExercisingDatabase exercisingDatabase)
{
    TextHelper.TextGenerator($"Total time: ", ConsoleColor.Cyan);
    TextHelper.TextGenerator($"Average of all activity records: ", ConsoleColor.Cyan);

    List<ExercisingActivity> activities = exercisingDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);

    Dictionary<ExercisingType, int> typeCounts = new Dictionary<ExercisingType, int>();
    foreach (ExercisingActivity activity in activities)
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

async Task ShowWorkingStatistics(IUserManagerService userManagerService, IWorkingDatabase workingDatabase)
{
    TextHelper.TextGenerator($"Total time: ", ConsoleColor.Cyan);
    TextHelper.TextGenerator($"Average of all activity records: ", ConsoleColor.Cyan);
    TextHelper.TextGenerator($"Home VS Office working: ", ConsoleColor.Cyan);

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
    Console.ReadLine();
    await ShowStatistics(userManagerService);
}

async Task ShowHobbiesStatistics(IUserManagerService userManagerService, IHobbyDatabase hobbyDatabase)
{
    TextHelper.TextGenerator($"Total time: ", ConsoleColor.Cyan);

    List<Hobby> hobbies = hobbyDatabase.GetActivityByUserId(userManagerService.CurrentUser.Id);
    List<string> distinctNames = hobbies.Select(h => h.HobbyName).Distinct().ToList();
    if (distinctNames.Count > 0)
    {
        TextHelper.TextGenerator($"List of all hobbies: ", ConsoleColor.Cyan);
        foreach (string name in distinctNames)
        {
            TextHelper.TextGenerator($"- {name}", ConsoleColor.Yellow);
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

async Task ShowGlobalStatistics(IUserManagerService userManagerService)
{
    TextHelper.TextGenerator($"Total time of all activities: ", ConsoleColor.Cyan);
    TextHelper.TextGenerator($"User favorite activity: ", ConsoleColor.Cyan);

    TextHelper.TextGenerator($"\nPress ENTER to go back to the main menu", ConsoleColor.Cyan);
    Console.ReadLine();
    await ShowStatistics(userManagerService);
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

async Task ShowHobbyActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
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

async Task ShowWorkingActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
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

async Task ShowExercisingActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
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

async Task ShowReadingActivity(IUserManagerService userManagerService, ITimerTrackerService timerService)
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

async Task ShowLoginMenu(IUserManagerService userManagerService)
{
    Console.WriteLine($"\n{UserLogIn.LOG_IN}.Log In\n{UserLogIn.REGISTER_USER}.Register");
    string authChoice = Console.ReadLine();

    switch (authChoice)
    {
        case UserLogIn.LOG_IN:
            await ShowLogIn(userManagerService);
            break;
        case UserLogIn.REGISTER_USER:
            await ShowRegister(userManagerService);
            break;
        default:
            TextHelper.TextGenerator("Invalid Input! Please enter one of the given options...", ConsoleColor.Red);
            await ShowLoginMenu(userManagerService);
            break;
    }
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
                    Console.ReadKey();
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