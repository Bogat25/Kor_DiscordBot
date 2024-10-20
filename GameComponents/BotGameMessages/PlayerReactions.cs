using Discord.WebSocket;
using Discord;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.BotGameMessages
{
    public class PlayerReactions
    {

        public static async Task ManageReactionsReactionAdded(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
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
                            await gm.PlayerJoined(new Player(reaction.UserId.ToString(), reaction.User.ToString()));
                        }
                        else if (reaction.Emote.Name == ReactionTypes.xEmoji.Name)
                        {

                            foreach (var game in Program.activeGames)
                            {
                                if (reaction.UserId.ToString() == game.gameInfo.players[0].id && game.gameInfo.message.lastMessageID == reaction.MessageId)
                                {
                                    await BotMessages.RemoveMessage(game.gameInfo); //kitörlöm az üzenetet
                                    Program.activeGames.Remove(game);
                                    break;
                                }
                            }
                        }
                        else if (reaction.Emote.Name == ReactionTypes.startEmoji.Name)
                        {
                            if (gm.gameInfo.players.Count >= gm.gameInfo.settings.MinPlayers)
                            {
                                await BotMessages.GameStartedSuccesfully(gm.gameInfo);
                            }
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
