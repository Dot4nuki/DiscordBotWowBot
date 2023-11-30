using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotWowBot
{
    internal class OrehaMath
    {
        public static string[] GetValues(double decrease,int option)
        {
            dynamic jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("excavationOreha.json")); //can be removed?
            if(option == 1)
                jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("excavationOreha.json"));
            else if(option == 2)
                jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("fishingOreha.json"));
            else if (option == 3)
                jsonFile = JsonConvert.DeserializeObject(File.ReadAllText("huntingOreha.json"));
            double white = jsonFile["white"];
            double green = jsonFile["green"];
            double blue = jsonFile["blue"];
            double basic = jsonFile["basic"];
            double superior = jsonFile["superior"];
            double prime = jsonFile["prime"];
            string time = jsonFile["time"];

            double basiccraftingcost = 0;
            double superiorcraftingcost = 0;
            double primecraftingcost = 0;
            double basicmatwhite = 0;
            double basicmatgreen = 0;
            double basicmatblue = 0;
            double superiormatwhite = 0;
            double superiormatgreen = 0;
            double superiormatblue = 0;
            double primematwhite = 0;
            double primematgreen = 0;
            double primematblue = 0;
            double basicprofit = 0;
            double superiorprofit = 0;
            double primeprofit = 0;
            double basicprofitbundle = 0;
            double superiorprofitbundle = 0;
            double primeprofitbundle = 0;
            double basicSellvsCraft = 0;
            double superiorSellvsCraft = 0;
            double primeSellvsCraft = 0;
            double basicCraft = 0;
            double superiorCraft = 0;
            double primeCraft = 0;
            basicCraft = (decrease / 100) * 205; //calculate gold needed to craft orehas depending on stronghold crafting cost % decrease
            superiorCraft = (decrease / 100) * 250;
            primeCraft = (decrease / 100) * 300;
            basicCraft = Math.Round(basicCraft, 0);
            superiorCraft = Math.Round(superiorCraft, 0);
            primeCraft = Math.Round(primeCraft, 0);
            basicCraft = 205 - basicCraft;
            superiorCraft = 250 - superiorCraft;
            primeCraft = 300 - primeCraft;
            basicmatwhite = white * 0.64; //gold per mats to craft each oreha
            basicmatgreen = green * 2.6;
            basicmatblue = blue * 0.8;
            superiormatwhite = white * 0.94;
            superiormatgreen = green * 2.9;
            superiormatblue = blue * 1.6;
            primematwhite = white * 1.07;
            primematgreen = green * 5.1;
            primematblue = blue * 5.2;
            basiccraftingcost = basicCraft + basicmatwhite + basicmatgreen + basicmatblue; // crafting cost per oreha
            superiorcraftingcost = superiorCraft + superiormatwhite + superiormatgreen + superiormatblue;
            primecraftingcost = primeCraft + primematwhite + primematgreen + primematblue;
            basiccraftingcost = Math.Round(basiccraftingcost, 3); // rounding
            superiorcraftingcost = Math.Round(superiorcraftingcost, 3);
            primecraftingcost = Math.Round(primecraftingcost, 3);
            basicprofit = ((basic - 1) * 30) - basiccraftingcost; //profit per craft
            superiorprofit = ((superior - 2) * 20) - superiorcraftingcost;
            primeprofit = ((prime - 3) * 15) - primecraftingcost;
            basicprofit = Math.Round(basicprofit, 3);
            superiorprofit = Math.Round(superiorprofit, 3);
            primeprofit = Math.Round(primeprofit, 3);
            basicprofitbundle = basicprofit * 30; //profit per craft x30 aka bundle
            superiorprofitbundle = superiorprofit * 30;
            primeprofitbundle = primeprofit * 30;
            basicprofitbundle = Math.Round(basicprofitbundle, 3);
            superiorprofitbundle = Math.Round(superiorprofitbundle, 3);
            primeprofitbundle = Math.Round(primeprofitbundle, 3);
            basicSellvsCraft = ((basicmatwhite + basicmatgreen + basicmatblue) * 30 * 0.95 + (basicCraft * 30)) - ((basic - 1) * 900); //sell mats vs crafting x30
            superiorSellvsCraft = ((superiormatwhite + superiormatgreen + superiormatblue) * 30 * 0.95 + (superiorCraft * 30)) - ((superior - 2) * 600);
            primeSellvsCraft = ((primematwhite + primematgreen + primematblue) * 30 * 0.95 + (primeCraft * 30)) - ((prime - 3) * 450);
            basicSellvsCraft = Math.Round(basicSellvsCraft, 3);
            superiorSellvsCraft = Math.Round(superiorSellvsCraft, 3);
            primeSellvsCraft = Math.Round(primeSellvsCraft, 3);
            string[] result = { basiccraftingcost.ToString(), superiorcraftingcost.ToString(), primecraftingcost.ToString(), basicprofit.ToString(), superiorprofit.ToString(), primeprofit.ToString(), basicprofitbundle.ToString(), superiorprofitbundle.ToString(), primeprofitbundle.ToString(), basicSellvsCraft.ToString(), superiorSellvsCraft.ToString(), primeSellvsCraft.ToString(), time };
            return result;
        }
    }
}
