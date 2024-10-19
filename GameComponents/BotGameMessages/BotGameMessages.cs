using Discord;
using Discord.WebSocket;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;
using System;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.BotGameMessages
{
    public class BotMessages
    {
        public static async Task GameStartedAskToJoin(RunningGame runningGameInfo)
        {
            // A szerver (guild) lekérdezése az ID alapján a Program.Client segítségével
            var guild = Program.Client.GetGuild(ulong.Parse(runningGameInfo.gameServerId));
            if (guild == null)
            {
                Console.WriteLine("Nem található a szerver.");
                return;
            }

            // A csatorna lekérdezése az ID alapján
            var channel = guild.GetTextChannel(ulong.Parse(runningGameInfo.gameChannelId));
            if (channel == null)
            {
                Console.WriteLine("Nem található a csatorna.");
                return;
            }
            // Üzenet elküldése a csatornára
            await channel.SendMessageAsync($"A játék elkezdődött! Csatlakozz {runningGameInfo.gameMasterUserName} által.");


            var embed = new EmbedBuilder()
            {
                Title = $"\"{runningGameInfo.gameMasterUserName}\" létrehozott egy gamet.",
                Description = "Csatlakozz te is.\n\nEddig csatlakoztak:\n\n(később feltöltendő list)\n\nCsatlakozni a reakció megnyomásával tudsz.",
                Color = Color.Green
            }.Build();

            // Üzenet elküldése a megadott csatornára
            var message = await channel.SendMessageAsync(embed: embed);

            // Reakció hozzáadása (például 👍 emoji)
            var thumbsUpEmoji = new Emoji("👍");
            await message.AddReactionAsync(thumbsUpEmoji);


        }
    }
}
