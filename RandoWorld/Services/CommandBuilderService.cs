using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoWorld.Services
{
    public class CommandBuilderService
    {
        #region All the slash commands name's and descriptions
        public async Task BuildAllSlashCommands(DiscordSocketClient _client)
        {
            List<ApplicationCommandProperties> applicationCommandProperties = new List<ApplicationCommandProperties>();
            try
            {
                #region Discord Guild specific commands
                // If u want to make commands specific to a discord u can use this
                var guild = _client.GetGuild(519235590328287242);
                // Next, lets create our user and message command builder. This is like the embed builder but for context menu commands.
                var guildUserCommand = new UserCommandBuilder();
                var guildMessageCommand = new MessageCommandBuilder();

                // Note: Names have to be all lowercase and match the regular expression ^[\w -]{3,32}$ / uncomment these if u want these commands to work
                //guildUserCommand.WithName("Guild User Command");
                //guildMessageCommand.WithName("Guild Message Command");
                #endregion

                #region Example for creating a slash command with a added option
                // Use this command options builder to add options to the normal slashcommands
                // this one will add a name in the add-family
                SlashCommandOptionBuilder slashCommandOptionBuilder = new SlashCommandOptionBuilder();
                slashCommandOptionBuilder.WithName("name");
                slashCommandOptionBuilder.WithType(ApplicationCommandOptionType.String);
                slashCommandOptionBuilder.WithDescription("Add a family");
                slashCommandOptionBuilder.WithRequired(true); // Only add this if you want it to be required

                SlashCommandBuilder globalCommandAddFamily = new SlashCommandBuilder();
                globalCommandAddFamily.WithName("add-family");
                globalCommandAddFamily.WithDescription("Add a family");
                globalCommandAddFamily.AddOptions(slashCommandOptionBuilder);
                applicationCommandProperties.Add(globalCommandAddFamily.Build());
                #endregion

                // The clear command
                SlashCommandBuilder globalCommandClear = new SlashCommandBuilder()
                    .WithName("clear")
                    .WithDescription("Clears the chat with the amount of messages given")
                    .AddOption("amount", ApplicationCommandOptionType.Integer, "of message to be cleared", isRequired: true);
                applicationCommandProperties.Add(globalCommandClear.Build());

                // The Help command for all the commands.
                SlashCommandBuilder globalCommandHelp = new SlashCommandBuilder();
                globalCommandHelp.WithName("help");
                globalCommandHelp.WithDescription("Gives all commands for users to use");
                applicationCommandProperties.Add(globalCommandHelp.Build());

                await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }
        #endregion

        #region The actuall logic of the slash commands
        public async Task Clear(SocketSlashCommand command)
        {
            var amountOfMessages = command.Data.Options.First().Value;
            int convertedValue = int.Parse(amountOfMessages.ToString());
            var messages = await command.Channel.GetMessagesAsync(convertedValue).FlattenAsync(); //defualt is 100
            await (command.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }

        public async Task HandleListRoleCommand(SocketSlashCommand command)
        {
            // We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
            var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

            // We remove the everyone role and select the mention of each role.
            var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            var embedBuiler = new EmbedBuilder()
                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Roles")
                .WithDescription(roleList)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            // Now, Let's respond with the embed.
            await command.RespondAsync(embed: embedBuiler.Build());
        }
        #endregion
    }
}
