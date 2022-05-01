using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Discord.Net;
using Newtonsoft.Json;
using Discord;
using System.Linq;
using RandoWorld.Services;

namespace RandoWorld
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;
        private readonly CommandBuilderService _commandBuilder;

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;
            _commandBuilder = new CommandBuilderService();

            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.Ready += Client_Ready;
            _discord.SlashCommandExecuted += SlashCommandHandler;
            _discord.ButtonExecuted += MyButtonHandler;
        }

        /// <summary>
        /// Use this method to take in the slash commands and sent them to their respective method for the logic part
        /// Fill this list with new cases for each new slash command, keep the names the same as the command in the switchcase
        /// </summary>
        /// <param name="command">The command that will be sent by a user</param>
        /// <returns></returns>
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "list-roles":
                    await _commandBuilder.HandleListRoleCommand(command);
                    break;
                case "clear":
                    await _commandBuilder.Clear(command);
                    break;
            }
        }

        /// <summary>
        /// The handler for every button that will be pressed in the application
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public async Task MyButtonHandler(SocketMessageComponent component)
        {
            // We can now check for our custom id
            switch (component.Data.CustomId)
            {
                // Since we set our buttons custom id as 'custom-id', we can check for it like this:
                case "custom-id":
                    // Lets respond by sending a message saying they clicked the button
                    await component.RespondAsync($"{component.User.Mention} has clicked the button!");
                    break;
            }
        }

        /// <summary>
        /// Method for building all the slash commands nothing needs to be added here
        /// for new commands see the _commandBuilder
        /// </summary>
        /// <returns></returns>
        public async Task Client_Ready()
        {
            await _commandBuilder.BuildAllSlashCommands(_discord);
        }

        /// <summary>
        /// DO NOT CHANGE THIS: this is the onMessageReceivedAsync this will handle all the messages that the bot receives
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
            if (msg == null) return;
            if (msg.Author.Id == _discord.CurrentUser.Id) return;     // Ignore self when checking commands
            
            var context = new SocketCommandContext(_discord, msg);     // Create the command context

            int argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix("!", ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess)     // If not successful, reply with the error.
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}
