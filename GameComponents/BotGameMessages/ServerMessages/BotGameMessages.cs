using Discord;
using Discord.WebSocket;
using Discord_Kor.GameComponents.Classes;
using Discord_Kor.GameComponents.GameManagerClass;
using DiscordKor;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.BotGameMessages.ServerMessages;
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
        var playerNames = string.Join("\n", runningGameInfo.players.Select(p => p.Name));
        var messageInfo = new MessageInfo();

        if (trueIfUpdate && existingMessageId.HasValue)
        {
            var message = await channel.GetMessageAsync(existingMessageId.Value) as IUserMessage;

            if (message != null)
            {
                var embed = new EmbedBuilder()
                {
                    Title = $"\"{runningGameInfo.players[0].Name}\" frissítette a gamet.",
                    Description = $"Frissítve! Csatlakozz te is.\n\nEddig csatlakoztak:\n\n{playerNames}\n\nCsatlakozni a reakció megnyomásával tudsz.",
                    Color = Color.Orange
                }.Build();
                await message.ModifyAsync(msg =>
                {
                    msg.Embed = embed;
                });

                messageInfo.currentGameState = "waitForJoin";
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
                Title = $"\"{runningGameInfo.players[0].Name}\" létrehozott egy gamet.",
                Description = $"Csatlakozz te is.\n\nEddig csatlakoztak:\n\n{playerNames}\n\nCsatlakozni a reakció megnyomásával tudsz.",
                Color = Color.Green
            }.Build();

            var message = await channel.SendMessageAsync(embed: embed);

            await message.AddReactionAsync(ReactionTypes.greenCheckEmoji);
            await message.AddReactionAsync(ReactionTypes.startEmoji);
            await message.AddReactionAsync(ReactionTypes.xEmoji);


            // MessageInfo feltöltése új üzenet adatokkal
            messageInfo.currentGameState = "waitForJoin";
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
        if (gameInfo.message.currentGameState == "waitForJoin")
        {
            var guild = Program.Client.GetGuild(ulong.Parse(gameInfo.gameServerId));
            var channel = guild.GetTextChannel(ulong.Parse(gameInfo.gameChannelId));

            SendGameStartMessage(channel, gameInfo, true, gameInfo.message.lastMessageID);
        }
    }
    public static async Task GameStartedSuccesfully(RunningGame gameInfo)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("Játék sikeresen elindult!")
            .WithDescription($"A játékmester: **{gameInfo.players.First().Name}**")
            .WithColor(Color.Green);

        // Játékosok listájának összeállítása
        StringBuilder playersList = new StringBuilder();
        foreach (var player in gameInfo.players)
        {
            playersList.AppendLine(player.Name);
        }

        embedBuilder.AddField("Játékosok:", playersList.ToString(), false);

        // A GameSettings osztály ToString() metódusának használata a beállításokhoz
        embedBuilder.AddField("Játék Beállítások", gameInfo.settings.ToString(), false);

        // Megkeressük a Discord guildet és a csatornát
        var guild = Program.Client.GetGuild(ulong.Parse(gameInfo.gameServerId));
        var channel = guild.GetTextChannel(ulong.Parse(gameInfo.gameChannelId));

        // Üzenet küldése
        await channel.SendMessageAsync(embed: embedBuilder.Build());
    }

    public static async Task SendPlayersDataSheet(RunningGame runningGame)
    {
        // Megkeressük a Discord guildet és a csatornát
        var guild = Program.Client.GetGuild(ulong.Parse(runningGame.gameServerId));
        var channel = guild.GetTextChannel(ulong.Parse(runningGame.gameChannelId));

        // Végigmegyünk az összes játékoson és elküldjük a ToString() alapján az adataikat
        foreach (var player in runningGame.players)
        {
            // Embed létrehozása minden egyes játékos számára
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Játékos Adatlap - {player.Name}")
                .WithDescription(player.ToString())
                .WithColor(Color.Blue);

            // Üzenet küldése a csatornára
            await channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }


    public static async Task SendCurrentGameState(RunningGame runningGame)
    {
        var embedBuilder = new EmbedBuilder()
            .WithTitle("🛑 Aktuális Játék Állapot")
            .WithColor(Color.DarkBlue); // Sötétkék szín a kiemeléshez

        // Élő játékosok listája
        StringBuilder livePlayersList = new StringBuilder();
        StringBuilder eliminatedPlayersList = new StringBuilder();

        int playerIndex = 1; // Játékosok sorszáma

        // Végigmegyünk a játékosokon és összeállítjuk az élő és kiesett játékosok listáját
        foreach (var player in runningGame.players)
        {
            if (player.IsAlive)
            {
                // Élő játékosok félkövér stílusban és sorszámozva, nincs extra sor
                livePlayersList.Append($"**{playerIndex}. {player.Name}**\n");
            }
            else
            {
                // Kiesett játékosok áthúzott stílusban és sorszámozva, nincs extra sor
                eliminatedPlayersList.Append($"~~{playerIndex}. {player.Name}~~\n");
            }
            playerIndex++;
        }

        // Élő játékosok hozzáadása az embedhez
        embedBuilder.AddField("✅ Élő Játékosok:", livePlayersList.Length > 0 ? livePlayersList.ToString() : "Nincsenek élő játékosok", false);

        // Kiesett játékosok hozzáadása az embedhez
        embedBuilder.AddField("💀 Kiesett Játékosok:", eliminatedPlayersList.Length > 0 ? eliminatedPlayersList.ToString() : "Nincsenek kiesett játékosok", false);

        // Megkeressük a Discord guildet és a csatornát
        var guild = Program.Client.GetGuild(ulong.Parse(runningGame.gameServerId));
        var channel = guild.GetTextChannel(ulong.Parse(runningGame.gameChannelId));

        await channel.SendMessageAsync(embed: embedBuilder.Build());
    }

    public async Task SendOutCurrentGameInfos(RunningGame gameInfo)
    {
        var guild = Program.Client.GetGuild(ulong.Parse(gameInfo.gameServerId));
        var channel = guild.GetTextChannel(ulong.Parse(gameInfo.gameChannelId));

        if (channel != null)
        {
            // Gyűjtsd össze az életben lévő játékosokat
            var alivePlayers = gameInfo.players.Where(p => p.IsAlive).ToList();

            if (alivePlayers.Count > 0)
            {
                string message = "**Játékosok, akiket ki lehet szavazni:**\n\n";

                // Add hozzá minden életben lévő játékos adatait a message-hez
                foreach (var player in alivePlayers)
                {
                    message += $"{player.Name}\n";  // Csak a nevüket írjuk ki, de akár több információt is hozzáadhatsz
                }

                // Üzenet küldése a csatornára
                await channel.SendMessageAsync(message);
            }
            else
            {
                // Ha nincs életben lévő játékos
                await channel.SendMessageAsync("Nincs több életben lévő játékos.");
            }
        }
    }

    public static SendEvenVotes(RunningGame gameInfo, VoteResult voteResult)
    {
        var guild = Program.Client.GetGuild(ulong.Parse(gameInfo.gameServerId));
        var channel = guild.GetTextChannel(ulong.Parse(gameInfo.gameChannelId));

        var embedBuilder = new EmbedBuilder()
            .WithTitle("A szavazatok eredménye megegyezik!")
            .WithDescription("Kérlek szavazz újra!")
            .WithColor(Color.Red);
    }

}


