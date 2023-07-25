using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using TwitchLib.PubSub.Events;

namespace CommieMuter
{
    public class Program
    {
        private static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public static Discord? Discord { get; private set; }

        public static Twitch? Twitch { get; private set; }

        private static async Task MainAsync()
        {
            Discord = new Discord();

            Console.WriteLine();

            Twitch = new Twitch(Discord);

            if (CheckDebug())
            {
                do
                {
                    string? input = Console.ReadLine();

                    if (input == null) { continue; }

                    int? result = GetBitAmount(input);

                    if (result.HasValue)
                    {
                        OnBitsReceivedV2Args bitsReceived = new();

                        // This is stupid
                        PropertyInfo? bitsUsedProperty = bitsReceived.GetType().GetProperty("BitsUsed");
                        PropertyInfo? userNameProperty = bitsReceived.GetType().GetProperty("UserName");

                        bitsUsedProperty.SetValue(bitsReceived, result.Value);
                        userNameProperty.SetValue(bitsReceived, "test");

                        Twitch.OnBitsReceivedV2(null, bitsReceived);
                    }
                }while (true);
            }

            await Task.Delay(-1);
        }

        private static int? GetBitAmount(string input)
        {
            string pattern = @"^bit (\d+)";

            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return null;
        }

        private static bool CheckDebug()
        {
            if (Debugger.IsAttached)
            {
                return true;
            }

            return false;
        }

        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}