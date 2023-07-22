using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace CommieMuter
{
    public class Twitch
    {
        private static TwitchPubSub? TwitchClient;

        private static Discord? Discord;

        private const string CHANNELID = "126663894"; //valkyrie_illustration Twitch channel ID

        public Twitch(Discord discord)
        {
            TwitchClient = new TwitchPubSub();

            TwitchClient.OnRewardRedeemed += OnRewardRedeemed;
            TwitchClient.OnListenResponse += OnResponse;

            TwitchClient.OnPubSubServiceConnected += ServiceConnected;
            TwitchClient.OnPubSubServiceClosed += ServiceClosed;
            TwitchClient.OnPubSubServiceError += ServiceError;

            TwitchClient.ListenToRewards(CHANNELID);

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

        public TwitchPubSub GetTwitchClient()
        {
            if (TwitchClient == null)
            {
                throw new NullReferenceException("Twitch client is not net");
            }

            return TwitchClient;
        }

        private static void OnRewardRedeemed(object? sender, OnRewardRedeemedArgs rewardRedeemArgs)
        {
            string userName = rewardRedeemArgs.DisplayName;
            string redeemTitle = rewardRedeemArgs.RewardTitle;

            Console.WriteLine($"{userName} redeemed {redeemTitle}");

            if (Discord == null)
            {
                throw new NullReferenceException("Discord client is not set");
            }

            if (redeemTitle.ToLower() == "mute commissar")
            {
                Discord.MuteCommissar();
            }
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
            TwitchClient.SendTopics();
        }
    }
}
