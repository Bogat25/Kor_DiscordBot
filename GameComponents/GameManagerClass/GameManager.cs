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
        public ulong lastMessageID = 0;

        public GameManager(RunningGame runningGamesList)
        {
            this.gameInfo = runningGamesList;
        }
        public async Task StartGame()
        {

            lastMessageID = await BotMessages.GameStartedAskToJoin(gameInfo);

            Console.WriteLine("");
        }

    }
}
