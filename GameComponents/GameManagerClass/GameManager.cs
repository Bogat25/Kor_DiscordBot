using Discord;
using Discord.WebSocket;
using Discord_Kor.GameComponents.BotGameMessages;
using Discord_Kor.GameComponents.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.GameManagerClass
{
    //Console.WriteLine("Game Manager has been Started");
    public class GameManager
    {
        public RunningGame gameInfo = new RunningGame();
        public GameManager(RunningGame runningGamesList)
        {
            this.gameInfo = runningGamesList;
        }
        public async Task StartGame()
        {
            gameInfo.message = await BotMessages.GameStartedAskToJoin(gameInfo);

        }
        public async Task PlayerJoined(Player player)
        {
            if (player.id != gameInfo.players[0].id)
            {
                gameInfo.players.Add(player);
            }
            await BotMessages.UpdateLastMessage(gameInfo);
        }
        public async Task PlayerLeaved(Player player)
        {
            if (player.id != gameInfo.players[0].id)
            {
                foreach (var item in gameInfo.players)
                {
                    if (item.id == player.id)
                    {
                        var alma = gameInfo.players.Remove(item);
                        break;
                    }
                }
            }
            await BotMessages.UpdateLastMessage(gameInfo);
        }
    }
}