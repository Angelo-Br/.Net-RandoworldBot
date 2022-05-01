using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Example.Modules
{
    [Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    public class ModeratorModule : ModuleBase<SocketCommandContext>
    {
        [Command("kick")]
        [Summary("Kick the specified user.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick([Remainder]SocketGuildUser user)
        {
            await ReplyAsync($"cya {user.Mention} :wave:");
            await user.KickAsync();
        }

        [Command("spawner")]
        public async Task Spawn()
        {
            
            IEmote emote = Emote.Parse("<:hmmm:645227965440851991>");
            var builder = new ComponentBuilder()
                .WithButton("OOT Role", "ootButton", ButtonStyle.Primary, emote, row: 0)
                .WithButton("LTTP Role", "lttpButton", ButtonStyle.Primary, row: 0)
                .WithButton("Prime Role", "primeButton", ButtonStyle.Danger, row: 0)
                .WithButton("Minecraft Role", "minecraftButton", ButtonStyle.Success, row: 0);


            await ReplyAsync("All the role buttons will be located here", components: builder.Build());
        }
    }
}
