using System;
using System.Collections.Generic;
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

namespace DiscordBotWowBot
{
    internal class MessageBuilder
    {
        public static DiscordMessageBuilder GetMessage(string[] oreha,int option)
        {
            string thumbnail = "";
            int color = 0;
            if (option == 1)
            {
                color = 0xA2570B;
                thumbnail = @"https://lostark.wiki.fextralife.com/file/Lost-Ark/excavating_icon_2_trade_skills_lost_ark_wiki_guide.png";
            }  
            else if (option == 2)
            {
                color = 0x2592db;
                thumbnail = @"https://lostark.wiki.fextralife.com/file/Lost-Ark/fishing_icon_2_trade_skills_lost_ark_wiki_guide.png";
            }    
            else
            {
                color = 0xD10000;
                thumbnail = @"https://lostark.wiki.fextralife.com/file/Lost-Ark/hunting_icon_2_trade_skills_lost_ark_wiki_guide.png";
            }  
            DateTime newtime = DateTime.Parse(oreha[12]);
            var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Basic              Superior       Prime")
                    .WithColor(color)
                    .WithThumbnail(thumbnail)
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
            return (message);
        }
    }
}
