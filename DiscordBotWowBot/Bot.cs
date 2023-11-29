using DiscordBotWowBot.SlashCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace DiscordBotWowBot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            var config = new DiscordConfiguration
            {
                    Intents = DiscordIntents.All,
                    Token = configJson.Token,
                    TokenType = DSharpPlus.TokenType.Bot,
                    AutoReconnect = true,
                    MinimumLogLevel = LogLevel.Debug,
                };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;
            Client.ComponentInteractionCreated += ButtonPressResponce;
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] {configJson.Prefix},
                EnableMentionPrefix = true,
                EnableDms = false,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfig = Client.UseSlashCommands();
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(1)
            });
            slashCommandsConfig.RegisterCommands<SCommands>();
            await Client.ConnectAsync();
            await Task.Delay(-1);

        }

        private async Task MenuSelectResponce(DiscordClient sender, ContextMenuInteractionCreateEventArgs e)
        {
            var embedMenu = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle("Raid Selected!")
                .WithColor(0xF1F1F1)
                );
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(embedMenu));
        }

        private async Task ButtonPressResponce(DiscordClient sender, ComponentInteractionCreateEventArgs e) //Interaction handler
        {
            if(e.Interaction.Data.CustomId == "1") //button handler, with a select menu being created
            {
                var options = new List<DiscordSelectComponentOption>()
                {
                    new DiscordSelectComponentOption("Valtan","valtan"),
                    new DiscordSelectComponentOption("Vykas","vykas"),
                    new DiscordSelectComponentOption("Kakul-Saydon","kakul"),
                    new DiscordSelectComponentOption("Brelshaza","brel"),
                    new DiscordSelectComponentOption("Akkan","akkan")
                };
                var dropdown = new DiscordSelectComponent("dropdown", null, options);

                var embedMenu = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle("Creating Raid")
                .WithDescription("Select a legion raid")
                .WithColor(0xF1F1F1)
                );
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(embedMenu).AddComponents(dropdown));
            }
            else if(e.Interaction.Data.CustomId == "2")
            {
                var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle("2nd button")
                .WithColor(0xC1C1C1)
                );
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message));
            }
            else if (e.Interaction.Data.CustomId == "dropdown")
            {
                var message = new DiscordMessageBuilder()   
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle(e.Values[0])
                .WithColor(0xC1C1C1)
                );
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message));
            } 
        }

        private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
