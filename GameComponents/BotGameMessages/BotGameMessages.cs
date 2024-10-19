using Discord;
using Discord.WebSocket;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.BotGameMessages
{
    public class BotMessages
    {

        public BotMessages()
        {
        }

        public static async Task<ulong> GameStartedAskToJoin(RunningGame runningGameInfo)
        {
            // A szerver (guild) lekérdezése az ID alapján a Program.Client segítségével
            var guild = Program.Client.GetGuild(ulong.Parse(runningGameInfo.gameServerId));
            if (guild == null)
            {
                Console.WriteLine("Nem található a szerver.");
                return 0;
            }

            // A csatorna lekérdezése az ID alapján
            var channel = guild.GetTextChannel(ulong.Parse(runningGameInfo.gameChannelId));
            if (channel == null)
            {
                Console.WriteLine("Nem található a csatorna.");
                return 0;
            }

            // Üzenet elküldése a csatornára
            await channel.SendMessageAsync($"A játék elkezdődött! Csatlakozz {runningGameInfo.players[0].name} által.");

            var embed = new EmbedBuilder()
            {
                Title = $"\"{runningGameInfo.players[0].name}\" létrehozott egy gamet.",
                Description = "Csatlakozz te is.\n\nEddig csatlakoztak:\n\n(később feltöltendő list)\n\nCsatlakozni a reakció megnyomásával tudsz.",
                Color = Color.Green
            }.Build();

            // Üzenet elküldése a megadott csatornára
            var message = await channel.SendMessageAsync(embed: embed);

            // Reakció hozzáadása (például 👍 emoji)
            var thumbsUpEmoji = new Emoji("👍");
            await message.AddReactionAsync(thumbsUpEmoji);
            return message.Id;
        }

        public static async Task ReactionAdded(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
        {
            Console.WriteLine("test");
        }


        public static async Task ManageReactions(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
        {
            Console.WriteLine("test");
        }
    }
}
