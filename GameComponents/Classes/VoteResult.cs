using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.Classes
{
    public class VoteResult
    {
        public bool votesAreEven = false;
        public List<Player> votedPlayers = new List<Player>();
        public string deadPlayerID = string.Empty;
        
    }
}
