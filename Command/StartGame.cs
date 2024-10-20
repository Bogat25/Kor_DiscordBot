using Discord.Interactions;
using Discord_Kor.GameComponents.Classes;
using DiscordKor;

public class StartGameCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [EnabledInDm(false)]
    [SlashCommand("startgame", "Start The Game")]
    public async Task StartGame()
    {
        // Küldünk egy rejtett választ (ephemeral), ami csak a felhasználó lát.
        await RespondAsync("A játék indítása folyamatban van...", ephemeral: true);

        // Majd elindítjuk a játékot a háttérben
        await Program.StartGame(new RunningGame(new Player(Context.User.Id.ToString(), Context.User.GlobalName), Context.Guild.Id.ToString(), Context.Channel.Id.ToString()));
    }
}
