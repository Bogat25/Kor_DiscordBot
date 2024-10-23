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
        public int VoteTime { get; set; } = 60;
        public int DiscussionTime { get; set; } = 120;

        public override string ToString()
        {
            return $@"
            Max Játékosok: {MaxPlayers}
            Min Játékosok: {MinPlayers}
            Szavazási idő (másodperc): {VoteTime}
            Vita időtartam (másodperc): {DiscussionTime}";
        }
    }

}

