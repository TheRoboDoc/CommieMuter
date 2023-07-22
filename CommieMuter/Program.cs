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

            await Task.Delay(-1);
        }
    }
}