using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CommieMuter
{
    public class Discord
    {
        public const ulong COMMISSARID = 311026477057703936;

        private readonly TimeSpan MutePeriod = TimeSpan.FromMinutes(1);

        public static DiscordClient? Client { get; private set; }

        public ulong GuildID { get; private set; }

        private readonly DiscordGuild Guild;

        public Discord()
        {
            Console.WriteLine("Setting up Discord client");

            DiscordConfiguration configuration = new()
            {
                Token = Tokens.DiscordToken,
                TokenType = TokenType.Bot,

                Intents =
                DiscordIntents.GuildVoiceStates |
                DiscordIntents.Guilds,

                MinimumLogLevel = LogLevel.None
            };

            Client = new DiscordClient(configuration);

            Console.WriteLine("Connecting...");

            Client.ConnectAsync().GetAwaiter().GetResult();

            Console.WriteLine("Connected");
            Console.WriteLine();

            GuildID = CheckGuildIDValidity(AskForID()).Result;

            Guild = Client.GetGuildAsync(GuildID).Result;
        }

        public async void MuteCommissar()
        {
            Console.WriteLine("Muting Commissar");

            bool commissarMute = false;

            DiscordChannel[] channels = Guild.GetChannelsAsync().Result.ToArray();

            foreach (DiscordChannel channel in channels.Where(channel => channel.Type == ChannelType.Voice))
            {
                foreach (DiscordUser user in channel.Users)
                {
                    if (user.Id == COMMISSARID)
                    {
                        DiscordMember commieMember = await Guild.GetMemberAsync(user.Id);

                        await commieMember.SetMuteAsync(true);

                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(MutePeriod);

                            await UnMuteCommissar(commieMember);
                        });

                        commissarMute = true;

                        break;
                    }
                }
            }

            if (!commissarMute)
            {
                Console.WriteLine("Couldn't mute Commissar, is he in a voice channel?");
            }
        }

        private static async Task UnMuteCommissar(DiscordMember commieMember)
        {
            Console.WriteLine("Unmuting Commissar");

            await commieMember.SetMuteAsync(false);
        }

        private static ulong AskForID()
        {
            string pattern = @"^\d{18}$";

            Regex regex = new(pattern);

            do
            {
                Console.Write("Give Discord server ID: ");
                string? id = Console.ReadLine();

                if (!string.IsNullOrEmpty(id) && regex.IsMatch(id) && ulong.TryParse(id, out ulong ID))
                {
                    return ID;
                }
                else
                {
                    Console.WriteLine("Invalid server ID. Needs to be 18 characters long and contain only numbers\n");
                }
            } while (true);
        }

        private static async Task<ulong> CheckGuildIDValidity(ulong guildID)
        {
            DiscordGuild guild;

            if (Client == null)
            {
                throw new NullReferenceException("Discord client was not set");
            }

            do
            {
                try
                {
                    guild = await Client.GetGuildAsync(guildID);
                    break;
                }
                catch
                {
                    Console.WriteLine("Invalid server ID. Bot isn't on the server with such ID\n");
                    guildID = AskForID();
                }
            }
            while (true);

            return guild.Id;
        }
    }
}
