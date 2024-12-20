﻿using Amazon.S3.Model;
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
using System.Diagnostics;

namespace Discord_Kor.GameComponents.GameManagerClass;

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
        }//s
        await BotMessages.UpdateLastMessage(gameInfo);
    }

    public async Task GameStarted()
    {
        foreach (var player in gameInfo.players)
        {
            player.CreatePersonality();
        }
        await BotMessages.SendPlayersDataSheet(gameInfo);
        await GameRunning();
    }
    public async Task GameRunning()
    {
        while (true)
        {
            int alivePlayersCount = gameInfo.players.Where(p => p.IsAlive == true).Count();
            gameInfo.allPlayersVoted = gameInfo.players.All(p => p.AlreadyVote);

            if (alivePlayersCount < 3)
            {
                await AskTwoManStanding();
                
                break;
            }
            await VoteCircle();
        }
        await CalculateTheWinner();
        await BotMessages.SendGameEndedAndResult(gameInfo);
        Console.WriteLine("");
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
    public async Task VoteCircle()
    {
        await VoteSystem.AskPeopleToVote(gameInfo);
        await BotMessages.SendDiscussionTimeStarted(gameInfo, gameInfo.settings.DiscussionTime);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        while (!gameInfo.allPlayersVoted)
        {
            gameInfo.allPlayersVoted = gameInfo.players.All(p => p.AlreadyVote);
            // Ha még nem szavazott mindenki, várunk egy kicsit
            if (!gameInfo.allPlayersVoted)
            {
                await Task.Delay(1000);
            }
            else
            {
                break;
            }

            if (stopwatch.Elapsed.TotalSeconds >= gameInfo.settings.DiscussionTime)
            {
                await VoteSystem.NotifyLateVoters(gameInfo);
                foreach (var player in gameInfo.players)
                {
                    player.AlreadyVote = true;
                }
                break;
            }
        }
        VoteResult voteResult = VoteCalculator.CalculateVotes(gameInfo);

        gameInfo.ApplieVoteResults(voteResult);
        
        if (voteResult.votesAreEven)
        {
            await BotMessages.SendEvenVotesResult(gameInfo, voteResult);
        }
        else
        {
            await BotMessages.SendVotesResult(gameInfo, voteResult);
        }
    }
    public async Task WaitForVoteCircleEnd()
    {
        await BotMessages.SendDiscussionTimeStarted(gameInfo, gameInfo.settings.DiscussionTime);

        if (gameInfo.allPlayersVoted)
        {
            return;
        }
        else
        {
            await Task.Delay(gameInfo.settings.DiscussionTime * 1000); //azert szorzunk 1000 el hogy "mp-be valtsunk"
            if (gameInfo.allPlayersVoted == true)
            {
                return;
            }
            else
            {
                gameInfo.allPlayersVoted = true;
                await VoteSystem.NotifyLateVoters(gameInfo);
            }
        }
    }

    public async Task DiscusTime()
    {
        await BotMessages.SendDiscussionTimeStarted(gameInfo, gameInfo.settings.DiscussionTime);
    }

    public async Task AskTwoManStanding()
    {

        await BotMessages.SendTwoManStandingServerMessage(gameInfo);
        await VoteSystem.AskPlayersToDecide(gameInfo);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        while (true)
        {
            if (gameInfo.players.Count(p => p.isCooperating != null) == 2)
            {
                return;
            }
            else if(stopwatch.Elapsed.TotalSeconds >= gameInfo.settings.DecideToCooperate * 1000)
            {
                foreach (var p in gameInfo.players)
                {
                    if (p.isCooperating == null)
                    {
                        await VoteSystem.NotifyLateCooperators(gameInfo);
                        p.isCooperating = true;
                    }
                }
                return;
            }
        }
    }
    public async Task CalculateTheWinner()
    {
        List<Player> lastTwo = gameInfo.players.Where(p => p.IsAlive == true).ToList();

        if (lastTwo.Count(l => l.isCooperating == true) == 2)
        {
            await BotMessages.SendTheTwoWinner(gameInfo.gameChannelId, lastTwo);
        }
        else if (lastTwo.Count(l => l.isCooperating == true) == 1)
        {
            foreach (var l in lastTwo)
            {
                if (l.isCooperating == true)
                {
                    l.IsAlive = false;
                    break;
                }
            }
            foreach (var player in gameInfo.players)
            {
                foreach(var lp in lastTwo)
                {
                    if (player.Id == lp.Id)
                    {
                        player.IsAlive = lp.IsAlive;
                    }
                }
            }
            await BotMessages.SendTheOneWinner(gameInfo.gameChannelId,lastTwo);
        }
        else if (lastTwo.Count(l => l.isCooperating == true) == 0)
        {
            foreach (var l in lastTwo)
            {
                l.IsAlive = false;
            }
            gameInfo.players.ForEach(p => p.IsAlive = false);
            await BotMessages.SendNoWinner(gameInfo.gameChannelId, lastTwo);

        }
    }
}