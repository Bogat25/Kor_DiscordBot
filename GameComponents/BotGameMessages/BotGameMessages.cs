using Discord;
using Discord.WebSocket;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.BotGameMessages
{
    public class BotMessages
    {

        public BotMessages()
        {
        }

        public static async Task<MessageInfo> GameStartedAskToJoin(RunningGame runningGameInfo)
        {
            var guild = Program.Client.GetGuild(ulong.Parse(runningGameInfo.gameServerId));
            if (guild == null)
            {
                Console.WriteLine("Nem található a szerver.");
                return null;
            }
            var channel = guild.GetTextChannel(ulong.Parse(runningGameInfo.gameChannelId));
            if (channel == null)
            {
                Console.WriteLine("Nem található a csatorna.");
                return null;
            }
            var messageInfo = await SendGameStartMessage(channel, runningGameInfo, false); // ha true, akkor updateli, ha nem, akkor elküldi
            return messageInfo;
        }
        public static async Task<MessageInfo> SendGameStartMessage(ITextChannel channel, RunningGame runningGameInfo, bool trueIfUpdate, ulong? existingMessageId = null)
        {
            // Játékosok neveinek listája
            var playerNames = string.Join("\n", runningGameInfo.players.Select(p => p.name));
            var messageInfo = new MessageInfo();

            if (trueIfUpdate && existingMessageId.HasValue)
            {
                var message = await channel.GetMessageAsync(existingMessageId.Value) as IUserMessage;

                if (message != null)
                {
                    var embed = new EmbedBuilder()
                    {
                        Title = $"\"{runningGameInfo.players[0].name}\" frissítette a gamet.",
                        Description = $"Frissítve! Csatlakozz te is.\n\nEddig csatlakoztak:\n\n{playerNames}\n\nCsatlakozni a reakció megnyomásával tudsz.",
                        Color = Color.Orange 
                    }.Build();
                    await message.ModifyAsync(msg =>
                    {
                        msg.Embed = embed;
                    });

                    messageInfo.lastMessageType = "waitForJoin";
                    messageInfo.lastMessageID = message.Id;
                    messageInfo.lastMessageChanel = channel.Id;

                    return messageInfo;
                }
                else
                {
                    Console.WriteLine("Nem található az üzenet a megadott ID-val.");
                    return null;
                }
            }
            else
            {
                var embed = new EmbedBuilder()
                {
                    Title = $"\"{runningGameInfo.players[0].name}\" létrehozott egy gamet.",
                    Description = $"Csatlakozz te is.\n\nEddig csatlakoztak:\n\n{playerNames}\n\nCsatlakozni a reakció megnyomásával tudsz.",
                    Color = Color.Green
                }.Build();

                var message = await channel.SendMessageAsync(embed: embed);

                await message.AddReactionAsync(ReactionTypes.greenCheckEmoji);
                await message.AddReactionAsync(ReactionTypes.startEmoji);
                await message.AddReactionAsync(ReactionTypes.xEmoji);


                // MessageInfo feltöltése új üzenet adatokkal
                messageInfo.lastMessageType = "waitForJoin";
                messageInfo.lastMessageID = message.Id;
                messageInfo.lastMessageChanel = channel.Id;

                return messageInfo;
            }
        }
        public static async Task RemoveMessage(RunningGame gameInfo)
        {
            // Szerver (guild) megszerzése
            var guild = Program.Client.GetGuild(ulong.Parse(gameInfo.gameServerId));
            if (guild == null)
            {
                Console.WriteLine("No server found for the message to delete");
                return;
            }

            // Csatorna megszerzése
            var channel = guild.GetTextChannel(ulong.Parse(gameInfo.gameChannelId));
            if (channel == null)
            {
                Console.WriteLine("No channel found for the message to delete");
                return;
            }

            // Üzenet megszerzése és törlése
            var message = await channel.GetMessageAsync(gameInfo.message.lastMessageID) as IUserMessage;
            if (message == null)
            {
                Console.WriteLine("No message found to delete");
                return;
            }

            await message.DeleteAsync(); // Üzenet törlése
        }



        public static async Task UpdateLastMessage(RunningGame gameInfo)
        {
            if (gameInfo.message.lastMessageType == "waitForJoin")
            {
                var guild = Program.Client.GetGuild(ulong.Parse(gameInfo.gameServerId));
                var channel = guild.GetTextChannel(ulong.Parse(gameInfo.gameChannelId));

                SendGameStartMessage(channel, gameInfo, true, gameInfo.message.lastMessageID);
            }
        }
        public static async Task GameStartedSuccesfully(RunningGame gameInfo)
        {
            // Embed létrehozása, játék indítási információk megjelenítése
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Játék sikeresen elindult!")
                .WithDescription($"A játékmester: **{gameInfo.players.First().name}**")
                .WithColor(Color.Green);

            // Játékosok listájának összeállítása
            StringBuilder playersList = new StringBuilder();
            foreach (var player in gameInfo.players)
            {
                playersList.AppendLine(player.name);
            }

            embedBuilder.AddField("Játékosok:", playersList.ToString(), false);

            // Játék beállítások (settings) hozzáadása
            embedBuilder.AddField("Játék Beállítások", $"Max Játékosok: {gameInfo.settings.MaxPlayers}\n" +
                                                        $"Min Játékosok: {gameInfo.settings.MinPlayers}\n" +
                                                        $"Szavazási idő (másodperc): {gameInfo.settings.VoteTime}\n" +
                                                        $"Vita időtartam (másodperc): {gameInfo.settings.DiscussionTime}", false);

            // Küldjük az üzenetet a csatornába
            var guild = Program.Client.GetGuild(ulong.Parse(gameInfo.gameServerId));
            var channel = guild.GetTextChannel(ulong.Parse(gameInfo.gameChannelId));
            await channel.SendMessageAsync(embed: embedBuilder.Build());
        }


    }

}
