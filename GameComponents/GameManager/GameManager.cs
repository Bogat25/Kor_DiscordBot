﻿using Discord_Kor.GameComponents.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.GameManager
{
    //Console.WriteLine("Game Manager has been Started");
    public class GameManager
    {
        public static async Task Main(string gameManagerUserToken, string gameManagerUserName, string channelID)
        {   
            Console.WriteLine("test");
        }

        public static async Task GameStarted(RunningGame runningGame)
        {
            Console.WriteLine(runningGame.gameMasterUserName);

        }
    }
}