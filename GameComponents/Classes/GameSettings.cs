using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.Classes
{
    public class GameSettings
    {
        public int MaxPlayers { get; set; } = 10;
        public int MinPlayers { get; set; } = 2;
        public int VoteTime { get; set; } = 60; //seconds
        public int DiscussionTime { get; set; } = 300; //seconds        
    }
}

