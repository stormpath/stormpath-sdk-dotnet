using System;
using System.Threading;

namespace Stormpath.Demo
{
    public static class Helpers
    {
        public static bool SpacebarToContinue(CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
                return false;

            Console.Write($"{Strings.NL}Press spacebar to continue");
            var key = Console.ReadKey(true);

            if (cancelToken.IsCancellationRequested)
                return false;

            ClearCurrentLine(Console.CursorTop);
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            return (key.KeyChar == ' ');
        }

        public static string TrimWithEllipse(string input, int maxLength)
        {
            if (input.Length <= maxLength)
                return input;

            return input.Substring(0, maxLength - 3) + "...";
        }

        public static void ClearCurrentLine(int line)
        {
            Console.SetCursorPosition(0, line);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }
            Console.SetCursorPosition(0, line);
        }
    }
}
