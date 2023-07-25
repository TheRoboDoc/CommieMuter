using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace CommieMuter
{
    public class Discord
    {
        public const ulong COMMISSARID = 311026477057703936;    //Commissar's user ID 311026477057703936
        public const ulong FUBIID = 590246495073206547;         //Robo's user ID 274970913370537985 (For testing)
        public const ulong VALHALLAID = 712862603147477002;

        public static DiscordClient? Client { get; private set; }

        public ulong GuildID { get; private set; }

        private readonly DiscordGuild Guild;

        public List<TimeSpan> TimeSpans = new();

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

            Task.Run(Mute);
        }

        public async Task Mute()
        {
            while (true)
            {
                if (!TimeSpans.Any())
                {
                    continue;
                }

                bool commissarMute = false;
                bool fubiMute = false;

                DiscordChannel[] channels = Guild.GetChannelsAsync().Result.ToArray();

                DiscordMember[] members = Array.Empty<DiscordMember>();

                foreach (DiscordChannel channel in channels.Where(channel => channel.Type == ChannelType.Voice))
                {
                    foreach (DiscordUser user in channel.Users)
                    {
                        if (user.Id == COMMISSARID || user.Id == FUBIID)
                        {
                            DiscordMember member = await Guild.GetMemberAsync(user.Id);

                            Program.WriteInfo($"Muting {member.DisplayName}");

                            await member.SetMuteAsync(true);

                            members = members.Append(member).ToArray();

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

                await Task.Delay(TimeSpans.First());

                TimeSpans.RemoveAt(0);

                if (TimeSpans.Any()) { continue; }

                foreach(DiscordMember member in  members)
                {
                    await UnMute(member);
                }
            }
        }

        private static async Task UnMute(DiscordMember member)
        {
            Program.WriteInfo($"Unmuting {member.DisplayName}");

            await member.SetMuteAsync(false);
        }
    }
}
