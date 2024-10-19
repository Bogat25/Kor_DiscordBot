using Discord_Kor.GameComponents.GameManagerClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.Classes
{
    public class RunningGames
    {
        public List<RunningGame> runningGameList = new List<RunningGame>();

        public void StartGame(string gameMasterUserName, string gameMasterDiscordID, string gameServerId, string gameChannelId)
        {
            runningGameList.Add(new RunningGame(new Player(gameMasterDiscordID, gameMasterUserName), gameServerId, gameChannelId));
        }

            //GameManager.GameStarted(gameMasterUserName, gameMasterDiscordID, gameChannelId);


    }

}
