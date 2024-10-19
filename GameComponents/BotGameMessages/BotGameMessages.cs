using Discord;
using Discord.WebSocket;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;
using System;
using System.Reflection;
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


                // MessageInfo feltöltése új üzenet adatokkal
                messageInfo.lastMessageType = "waitForJoin";
                messageInfo.lastMessageID = message.Id;
                messageInfo.lastMessageChanel = channel.Id;

                return messageInfo;
            }
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
    }
}
