using Discord.Interactions;
using Discord_Kor.GameComponents.Classes;
using Discord_Kor.GameComponents.GameManager;

namespace DiscordKor;


public class StartGameCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [EnabledInDm(false)]
    [SlashCommand("startgame", "Start The Game")]
    public async Task StartGame()
    {
        var embed = EmbedTemplates.DefaultEmbed("Help", Context);

        embed.AddField("`Start game`", "Some help.");


        await RespondAsync(embed: embed.Build());
        await Program.StartGame(new RunningGame(new Player(Context.User.Id.ToString(), Context.User.GlobalName),Context.Guild.Id.ToString(), Context.Channel.Id.ToString()));
    }
}