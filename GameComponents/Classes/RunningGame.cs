using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.Classes
{
   public class RunningGame
    {
        public string gameMasterUserName;
        public string gameMasterDiscordID;
        public string gameServerId;
        public string gameChannelId;


        public RunningGame()
        {
        }

        public RunningGame(string gameMasterUserName, string gameMasterDiscordID, string gameServerId, string gameChannelId)
        {
            this.gameMasterUserName = gameMasterUserName;
            this.gameMasterDiscordID = gameMasterDiscordID;
            this.gameServerId = gameServerId;
            this.gameChannelId = gameChannelId;
        }
    }
}
