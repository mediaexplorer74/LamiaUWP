using System;

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
        }

        // Number of pops that initial settlements can sustain
        public static int InitialSettlementPopulationCapacity = 2;

        // How much of each resource a settlement can hold by default
        public static float InitialSettlementResourceCapacity = 100f;
        
        // How long it takes for a population to deposit their inventory
        public static float depositInventoryTime = 1.0f;

        // Locations of data definition files
        public const string FilenameDataDirectory = "Data";
        public const string FilenameDataFirstNames = "firstnames.txt";
        public const string FilenameDataSurnames = "surnames.txt";
        public const string FilenameDataResources = "resources.json";
        public const string FilenameDataPopulationSpecies = "populationspecies.json";
        public const string FilenameDataTasks = "tasks.json";
        public const string FilenameLocationTypes = "locationtypes.json";
    }
}