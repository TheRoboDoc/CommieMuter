using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TwitchLib.PubSub.Models.Responses;
using TwitchLib.PubSub;

namespace CommieMuter
{
    public class Twitch
    {
        private static TwitchPubSub TwitchClient;

        private const string CHANNELID = "126663894"; //valkyrie_illustration Twitch channel ID

        public Twitch()
        {
            TwitchClient = new TwitchPubSub();

            TwitchClient.OnRewardRedeemed += OnRewardRedeemed;
            TwitchClient.OnListenResponse += OnResponse;

            TwitchClient.OnPubSubServiceConnected += ServiceConnected;
            TwitchClient.OnPubSubServiceClosed += OnPubSubServiceClosed;
            TwitchClient.OnPubSubServiceError += OnPubSubServiceError;

            TwitchClient.ListenToRewards(CHANNELID);

            TwitchClient.Connect();
        }

        public TwitchPubSub GetTwitchClient()
        {
            return TwitchClient;
        }

        private static void OnRewardRedeemed(object? sender, TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
        {
            Console.WriteLine("Redeem");
            Console.WriteLine(e.RewardTitle);
        }

        private static void OnPubSubServiceError(object? sender, TwitchLib.PubSub.Events.OnPubSubServiceErrorArgs e)
        {
            Console.WriteLine($"Twitch client error: {e.Exception.Message}");
        }

        private static void OnPubSubServiceClosed(object? sender, EventArgs e)
        {
            Console.WriteLine("Twitch client closed");
        }

        private static void OnResponse(object? sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
        {
            if (!e.Successful)
            {
                Console.WriteLine($"Twitch client error: {e.Response.Error}");
            }
            else
            {
                Console.WriteLine("Listen successful");
                Console.WriteLine($"Listening to topic: {e.Topic}");
            }
        }

        private static void ServiceConnected(object? sender, EventArgs e)
        {
            Console.WriteLine("Connected to Twitch");
            TwitchClient.SendTopics();
        }
    }
}
