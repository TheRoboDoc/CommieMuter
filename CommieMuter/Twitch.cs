using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace CommieMuter
{
    public class Twitch
    {
        private static TwitchPubSub? TwitchClient;

        private static Discord? Discord;

        private const string CHANNELID = "126663894"; //valkyrie_illustration Twitch channel ID 126663894
                                                      //the_robodoc Twitch channel ID 430864937
        public Twitch(Discord discord)
        {
            TwitchClient = new TwitchPubSub();

            TwitchClient.OnBitsReceivedV2 += OnBitsReceivedV2;
            TwitchClient.OnListenResponse += OnResponse;

            TwitchClient.OnPubSubServiceConnected += ServiceConnected;
            TwitchClient.OnPubSubServiceClosed += ServiceClosed;
            TwitchClient.OnPubSubServiceError += ServiceError;

            TwitchClient.ListenToBitsEventsV2(CHANNELID);

            try
            {
                Console.WriteLine("Connecting to Twtich...");
                TwitchClient.Connect();
            }
            catch
            {
                TwitchClient.Disconnect();
            }

            Discord = discord;
        }

        private void OnBitsReceivedV2(object? sender, OnBitsReceivedV2Args e)
        {
            if (Discord == null)
            {
                throw new NullReferenceException("Discord clinet not setup");
            }

            Console.WriteLine($"\n{e.UserName} tipped {e.BitsUsed} bits");

            TimeSpan time = TimeSpan.FromSeconds(e.BitsUsed / 10);

            Discord.TimeSpan += time;

            Discord?.MuteCommissar(time);
        }

        public TwitchPubSub GetTwitchClient()
        {
            if (TwitchClient == null)
            {
                throw new NullReferenceException("Twitch client is not net");
            }

            return TwitchClient;
        }

        private static void OnResponse(object? sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
            {
                Console.WriteLine($"Twitch client error: {e.Response.Error}");
            }
            else
            {
                Console.WriteLine($"Listening to topic: {e.Topic}");
                Console.WriteLine();
            }
        }

        private static void ServiceError(object? sender, OnPubSubServiceErrorArgs e)
        {
            Console.WriteLine($"Twitch client error: {e.Exception.Message}");
        }

        private static void ServiceClosed(object? sender, EventArgs e)
        {
            Console.WriteLine("Twitch client closed");
        }

        private static void ServiceConnected(object? sender, EventArgs e)
        {
            if (TwitchClient == null)
            {
                throw new NullReferenceException("Twitch client is not net");
            }

            Console.WriteLine("Connected to Twitch");
            TwitchClient.SendTopics(Tokens.TwitchAccessToken);
        }
    }
}
