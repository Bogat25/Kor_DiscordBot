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

            // Kiíratási sablon hívása
            var message = await SendGameStartMessage(channel, runningGameInfo, false); //ha true akkor updateli ha nem true akkor elküldi
            return message;
        }

        // Sablon függvény a kiíratáshoz
        public static async Task<ulong> SendGameStartMessage(ITextChannel channel, RunningGame runningGameInfo, bool trueIfUpdate, ulong? existingMessageId = null)
        {
            // Játékosok neveinek listája
            var playerNames = string.Join("\n", runningGameInfo.players.Select(p => p.name));

            if (trueIfUpdate && existingMessageId.HasValue)
            {
                // Ha a trueIfUpdate true és van meglévő üzenet ID, frissítse az üzenetet
                var message = await channel.GetMessageAsync(existingMessageId.Value) as IUserMessage;

                if (message != null)
                {
                    // Embed frissítése az új adatokkal
                    var embed = new EmbedBuilder()
                    {
                        Title = $"\"{runningGameInfo.players[0].name}\" frissítette a gamet.",
                        Description = $"Frissítve! Csatlakozz te is.\n\nEddig csatlakoztak:\n\n{playerNames}\n\nCsatlakozni a reakció megnyomásával tudsz.",
                        Color = Color.Orange // Frissítéshez más szín
                    }.Build();

                    // Meglévő üzenet módosítása
                    await message.ModifyAsync(msg =>
                    {
                        msg.Content = $"A játék frissítve lett! Csatlakozz {runningGameInfo.players[0].name} által.";
                        msg.Embed = embed;
                    });

                    // Visszaadja a meglévő üzenet azonosítóját
                    return message.Id;
                }
                else
                {
                    Console.WriteLine("Nem található az üzenet a megadott ID-val.");
                    return 0;
                }
            }
            else
            {
                // Ha nem frissítésről van szó, új üzenetet hoz létre
                await channel.SendMessageAsync($"A játék elkezdődött! Csatlakozz {runningGameInfo.players[0].name} által.");

                var embed = new EmbedBuilder()
                {
                    Title = $"\"{runningGameInfo.players[0].name}\" létrehozott egy gamet.",
                    Description = $"Csatlakozz te is.\n\nEddig csatlakoztak:\n\n{playerNames}\n\nCsatlakozni a reakció megnyomásával tudsz.",
                    Color = Color.Green
                }.Build();

                var message = await channel.SendMessageAsync(embed: embed);

                var thumbsUpEmoji = new Emoji("👍");
                await message.AddReactionAsync(thumbsUpEmoji);

                return message.Id;
            }
        }



        public static async Task ManageReactionsReactionAdded(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
        {
            if (reaction.UserId == 1296986541142577223) //így a bot nem reagál a saját reakcióira
            {
                return;
            }
            List<GameManagerClass.GameManager> gameManagerek = Program.activeGames;
            bool found = false;
            foreach (var gm  in gameManagerek)
            {
                if (gm.gameInfo.lastMessageID == reaction.MessageId)
                {
                    found = true;
                    if (gm.gameInfo.lastMessageType == "waitForJoin")
                    {
                        if (reaction.Emote.Name == "👍")
                        {
                            await gm.PlayerJoined(new Player(reaction.UserId.ToString(), reaction.User.ToString()));
                        }
                    }

                }
            }
        }
        public static async Task ManageReactionsReactionRemoved(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
        {
            if (reaction.UserId == 1296986541142577223) //így a bot nem reagál a saját reakcióira
            {
                return;
            }
            List<GameManagerClass.GameManager> gameManagerek = Program.activeGames;
            bool found = false;
            foreach (var gm  in gameManagerek)
            {
                if (gm.gameInfo.lastMessageID == reaction.MessageId)
                {
                    found = true;
                    if (gm.gameInfo.lastMessageType == "waitForJoin")
                    {
                        if (reaction.Emote.Name == "👍")
                        {
                            await gm.PlayerLeaved(new Player(reaction.UserId.ToString(), reaction.User.ToString()));
                        }
                    }
                }
            }
        }

        public static async Task UpdateLastMessage(RunningGame gameInfo)
        {
            if (gameInfo.lastMessageType == "waitForJoin")
            {

            }
        }
    }
}
