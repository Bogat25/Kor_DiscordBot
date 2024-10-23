using Discord_Kor.GameComponents.GameManagerClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.Classes;

public class RunningGame
{
    public string gameServerId;
    public string gameChannelId;
    public MessageInfo message = new MessageInfo();
    public List<Player> players = new List<Player>();
    public GameSettings settings = new GameSettings();
    public List<VoteAsksInRound >voteAsks = new List<VoteAsksInRound>();
    public bool allPlayersVoted = false;
    public RunningGame()
    {
    }

    public RunningGame(Player gameMaster, string gameServerId, string gameChannelId)
    {
        this.gameServerId = gameServerId;
        this.gameChannelId = gameChannelId;
        players.Add(gameMaster);
    }



    public void ApplieVoteResults(VoteResult voteResult)
    {
        Random rnd = new Random();
        if (voteResult.votesAreEven)
        {
            voteResult.deadPlayerID = voteResult.votedPlayers[rnd.Next(voteResult.votedPlayers.Count)].Id;
        }else
        {
            voteResult.deadPlayerID = voteResult.votedPlayers[0].Id;
        }

        foreach (var player in players)
        {
            if (player.Id == voteResult.deadPlayerID)
            {
                player.IsAlive = false;
            }
        }
        foreach (var player in players )
        {
            player.AlreadyVote = false;
            player.ReceivedVotes = 0;
        }
    }
}
