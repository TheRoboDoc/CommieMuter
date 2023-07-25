using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace CommieMuter
{
    public class Discord
    {
        public const ulong COMMISSARID = 311026477057703936;
        public const ulong FUBIID = 590246495073206547;
        public const ulong VALHALLAID = 712862603147477002;

        public static DiscordClient? Client { get; private set; }

        public ulong GuildID { get; private set; }

        private readonly DiscordGuild Guild;

        public TimeSpan TimeSpan;

        public Discord()
        {
            Program.WriteInfo("Setting up Discord client");

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

            Program.WriteInfo("Connecting...");

            Client.ConnectAsync().GetAwaiter().GetResult();

            Program.WriteSuccess("Connected");
            Console.WriteLine();

            GuildID = VALHALLAID;

            Guild = Client.GetGuildAsync(GuildID).Result;

            TimeSpan = TimeSpan.FromSeconds(0);
        }

        public async void Mute(TimeSpan time)
        {
            bool commissarMute = false;
            bool fubiMute = false;

            DiscordChannel[] channels = Guild.GetChannelsAsync().Result.ToArray();

            foreach (DiscordChannel channel in channels.Where(channel => channel.Type == ChannelType.Voice))
            {
                foreach (DiscordUser user in channel.Users)
                {
                    if (user.Id == COMMISSARID || user.Id == FUBIID)
                    {       
                        DiscordMember member = await Guild.GetMemberAsync(user.Id);

                        Program.WriteInfo($"Muting {member.DisplayName}");

                        await member.SetMuteAsync(true);

                        _ = Task.Run(async () =>
                        {
                            do
                            {
                                await Task.Delay(TimeSpan);

                                TimeSpan -= time;
                            } while (TimeSpan > TimeSpan.FromSeconds(0));

                            await UnMute(member);
                        });

                        if (user.Id == COMMISSARID)
                        {
                            commissarMute = true;
                        }
                        else if (user.Id == FUBIID)
                        {
                            fubiMute = true;
                        }
                    }

                    if (commissarMute && fubiMute) { break; }
                }
            }

            if (!commissarMute)
            {
                Program.WriteWarning("Couldn't mute Commissar, is he in the voice channel?");
            }

            if (!fubiMute)
            {
                Program.WriteWarning("Couldn't mute Fubi, is she in the voice channel?");
            }
        }

        private static async Task UnMute(DiscordMember member)
        {
            Program.WriteInfo($"Unmuting {member.DisplayName}");

            await member.SetMuteAsync(false);
        }
    }
}
