using Discord.Interactions;
using Discord_Kor.GameComponents.Classes;

namespace DiscordKor
{
    public class Settings : InteractionModuleBase<ShardedInteractionContext>
    {
        [EnabledInDm(false)]
        [SlashCommand("settings", "Show game settings")]
        public async Task ShowSettings()
        {
            // Létrehozunk egy GameSettings példányt
            var gameSettings = new GameSettings();

            // Embed üzenet létrehozása
            var embed = EmbedTemplates.DefaultEmbed("Game Settings", Context);

            embed.AddField("Max Players", gameSettings.MaxPlayers.ToString(), true);
            embed.AddField("Min Players", gameSettings.MinPlayers.ToString(), true);
            embed.AddField("Vote Time (seconds)", gameSettings.VoteTime.ToString(), true);
            embed.AddField("Discussion Time (seconds)", gameSettings.DiscussionTime.ToString(), true);

            // Válasz küldése
            await RespondAsync(embed: embed.Build());
        }
    }
}
