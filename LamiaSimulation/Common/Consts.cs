using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    internal static class Consts
    {
        // IDs of game pages
        public struct Pages
        {
            public static string Population = "population";
            public static string Buildings = "buildings";
            public static string Research = "research";
            public static string Upgrades = "upgrades";
        }

        // Number of pops that initial settlements can sustain
        public static int InitialSettlementPopulationCapacity = 2;

        // How much of each resource a settlement can hold by default
        public static Dictionary<string, float> InitialSettlementResourceCapacity = new()
        {
            {"raw_food", 30f},
            {"logs", 10f},
            {"research", 20f}
        };

        // How long it takes for a population to deposit their inventory
        public static float depositInventoryTime = 1.0f;
        
        // How much hunger is reduced from depositing items
        public static float depositInventoryHungerReduction = .1f;
        
        // How long it takes for a population to eat something
        public static float populationEatingTime = .4f;
        
        // How long it takes for a population to die if they are starving
        public static float populationStarvationDeathTime = 20f;

        // How long it takes for a population to spawn if there's room
        public static float populationSpawnTime = 10f;
        
        // How many population need to spawn before research is unlocked
        public static int UnlockResearchAtPopulationCount = 4;

        // Location of save file
        public const string FilenameSaveFile = "save.dat";
        
        // How long between save game intervals
        public const float SaveGameTimeInterval = 5.0f;
        
        // Locations of data definition files
        public const string FilenameDataDirectory = "Data";
        public const string FilenameDataFirstNames = "firstnames.txt";
        public const string FilenameDataSurnames = "surnames.txt";
        public const string FilenameDataResources = "resources.json";
        public const string FilenameDataPopulationSpecies = "populationspecies.json";
        public const string FilenameDataTasks = "tasks.json";
        public const string FilenameLocationTypes = "locationtypes.json";
        public const string FilenameBuildingTypes = "buildings.json";
        public const string FilenameResearchTypes = "research.json";
        public const string FilenameResourceCategories = "resourcecategories.json";
        public const string FilenameUpgrades = "upgrades.json";
    }
}