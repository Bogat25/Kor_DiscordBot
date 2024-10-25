using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.Classes
{
    public class AskToCooperate
    {
        string userID;
        string messageID;

        public AskToCooperate(string userID, string messageID)
        {
            this.userID = userID;
            this.messageID = messageID;
        }
    }
}
