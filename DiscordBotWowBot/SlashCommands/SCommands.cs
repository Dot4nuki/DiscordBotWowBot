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
            TimeZoneInfo cetZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime cetTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cetZone);
            string time = cetTime.ToString();
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
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Basic              Superior       Prime")
                    .WithColor(0xA2570B)
                    .WithThumbnail(@"https://lostark.wiki.fextralife.com/file/Lost-Ark/excavating_icon_2_trade_skills_lost_ark_wiki_guide.png")
                    .AddField("CraftingCost", oreha[0], true)
                    .AddField("CraftingCost", oreha[1], true)
                    .AddField("CraftingCost", oreha[2], true)
                    .AddField("Profitx1", oreha[3], true)
                    .AddField("Profitx1", oreha[4], true)
                    .AddField("Profitx1", oreha[5], true)
                    .AddField("Profitx30", oreha[6], true)
                    .AddField("Profitx30", oreha[7], true)
                    .AddField("Profitx30", oreha[8], true)
                    .AddField("CraftvsSell", oreha[9], true)
                    .AddField("CraftvsSell", oreha[10], true)
                    .AddField("CraftvsSell", oreha[11], true)
                    .WithTimestamp(newtime)
                    );
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
            TimeZoneInfo cetZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime cetTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cetZone);
            string time = cetTime.ToString();
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
                ulong msgId = userInfo["msgidFish"];
                double decrease = userInfo["decrease"];
                string[] oreha = OrehaMath.GetValues(decrease, option);
                DateTime newtime = DateTime.Parse(oreha[12]);
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Basic              Superior       Prime")
                    .WithColor(0x2592db)
                    .WithThumbnail(@"https://lostark.wiki.fextralife.com/file/Lost-Ark/fishing_icon_2_trade_skills_lost_ark_wiki_guide.png")
                    .AddField("CraftingCost", oreha[0], true)
                    .AddField("CraftingCost", oreha[1], true)
                    .AddField("CraftingCost", oreha[2], true)
                    .AddField("Profitx1", oreha[3], true)
                    .AddField("Profitx1", oreha[4], true)
                    .AddField("Profitx1", oreha[5], true)
                    .AddField("Profitx30", oreha[6], true)
                    .AddField("Profitx30", oreha[7], true)
                    .AddField("Profitx30", oreha[8], true)
                    .AddField("CraftvsSell", oreha[9], true)
                    .AddField("CraftvsSell", oreha[10], true)
                    .AddField("CraftvsSell", oreha[11], true)
                    .WithTimestamp(newtime)
                    );
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
            TimeZoneInfo cetZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime cetTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cetZone);
            string time = cetTime.ToString();
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
                ulong msgId = userInfo["msgidHunt"];
                double decrease = userInfo["decrease"];
                string[] oreha = OrehaMath.GetValues(decrease, option);
                DateTime newtime = DateTime.Parse(oreha[12]);
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Basic              Superior       Prime")
                    .WithColor(0xD10000)
                    .WithThumbnail(@"https://lostark.wiki.fextralife.com/file/Lost-Ark/hunting_icon_2_trade_skills_lost_ark_wiki_guide.png")
                    .AddField("CraftingCost", oreha[0], true)
                    .AddField("CraftingCost", oreha[1], true)
                    .AddField("CraftingCost", oreha[2], true)
                    .AddField("Profitx1", oreha[3], true)
                    .AddField("Profitx1", oreha[4], true)
                    .AddField("Profitx1", oreha[5], true)
                    .AddField("Profitx30", oreha[6], true)
                    .AddField("Profitx30", oreha[7], true)
                    .AddField("Profitx30", oreha[8], true)
                    .AddField("CraftvsSell", oreha[9], true)
                    .AddField("CraftvsSell", oreha[10], true)
                    .AddField("CraftvsSell", oreha[11], true)
                    .WithTimestamp(newtime)
                    );
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
        /*[SlashCommand("CreateRaid", "Create a new raid")]
        public async Task CreateRaid(InteractionContext ctx, [Option("RaidName", "raidName")] string raidName, [Choice("Valtan", "valtan")][Choice("Vykas", "vykas")][Choice("Kakul", "kakul")][Choice("Brelshalza", "brel")][Choice("Akkan", "akkan")][Option("Raid", "raid")] string raid, [Option("Date", "date")] string date, [Option("Time", "time")] string time)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("not implemented, :("));
            try
            {
                Char delimiterDate = '-';
                Char delimiterTime = ':';
                String[] arrayDate = date.Split(delimiterDate);
                String[] arrayTime = time.Split(delimiterTime);
                DateTimeOffset schedule = new DateTime(int.Parse(arrayDate[2]), int.Parse(arrayDate[1]), int.Parse(arrayDate[0]), int.Parse(arrayTime[0]), int.Parse(arrayTime[1]), 0);
                long newtime = schedule.ToUnixTimeSeconds();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(raidName + " " + raid + " " + "<t:" + newtime.ToString() + ":F>"));
            }
            catch
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                  .WithContent("Date invalid"));
            }
        }*/
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
            messageExc = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle("Basic              Superior       Prime")
                .WithColor(0xA2570B)
                .WithThumbnail(@"https://lostark.wiki.fextralife.com/file/Lost-Ark/excavating_icon_2_trade_skills_lost_ark_wiki_guide.png")
                .AddField("CraftingCost", oreha[0], true)
                .AddField("CraftingCost", oreha[1], true)
                .AddField("CraftingCost", oreha[2], true)
                .AddField("Profitx1", oreha[3], true)
                .AddField("Profitx1", oreha[4], true)
                .AddField("Profitx1", oreha[5], true)
                .AddField("Profitx30", oreha[6], true)
                .AddField("Profitx30", oreha[7], true)
                .AddField("Profitx30", oreha[8], true)
                .AddField("CraftvsSell", oreha[9], true)
                .AddField("CraftvsSell", oreha[10], true)
                .AddField("CraftvsSell", oreha[11], true)
                .WithTimestamp(newtime)
                );

            oreha = OrehaMath.GetValues(decrease, 2);
            newtime = DateTime.Parse(oreha[12]);
            messageHunt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle("Basic              Superior       Prime")
                .WithColor(0xD10000)
                .WithThumbnail(@"https://lostark.wiki.fextralife.com/file/Lost-Ark/hunting_icon_2_trade_skills_lost_ark_wiki_guide.png")
                .AddField("CraftingCost", oreha[0], true)
                .AddField("CraftingCost", oreha[1], true)
                .AddField("CraftingCost", oreha[2], true)
                .AddField("Profitx1", oreha[3], true)
                .AddField("Profitx1", oreha[4], true)
                .AddField("Profitx1", oreha[5], true)
                .AddField("Profitx30", oreha[6], true)
                .AddField("Profitx30", oreha[7], true)
                .AddField("Profitx30", oreha[8], true)
                .AddField("CraftvsSell", oreha[9], true)
                .AddField("CraftvsSell", oreha[10], true)
                .AddField("CraftvsSell", oreha[11], true)
                .WithTimestamp(newtime)
                );
            oreha = OrehaMath.GetValues(decrease, 3);
            newtime = DateTime.Parse(oreha[12]);
            messageFish = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle("Basic              Superior       Prime")
                .WithColor(0x2592db)
                .WithThumbnail(@"https://lostark.wiki.fextralife.com/file/Lost-Ark/fishing_icon_2_trade_skills_lost_ark_wiki_guide.png")
                .AddField("CraftingCost", oreha[0], true)
                .AddField("CraftingCost", oreha[1], true)
                .AddField("CraftingCost", oreha[2], true)
                .AddField("Profitx1", oreha[3], true)
                .AddField("Profitx1", oreha[4], true)
                .AddField("Profitx1", oreha[5], true)
                .AddField("Profitx30", oreha[6], true)
                .AddField("Profitx30", oreha[7], true)
                .AddField("Profitx30", oreha[8], true)
                .AddField("CraftvsSell", oreha[9], true)
                .AddField("CraftvsSell", oreha[10], true)
                .AddField("CraftvsSell", oreha[11], true)
                .WithTimestamp(newtime)
                );
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
        [SlashCommand("OrehaUnSubscribe", "Unsubscribe to the oreha information")]
        public async Task UnSub(InteractionContext ctx)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("users.json"));
            foreach (dynamic name in jsonFile)
            {
                var userInfo = name.First;
                if (userInfo["id"] == ctx.Member.Id)
                {
                    userInfo.Remove("dot4nuki");
                }
            }
            string output = JsonConvert.SerializeObject(jsonFile, Formatting.Indented);
            File.WriteAllText("users.json", output);
            await ctx.CreateResponseAsync("You are now unsubscribed to the oreha information!", true);
        }
    }
}