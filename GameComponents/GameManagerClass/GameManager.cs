using Amazon.S3.Model;
using Discord;
using Discord.WebSocket;
using Discord_Kor.GameComponents.BotGameMessages.PersonMessages;
using Discord_Kor.GameComponents.BotGameMessages.ServerMessages;
using Discord_Kor.GameComponents.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Discord_Kor.GameComponents.GameManagerClass
{
    //Console.WriteLine("Game Manager has been Started");
    public class GameManager
    {
        public RunningGame gameInfo = new RunningGame();
        public GameManager(RunningGame runningGamesList)
        {
            this.gameInfo = runningGamesList;
        }
        public async Task StartGame()
        {
            gameInfo.message = await BotMessages.GameStartedAskToJoin(gameInfo);
            await WaitForGameStart(gameInfo);
            await GameStarted();

        }
        public async Task PlayerJoined(Player player)
        {
            if (player.Id != gameInfo.players[0].Id)
            {
                gameInfo.players.Add(player);
            }
            await BotMessages.UpdateLastMessage(gameInfo);
        }
        public async Task PlayerLeaved(Player player)
        {
            if (player.Id != gameInfo.players[0].Id)
            {
                foreach (var item in gameInfo.players)
                {
                    if (item.Id == player.Id)
                    {
                        var alma = gameInfo.players.Remove(item);
                        break;
                    }
                }
            }
            await BotMessages.UpdateLastMessage(gameInfo);
        }

        public async Task GameStarted()
        {
            foreach (var player in gameInfo.players)
            {
                player.CreatePersonality();
            }
            // Játékos adatok elküldése
            await BotMessages.SendPlayersDataSheet(gameInfo);
            await BotMessages.SendCurrentGameState(gameInfo);
            await VoteCircle();

            VoteResult voteResult = CalculateVotes();

            if (voteResult.votesAreEven)
            {
                await BotMessages.SendEvenVotes(gameInfo);
                await VoteCircle();
                voteResult = CalculateVotes();
            }
            Console.WriteLine("");
            // Szavazás kérése minden játékostól
        }

        public async Task VoteCircle()
        {
            await VoteSystem.AskPeopleToVote(gameInfo);
            // Várakozás, amíg minden játékos szavaz
            await WaitForAllVotes(gameInfo);
        }
        public async Task WaitForGameStart(RunningGame gameInfo)
        {
            bool gameStarted = false;

            while (!gameStarted)
            {
                if (gameInfo.message.currentGameState == "running")
                {
                    return;
                }
                await Task.Delay(1000);
            }
        }

        public async Task WaitForAllVotes(RunningGame gameInfo)
        {
            bool allPlayersVoted = false;

            while (!allPlayersVoted)
            {
                // Ellenőrzés: minden játékos szavazott-e
                allPlayersVoted = gameInfo.players.All(p => p.AlreadyVote);

                // Ha még nem szavazott mindenki, várunk egy kicsit
                if (!allPlayersVoted)
                {
                    await Task.Delay(1000);
                }
            }
        }

        public VoteResult CalculateVotes()
        {
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
                voteResult.votesAreEven = true;
            }
            return voteResult;
        }

    }
}