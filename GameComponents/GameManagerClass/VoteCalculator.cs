using Discord_Kor.GameComponents.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.GameManagerClass
{
    public static class VoteCalculator
    {
        public static VoteResult CalculateVotes(RunningGame gameInfo)
        {
            Random rnd = new Random();

            VoteResult voteResult = new VoteResult();
            int maxReceivedVotes = 0;
            foreach (var p in gameInfo.players)
            {
                if (p.ReceivedVotes > maxReceivedVotes)
                {
                    maxReceivedVotes = p.ReceivedVotes;
                }
            }
            foreach (var p in gameInfo.players)
            {
                if (p.ReceivedVotes == maxReceivedVotes)
                {
                    voteResult.votedPlayers.Add(p);
                }
            }
            if (voteResult.votedPlayers.Count() > 1)
            {
                voteResult.deadPlayerID = voteResult.votedPlayers[rnd.Next(0, voteResult.votedPlayers.Count())].Id;
                voteResult.votesAreEven = true;
            }
            return voteResult;
        }


    }
}
