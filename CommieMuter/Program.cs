namespace CommieMuter
{
    public class Program
    {
        private static void Main()
        {
            MainAsyn().GetAwaiter().GetResult();
        }

        

        private static async Task MainAsyn()
        {
            Twitch twitch = new();

            await Task.Delay(-1);
        }

        
    }
}