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
                Program.WriteInfo("Connecting to Twtich...");
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
                throw new NullReferenceException("Discord client not setup");
            }

            Program.WriteInfo($"\n{e.UserName} tipped {e.BitsUsed} bits");

            TimeSpan time = TimeSpan.FromSeconds(e.BitsUsed / 10);

            Discord.TimeSpan += time;

            Discord?.Mute(time);
        }

        public TwitchPubSub GetTwitchClient()
        {
            if (TwitchClient == null)
            {
                throw new NullReferenceException("Twitch client is not set");
            }

            return TwitchClient;
        }

        private static void OnResponse(object? sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
            {
                Program.WriteError($"Twitch client error: {e.Response.Error}");
            }
            else
            {
                Program.WriteInfo($"Listening to topic: {e.Topic}");
                Console.WriteLine();
            }
        }

        private static void ServiceError(object? sender, OnPubSubServiceErrorArgs e)
        {
            Program.WriteError($"Twitch client error: {e.Exception.Message}");
        }

        private static void ServiceClosed(object? sender, EventArgs e)
        {
            Program.WriteWarning("Twitch client closed");
        }

        private static void ServiceConnected(object? sender, EventArgs e)
        {
            if (TwitchClient == null)
            {
                throw new NullReferenceException("Twitch client is not net");
            }

            Program.WriteSuccess("Connected to Twitch");
            TwitchClient.SendTopics(Tokens.TwitchAccessToken);
        }
    }
}
