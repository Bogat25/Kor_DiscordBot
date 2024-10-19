using Discord_Kor.GameComponents.GameManagerClass;
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
        public string gameServerId;
        public string gameChannelId;
        public List<Player> players = new List<Player>();


        public RunningGame()
        {
        }

        public RunningGame(Player gameMaster, string gameServerId, string gameChannelId)
        {
            this.gameServerId = gameServerId;
            this.gameChannelId = gameChannelId;
            players.Add(gameMaster);
        }
    }
}
