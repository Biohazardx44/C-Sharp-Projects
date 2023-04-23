using MovieStore.Domain.Entities;

Console.WriteLine("Welcome to the Movie Store!");

while (true)
{
    Console.WriteLine("\nPlease choose an option:");
    Console.WriteLine("1. Login as Employee");
    Console.WriteLine("2. Login as User");
    Console.WriteLine("3. Exit");

    int option;
    while (!int.TryParse(Console.ReadLine(), out option) || option < 1 || option > 3)
    {
        Console.WriteLine("Invalid option. Please try again.");
        Console.Clear();
    }

    switch (option)
    {
        case 1:
            // Login as Employee
            Console.WriteLine("Enter your username and password:");

            // TODO: Implement Employee login

            break;
        case 2:
            // Login as User
            Console.WriteLine("Enter your username and password:");

            // TODO: Implement User login

            break;
        case 3:
            // Exit the application
            Console.WriteLine("Thank you for using the Movie Store!");
            return;
    }
}

Member peter = new Member();