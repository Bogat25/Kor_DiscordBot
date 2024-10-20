using Discord;
using Discord.WebSocket;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.BotGameMessages.PersonMessages
{
    public static class VoteSystem
    {

        public static async Task AskPeopleToVote(RunningGame gameInfo)
        {
            foreach (var player in gameInfo.players)
            {
                if (player.IsAlive && !player.AlreadyVote)
                {
                    // Megkeressük a játékos Discord felhasználóját a Discord ID alapján
                    var user = Program.Client.GetUser(ulong.Parse(player.Id));

                    if (user != null)
                    {
                        // Emoji lista készítése
                        var emojiList = new List<Emoji>
                {
                    ReactionTypes.oneEmoji, ReactionTypes.twoEmoji, ReactionTypes.threeEmoji, ReactionTypes.fourEmoji,
                    ReactionTypes.fiveEmoji, ReactionTypes.sixEmoji, ReactionTypes.sevenEmoji, ReactionTypes.eightEmoji,
                    ReactionTypes.nineEmoji, ReactionTypes.tenEmoji
                };

                        // Egyetlen üzenetbe összefoglalva a szavazási lehetőségek
                        var messageContent = new StringBuilder();
                        messageContent.AppendLine("Szavazz egy élő játékosra az alábbi emojik segítségével:");

                        int playerIndex = 0;
                        var livePlayers = new Dictionary<Emoji, Player>();

                        foreach (var targetPlayer in gameInfo.players)
                        {
                            if (targetPlayer.IsAlive && playerIndex < emojiList.Count)
                            {
                                // Hozzáadjuk a játékos nevét az emoji mellé az üzenethez
                                messageContent.AppendLine($"{emojiList[playerIndex]} {targetPlayer.Name}");
                                livePlayers.Add(emojiList[playerIndex], targetPlayer); // Emoji és játékos társítása
                                playerIndex++;
                            }
                        }

                        // Privát üzenet küldése a játékosnak a szavazási lehetőségekkel
                        var voteMessage = await user.SendMessageAsync(messageContent.ToString());

                        // Reakciók hozzáadása az üzenethez
                        for (int i = 0; i < playerIndex; i++)
                        {
                            await voteMessage.AddReactionAsync(emojiList[i]);
                        }

                        // Mentjük az üzenet ID-ját és a szavazó játékos ID-ját a voteAsks listába
                        gameInfo.voteAsks.Add(new VoteAsksInRound
                        {
                            VoterId = player.Id,
                            MessageId = voteMessage.Id.ToString()
                        });
                    }
                }
            }
        }


        // Reakciók várása (nem blokkoló módon)
        //public static async Task<Emoji> WaitForReactionAsync(IUserMessage voteMessage, Player votingPlayer)
        //{
        //    var tcs = new TaskCompletionSource<Emoji>();

        //    // Eseménykezelő, ami figyeli a reakciókat
        //    Program.Client.ReactionAdded += async (cache, channel, reaction) =>
        //    {
        //        if (reaction.UserId == ulong.Parse(votingPlayer.Id) && reaction.MessageId == voteMessage.Id)
        //        {
        //            var emoji = reaction.Emote as Emoji;
        //            if (emoji != null)
        //            {
        //                tcs.SetResult(emoji);
        //            }
        //        }
        //    };

        //    // Várakozás a reakcióra vagy egy időkorlát
        //    var task = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromMinutes(1)));
        //    return task == tcs.Task ? tcs.Task.Result : null;
        //}

        public static async Task ManageReactionsReactionAddedUser(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
        {
            // Aktív játékok listájának átvétele
            List<GameManagerClass.GameManager> gameManagerek = Program.activeGames;

            // Reakciókat küldő játékos keresése az aktív játékokban
            foreach (var gm in gameManagerek)
            {
                // Megnézzük, hogy a reakciót küldő játékos benne van-e a játékosok listájában
                var votingPlayer = gm.gameInfo.players.FirstOrDefault(p => p.Id == reaction.UserId.ToString());

                if (votingPlayer != null && votingPlayer.IsAlive && !votingPlayer.AlreadyVote)
                {
                    // Az üzenetet és csatornát kibontjuk, ha szükséges
                    var message = await cacheableMessage.GetOrDownloadAsync();
                    var channel = await cacheableChannel.GetOrDownloadAsync();

                    if (message != null && channel != null)
                    {
                        // Emoji alapján megtaláljuk, kire szavaztak
                        Emoji? emoji = reaction.Emote as Emoji;

                        if (emoji != null)
                        {
                            // Élő játékosok emoji társítása a játékban
                            var livePlayers = new Dictionary<Emoji, Player>();
                            var emojiList = new List<Emoji>
                            {
                                ReactionTypes.oneEmoji, ReactionTypes.twoEmoji, ReactionTypes.threeEmoji, ReactionTypes.fourEmoji,
                                ReactionTypes.fiveEmoji, ReactionTypes.sixEmoji, ReactionTypes.sevenEmoji, ReactionTypes.eightEmoji,
                                ReactionTypes.nineEmoji, ReactionTypes.tenEmoji
                            };

                            int playerIndex = 0;
                            foreach (var targetPlayer in gm.gameInfo.players)
                            {
                                if (targetPlayer.IsAlive)
                                {
                                    if (playerIndex < emojiList.Count)
                                    {
                                        livePlayers.Add(emojiList[playerIndex], targetPlayer);
                                        playerIndex++;
                                    }
                                }
                            }

                            // Ha a reakció érvényes emoji és élő játékosra vonatkozik
                            if (livePlayers.ContainsKey(emoji))
                            {
                                var votedPlayer = livePlayers[emoji];
                                votedPlayer.ReceiveVote(); // Szavazat hozzáadása a célzott játékoshoz
                                votingPlayer.AlreadyVote = true; // Jelzés, hogy a játékos már szavazott

                                // Visszajelzés a szavazó játékosnak privát üzenetben
                                var user = Program.Client.GetUser(ulong.Parse(votingPlayer.Id));
                                if (user != null)
                                {
                                    await user.SendMessageAsync($"Sikeresen szavaztál {votedPlayer.Name}-ra!");
                                    Console.WriteLine("test");
                                }
                            }
                        }
                    }
                }

            }
        }

    }
}
