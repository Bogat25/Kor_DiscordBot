using Discord.Interactions;

namespace DiscordKor;


public class HelpCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [EnabledInDm(false)]
    [SlashCommand("help", "Some help")]
    public async Task Help()
    {
        var embed = EmbedTemplates.DefaultEmbed("Help - How to Play", Context);

        embed.AddField("**Game Overview**", 
            "This is a social survival game where players must vote to eliminate each other until only 2 remain. " +
            "The final 2 players face a cooperation dilemma that determines the winner(s).");

        embed.AddField("**How to Start**", 
            "• Use `/startgame` to create a new game\n" +
            "• Other players can join by reacting to the game message\n" +
            "• The game starts when the host is ready\n" +
            "• Minimum 2 players required");

        embed.AddField("**Player Personalities**", 
            "Each player receives a random character with:\n" +
            "• Age, occupation, and family status\n" +
            "• Background and political views\n" +
            "• Appearance rating (1-10)\n" +
            "Use these details to roleplay and persuade others!");

        embed.AddField("**Voting Phase**", 
            "• Players discuss during the discussion time\n" +
            "• Vote to eliminate one player\n" +
            "• If votes are tied, a random player among the tied is eliminated\n" +
            "• This repeats until only 2 players remain alive");

        embed.AddField("**Final Cooperation Phase**", 
            "When 2 players remain, each must choose:\n" +
            "• **Cooperate**: Share the victory\n" +
            "• **Betray**: Try to win alone\n\n" +
            "**Results:**\n" +
            "• Both cooperate = Both win!\n" +
            "• One betrays = Only the betrayer wins\n" +
            "• Both betray = Nobody wins");

        embed.AddField("**Commands**", 
            "`/startgame` - Start a new game\n" +
            "`/settings` - View/modify game settings\n" +
            "`/help` - Show this help message");

        embed.AddField("**Tips**", 
            "• Use your character's background to build trust\n" +
            "• Form alliances but be ready to betray\n" +
            "• Vote strategically in discussion time\n" +
            "• In the final round, trust carefully!");

        await RespondAsync(embed: embed.Build());
    }
}