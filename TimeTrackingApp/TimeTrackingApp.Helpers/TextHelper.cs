namespace TimeTrackingApp.Helpers
{
    public static class TextHelper
    {
        /// <summary>
        /// Generates colored text on the console.
        /// </summary>
        /// <param name="text">The text to be generated.</param>
        /// <param name="color">The color of the generated text.</param>
        public static void TextGenerator(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Waits for the user to press a key, then clears the console.
        /// </summary>
        public static void WaitAndClear()
        {
            Console.ReadKey();
            Console.Clear();
        }

        /// <summary>
        /// Waits for the user to enter a line, and clears the console after the input has been received.
        /// </summary>
        public static void WaitAndClearLine()
        {
            Console.ReadLine();
            Console.Clear();
        }
    }
}