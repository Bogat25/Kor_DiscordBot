using Discord;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.BotGameMessages.PersonMessages
{
    public static class VoteSystem
    {

        public static async Task AskPeopleToVote(RunningGame gameInfo)
        {
            foreach (var player in gameInfo.players)
            {
                if (player.IsAlive)
                {
                    // Megkeressük a játékos Discord felhasználóját a Discord ID alapján
                    var user = Program.Client.GetUser(ulong.Parse(player.Id));

                    if (user != null)
                    {
                        // Üzenet küldése a játékosnak
                        await user.SendMessageAsync($"Szia {player.Name}, kérlek szavazz a játékban!");
                    }
                }
            }
        }


    }
}
