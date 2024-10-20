using Discord.WebSocket;
using Discord;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.BotGameMessages.ServerMessages
{
    public class PlayerReactions
    {

        public static async Task ManageReactionsReactionAdded_Server(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
        {
            if (reaction.Channel is null) //kiszuri a privat üzeneteket
            {
                return;
            }
            if (reaction.UserId == 1296986541142577223) //így a bot nem reagál a saját reakcióira
            {
                return;
            }
            List<GameManagerClass.GameManager> gameManagerek = Program.activeGames;
            bool found = false;
            foreach (var gm in gameManagerek)
            {
                if (gm.gameInfo.message.lastMessageID == reaction.MessageId)
                {
                    found = true;
                    if (gm.gameInfo.message.lastMessageType == "waitForJoin")
                    {
                        if (reaction.Emote.Name == ReactionTypes.greenCheckEmoji.Name)
                        {
                            await gm.PlayerJoined(new Player(reaction.UserId.ToString(), reaction.User.ToString()));
                        }
                        else if (reaction.Emote.Name == ReactionTypes.xEmoji.Name)
                        {

                            foreach (var game in Program.activeGames)
                            {
                                if (reaction.UserId.ToString() == game.gameInfo.players[0].Id && game.gameInfo.message.lastMessageID == reaction.MessageId)
                                {
                                    await BotMessages.RemoveMessage(game.gameInfo); //kitörlöm az üzenetet
                                    Program.activeGames.Remove(game);
                                    break;
                                }
                            }
                        }
                        else if (reaction.Emote.Name == ReactionTypes.startEmoji.Name)
                        {
                            if (reaction.UserId.ToString() == gm.gameInfo.players[0].Id)
                            {
                                if (gm.gameInfo.players.Count >= gm.gameInfo.settings.MinPlayers)
                                {
                                    await BotMessages.GameStartedSuccesfully(gm.gameInfo);

                                gm.gameInfo.message.lastMessageType = "runningGame";
                                await gm.GameStarted();
                                }
                            }
                        }
                    }
                }
            }
        }
        public static async Task ManageReactionsReactionRemoved_Server(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
        {
            if (reaction.UserId == 1296986541142577223) //így a bot nem reagál a saját reakcióira
            {
                return;
            }
            List<GameManagerClass.GameManager> gameManagerek = Program.activeGames;
            bool found = false;
            foreach (var gm in gameManagerek)
            {
                if (gm.gameInfo.message.lastMessageID == reaction.MessageId)
                {
                    found = true;
                    if (gm.gameInfo.message.lastMessageType == "waitForJoin")
                    {
                        if (reaction.Emote.Name == ReactionTypes.greenCheckEmoji.Name)
                        {
                            await gm.PlayerLeaved(new Player(reaction.UserId.ToString(), reaction.User.ToString()));
                        }
                    }
                }
            }
        }
    }
}
