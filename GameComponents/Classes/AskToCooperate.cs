using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.Classes
{
    public class AskToCooperate
    {
        public string userID;
        public string messageID;

        public AskToCooperate(string userID, string messageID)
        {
            this.userID = userID;
            this.messageID = messageID;
        }
    }
}
