using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DiscordBotWowBot.SlashCommands
{
    public class SCommands : ApplicationCommandModule
    {
        [SlashCommand("Crit", "Converts crit stat into crit chance")]
        public async Task Crit(InteractionContext ctx, [Option("Critstat", "Crit Stat")] double input)
        {
            double critchance = new double();
            double i = input * 0.03577;
            critchance = i;
            critchance = Math.Round(critchance, 2);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                  .WithContent(input + " crit stat gives " + critchance.ToString() + "% crit chance"));
        }


        [SlashCommand("KBW", "Calculates keen blunt weapon's efficiency depending on critrate and critdamage values")]
        public async Task KBW(InteractionContext ctx, [Option("CritChance", "Crit Chance")] double crit, [Option("CritDamage", "Crit Damage")] double dmg)
        {
            try
            {
                dmg = Math.Round(dmg / 5.0) * 5;
                crit = Math.Round(crit / 5.0) * 5;
                if (dmg >= 200 && dmg <= 350 && crit >= 0 && crit <= 100)
                {
                    dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("KBW.json"));
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("at " + crit.ToString() + " critchance and " + dmg.ToString() + " critdamage " + $"KeenBluntWeapon gives: {jsonFile[dmg.ToString()][crit.ToString()]}".ToString() + "% Dmg"));
                }
                else
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("Invalid Numbers, crit chance should be between 0-100 and crit dmg should be between 200-350 (not including kbw)"));
                }
            }
            catch (Exception e)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Something went wrong"));
                Console.WriteLine(e);
            }
        }


        [SlashCommand("Bracelet", "Roll a random 5line ancient bracelet, gl :)")]
        public async Task Bracelet(InteractionContext ctx)
        {
            string[] stat = { "Crit", "Swiftness", "Domination", "Endurance", "Expertice", "Specialization" };
            string[] basic = { "Strength", "Inteligence", "Dexterity", "HP", "Vitality", "Mana", "Physical/Magical Armor" };
            string[] special = { "Circulate", "Fervor", "Hammer", "Ambush", "Battle", "Precise", "Expose Weakness", "Dagger", "Raid", "Superiority", "Wedge", "Sawtooth Blade", "Weapon Power", "Harvest", "Cheer", "Enlightment", "Superiority", "Roly Poly", "Charge", "Smite", "Strike", "MP Recovery", "Swift Attack", "Investment", "Flip", "Counter", "Scorn", "Superiority", "Combat HP Recovery", "Revival", "Emergency Transfusion", "First Aid", "Encore", "Cool-Headed", "Reward", "Rampage", "Mileage" };
            string[] bracelet = { "", "", "", "", "" };
            int[] vitality = { 7300, 9100, 11000 };
            int[] mana = { 200, 250, 300 };
            int[] armor = { 1200, 2500, 4000 };
            bool exit = false;

            Random rando = new Random();
            int hasStat = 0;
            int hasBasic = 0;
            int rarity = 0;
            for (int i = 0; i < 5; i++)
            {
                int effect = rando.Next(3);
                if (effect == 0 && hasStat < 2) //main stats (crit spec etc)
                {
                    int index = rando.Next(stat.Length);
                    int check = 0;
                    foreach (string s in bracelet)
                    {
                        if (bracelet[check].Contains(stat[index]))
                        {
                            exit = true;
                        }
                        check++;
                    }
                    if (exit == false)
                    {
                        hasStat++;
                        rarity = rando.Next(61, 120);
                        bracelet[i] = rarity + " " + stat[index];
                    }
                    else
                    {
                        exit = false;
                        i--;
                    }
                }
                else if (effect == 1 && hasBasic < 1) //basic stats (str hp vit mana etc)
                {
                    int index = rando.Next(basic.Length);
                    int check = 0;
                    foreach (string s in bracelet)
                    {
                        if (bracelet[check].Contains(basic[index]))
                        {
                            exit = true;
                        }
                        check++;
                    }
                    if (exit == false)
                    {
                        if (basic[index] == "Strength" || basic[index] == "Inteligence" || basic[index] == "Dexterity")
                            rarity = rando.Next(2500, 5000);
                        else if (basic[index] == "HP")
                            rarity = rando.Next(1500, 4000);
                        else if (basic[index] == "Vitality")
                        {
                            rarity = rando.Next(3);
                            rarity = vitality[rarity];
                        }
                        else if (basic[index] == "Mana")
                        {
                            rarity = rando.Next(3);
                            rarity = mana[rarity];
                        }
                        else if (basic[index] == "Physical/Magical Armor")
                        {
                            rarity = rando.Next(3);
                            rarity = armor[rarity];
                        }
                        hasBasic++;
                        bracelet[i] = rarity.ToString() + " " + basic[index];
                    }
                    else
                    {
                        exit = false;
                        i--;
                    }
                }
                else if (effect == 2) //special effects (circulate, flip etc)
                {
                    int index = rando.Next(special.Length);
                    int check = 0;
                    rarity = rando.Next(3);
                    foreach (string s in bracelet)
                    {
                        if (bracelet[check].Contains(special[index]))
                        {
                            exit = true;
                        }
                        check++;
                    }
                    if (exit == false)
                    {
                        if (rarity == 0)
                            bracelet[i] = ":blue_circle: " + special[index];
                        else if (rarity == 1)
                            bracelet[i] = ":purple_circle: " + special[index];
                        else
                            bracelet[i] = ":orange_circle: " + special[index];
                    }
                    else
                    {
                        exit = false;
                        i--;
                    }
                }
                else
                {
                    i--;
                }
            }
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(bracelet[0] + "\n" + bracelet[1] + "\n" + bracelet[2] + "\n" + bracelet[3] + "\n" + bracelet[4]));
        }
        [SlashCommand("Excavating", "Insert the latest values for selling/crafting excavating mats/oreha")]
        public async Task Excavating(InteractionContext ctx, [Option("White", "White Price")] double white, [Option("Green", "Green Price")] double green, [Option("Blue", "Blue Price")] double blue, [Option("Basic", "Basic Fusion Price")] double basic, [Option("Superior", "Superio Fusion Price")] double superior, [Option("Prime", "Prime Fusion Price")] double prime)
        {
            DateTime timeUtc = DateTime.UtcNow;
            string time = timeUtc.ToString();
            int option = 1;
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("excavationOreha.json")); //save the values into json file
            jsonFile["white"] = white;
            jsonFile["green"] = green;
            jsonFile["blue"] = blue;
            jsonFile["basic"] = basic;
            jsonFile["superior"] = superior;
            jsonFile["prime"] = prime;
            jsonFile["time"] = time;
            string output = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
            File.WriteAllText("excavationOreha.json", output);

            //new code (except the awaited response async):

            dynamic jsonFileUsers = JsonConvert.DeserializeObject(File.ReadAllText("users.json"));
            foreach (dynamic name in jsonFileUsers)
            {
                //open dm and send message to them
                var userInfo = name.First;
                ulong uId = userInfo["id"];
                ulong msgId = userInfo["msgidExc"];
                double decrease = userInfo["decrease"];
                string[] oreha = OrehaMath.GetValues(decrease, option);
                DateTime newtime = DateTime.Parse(oreha[12]);
                var message = MessageBuilder.GetMessage(oreha, option);
                var user = await ctx.Guild.GetMemberAsync(uId);
                var DMChannel = await user.CreateDmChannelAsync();
                var Message = await DMChannel.GetMessageAsync((ulong)msgId, true);
                await Message.ModifyAsync(message).ConfigureAwait(true); // editing embed message in dms
            }

            await ctx.CreateResponseAsync("Information has been updated! Check DM for the new data", true);

        }
        [SlashCommand("Fishing", "Insert the latest values for selling/crafting fishing mats/oreha")]
        public async Task Fishing(InteractionContext ctx, [Option("White", "White Price")] double white, [Option("Green", "Green Price")] double green, [Option("Blue", "Blue Price")] double blue, [Option("Basic", "Basic Fusion Price")] double basic, [Option("Superior", "Superio Fusion Price")] double superior, [Option("Prime", "Prime Fusion Price")] double prime)
        {
            DateTime timeUtc = DateTime.UtcNow;
            string time = timeUtc.ToString();
            int option = 2;
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("fishingOreha.json")); //save the values into json file
            jsonFile["white"] = white;
            jsonFile["green"] = green;
            jsonFile["blue"] = blue;
            jsonFile["basic"] = basic;
            jsonFile["superior"] = superior;
            jsonFile["prime"] = prime;
            jsonFile["time"] = time;
            string output = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
            File.WriteAllText("fishingOreha.json", output);

            //new code (except the awaited response async):

            dynamic jsonFileUsers = JsonConvert.DeserializeObject(File.ReadAllText("users.json"));
            foreach (dynamic name in jsonFileUsers)
            {
                //open dm and send message to them
                var userInfo = name.First;
                ulong uId = userInfo["id"];
                ulong msgId = userInfo["msgidFish"];
                double decrease = userInfo["decrease"];
                string[] oreha = OrehaMath.GetValues(decrease, option);
                DateTime newtime = DateTime.Parse(oreha[12]);
                var message = MessageBuilder.GetMessage(oreha, option);
                var user = await ctx.Guild.GetMemberAsync(uId);
                var DMChannel = await user.CreateDmChannelAsync();
                var Message = await DMChannel.GetMessageAsync((ulong)msgId, true);
                await Message.ModifyAsync(message).ConfigureAwait(true); // editing embed message in dms
            }

            await ctx.CreateResponseAsync("Information has been updated! Check DM for the new data", true);
        }
        [SlashCommand("Hunting", "Insert the latest values for selling/crafting hunting mats/oreha")]
        public async Task Hunting(InteractionContext ctx, [Option("White", "White Price")] double white, [Option("Green", "Green Price")] double green, [Option("Blue", "Blue Price")] double blue, [Option("Basic", "Basic Fusion Price")] double basic, [Option("Superior", "Superio Fusion Price")] double superior, [Option("Prime", "Prime Fusion Price")] double prime)
        {
            DateTime timeUtc = DateTime.UtcNow;
            string time = timeUtc.ToString();
            int option = 3;
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("huntingOreha.json")); //save the values into json file
            jsonFile["white"] = white;
            jsonFile["green"] = green;
            jsonFile["blue"] = blue;
            jsonFile["basic"] = basic;
            jsonFile["superior"] = superior;
            jsonFile["prime"] = prime;
            jsonFile["time"] = time;
            string output = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
            File.WriteAllText("huntingOreha.json", output);

            //new code (except the awaited response async):

            dynamic jsonFileUsers = JsonConvert.DeserializeObject(File.ReadAllText("users.json"));
            foreach (dynamic name in jsonFileUsers)
            {
                //open dm and send message to them
                var userInfo = name.First;
                ulong uId = userInfo["id"];
                ulong msgId = userInfo["msgidHunt"];
                double decrease = userInfo["decrease"];
                string[] oreha = OrehaMath.GetValues(decrease, option);
                DateTime newtime = DateTime.Parse(oreha[12]);
                var message = MessageBuilder.GetMessage(oreha, option);
                var user = await ctx.Guild.GetMemberAsync(uId);
                var DMChannel = await user.CreateDmChannelAsync();
                var Message = await DMChannel.GetMessageAsync((ulong)msgId, true);
                await Message.ModifyAsync(message).ConfigureAwait(true); // editing embed message in dms
            }

            await ctx.CreateResponseAsync("Information has been updated! Check DM for the new data", true);
        }

        [SlashCommand("PartySynergy", "Shows the class' synergies")]
        public async Task ClassSyn(InteractionContext ctx, [Choice("Berserker", "berserker")] [Choice("Paladin", "paladin")][Choice("Gunlancer", "gunlancer")][Choice("Destroyer", "destroyer")][Choice("Slayer", "slayer")][Choice("Striker", "striker")][Choice("Wardancer", "wardancer")][Choice("Scrapper", "scrapper")][Choice("Soulfist", "soulfist")][Choice("Glaivier", "glaivier")][Choice("Gunslinger", "gunslinger")][Choice("Artillerist", "artillerist")][Choice("Deadeye", "deadeye")][Choice("Sharpshooter", "sharpshooter")][Choice("Scouter", "scouter")][Choice("Bard", "bard")][Choice("Sorceress", "sorceress")][Choice("Arcana", "arcana")][Choice("Summoner", "summoner")][Choice("Shadowhunter", "shadowhunter")][Choice("Deathblade", "deathblade")][Choice("Reaper", "reaper")][Choice("Artist", "artist")][Choice("Aeromancer", "aeromancer")][Option("Class", "Class")] string className)
        {

            string synergies = "";
            int i = 0;
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("partysynergies.json"));
            foreach (dynamic obj in jsonFile)
            {
                if (obj["class"] == className)
                {
                    synergies += obj["party synergy"] + "\n";
                    i++;
                }
            }
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(className + " synergies are:\n" + synergies));
        }
        [SlashCommand("OrehaSubscribe", "Subscribe to view and get updated oreha information automatically")]
        public async Task Sub(InteractionContext ctx, [Option("Decrease", "Stronghold decrease")] double decrease)
        {
            ulong? id = 0;
            bool newmember = true;
            ulong username = 0;
            DateTime newtime;
            string[] oreha;
            var messageExc = new DiscordMessageBuilder();
            var messageHunt = new DiscordMessageBuilder();
            var messageFish = new DiscordMessageBuilder();
            var DMChannel = await ctx.Member.CreateDmChannelAsync();
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("users.json"));
            oreha = OrehaMath.GetValues(decrease, 1);
            newtime = DateTime.Parse(oreha[12]);
            messageExc = MessageBuilder.GetMessage(oreha, 1);

            oreha = OrehaMath.GetValues(decrease, 3);
            newtime = DateTime.Parse(oreha[12]);
            messageHunt = MessageBuilder.GetMessage(oreha, 3);
            oreha = OrehaMath.GetValues(decrease, 2);
            newtime = DateTime.Parse(oreha[12]);
            messageFish = MessageBuilder.GetMessage(oreha, 2);
            foreach (dynamic name in jsonFile)
            {
                var userInfo = name.First;
                if (userInfo["id"] == ctx.Member.Id)
                {
                    newmember = false;
                    var msgExc = await DMChannel.SendMessageAsync(messageExc); //sending oreha to dms
                    var msgHunt = await DMChannel.SendMessageAsync(messageHunt);
                    var msgFish = await DMChannel.SendMessageAsync(messageFish);
                    JObject obj = new JObject();
                    obj.Add("id", ctx.User.Id);
                    obj.Add("decrease", decrease.ToString());
                    obj.Add("msgidExc", msgExc.Id);
                    obj.Add("msgidHunt", msgHunt.Id);
                    obj.Add("msgidFish", msgFish.Id);
                    jsonFile[ctx.User.Username] = obj;
                    string output = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
                    File.WriteAllText("users.json", output);
                    await ctx.CreateResponseAsync("Updating oreha information complete! New message sent.", true);
                }
            }
            if (newmember)
            {
                var msgExc = await DMChannel.SendMessageAsync(messageExc); //sending oreha to dms
                var msgHunt = await DMChannel.SendMessageAsync(messageHunt);
                var msgFish = await DMChannel.SendMessageAsync(messageFish);
                JObject obj = new JObject();
                obj.Add("id", ctx.User.Id);
                obj.Add("decrease", decrease.ToString());
                obj.Add("msgidExc", msgExc.Id);
                obj.Add("msgidHunt", msgHunt.Id);
                obj.Add("msgidFish", msgFish.Id);
                jsonFile[ctx.User.Username] = obj;
                string output = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
                File.WriteAllText("users.json", output);
            }
            await ctx.CreateResponseAsync("You are now subscribed to the oreha information! You will recieve automatically updated information into your DMs", true); //sending responce to channel (to prevent bot from waiting)
        }
        [SlashCommand("Help", "Help command")]
        public async Task Help(InteractionContext ctx, [Choice("Crit", "crit")][Choice("KBW", "kbw")][Choice("Bracelet", "bracelet")][Choice("PartySynergy", "partysynergy")][Choice("Excavating", "excavating")][Choice("Fishing", "fishing")][Choice("Hunting", "hunting")][Choice("OrehaSubscribe", "orehasubscribe")][Choice("Help", "help")][Option("Command","command")] string command)
        {
            string text = "";
            string title = command;
            if (command == "crit")
            {
                text = "Calculates your % crit chance depending on crit stat";
                title += " [critstat]";
            }
            else if (command == "kbw")
            {
                text = "Shows keen blunt weapon efficiency depending on crit chance and crit dmg (not including keenbluntweapon engraving, so base crit dmg is 200), the inputed values are rounded to the nearest 5";
                title += " [critchance] [critdmg]";
            }
            else if (command == "bracelet")
            {
                text = "Rolls a random bracelet with 5 lines, the circle's colors represent the rarity of the roll (for example, orange circle Fervor means legendary Fervor)";
            }
            else if (command == "partysynergy")
            {
                text = "Shows all the classes party synergies and party buffs";
                title += " [classname]";
            }
            else if (command == "excavating")
            {
                text = "Inserts the values from the ingame market to the database, and then updates everyone (that is subscribed) with the new data (the values are edited in the DMs, to prevent message spam)";
                title += " [white] [green] [blue] [basic] [superior] [prime]";
            }
            else if (command == "fishing")
            {
                text = "Inserts the values from the ingame market to the database, and then updates everyone (that is subscribed) with the new data (the values are edited in the DMs, to prevent message spam)";
                title += " [white] [green] [blue] [basic] [superior] [prime]";
            }
            else if (command == "hunting")
            {
                text = "Inserts the values from the ingame market to the database, and then updates everyone (that is subscribed) with the new data (the values are edited in the DMs, to prevent message spam)";
                title += " [white] [green] [blue] [basic] [superior] [prime]";
            }
            else if (command == "orehasubscribe")
            {
                text = "Subscribe to the oreha information, the information will be sent to the DMs and will be updated automatically, any updates will be edited in the DMs with the bot to prevent spam. the decrease value is the discount depending on stronghold structures, this value is calculated ingame and can be found when crafting orehas in % green text. The timestamp is based on user's current time";
                title += " [decrease]";
            }
            else if (command == "help")
            {
                text = "Shows a more detailed description to the command and how to use.";
            }
            var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("**/" + command + "**")
                    .WithColor(0x555555)
                    .WithThumbnail(@"https://preview.redd.it/7u51c9kq36i81.png?width=1176&format=png&auto=webp&s=b6fb959847b09369b793f3cf2a4d54a2981ed4be")
                    .WithDescription(text)
                    );
            await ctx.Channel.SendMessageAsync(message);
            await ctx.CreateResponseAsync(command);
        }
    }
}