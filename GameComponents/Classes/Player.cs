using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Discord_Kor.GameComponents.Classes
{
    public class Player
    {
        public string id;
        public string name;
        bool isAlive = true;

        public Player()
        {
        }

        public Player(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
