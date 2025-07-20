using NUnit.Framework;
using LamiaSimulation;

namespace Tests
{
    public abstract class BaseTest
    {
        protected Simulation simulation;

        [SetUp]
        protected void Setup()
        {
            simulation = Simulation.Instance;
            simulation.Reset();
            simulation.Start();
        }


        [TearDown]
        public void Cleanup()
        {
            simulation.Reset();
        }        
        
        protected float NumberForagesToMaxInventory()
        {
            var amountToExtract = Helpers.GetDataTypeById<TaskType>("forage").behaviour[0].value;
            var weightPerExtract = Helpers.GetDataTypeById<ResourceType>("raw_food").weight * amountToExtract;
            var inventoryMax = Helpers.GetDataTypeById<PopulationSpeciesType>("lamia").maxInventory;
            return (float)System.Math.Ceiling(inventoryMax / weightPerExtract);
        }
        
        protected float SecondsToMaxInventoryWhenForaging()
        {
            var timeToExtract = Helpers.GetDataTypeById<TaskType>("forage").timeToComplete;
            return (float)System.Math.Ceiling(timeToExtract * NumberForagesToMaxInventory());
        }

        protected float NumberForagesToHungry()
        {
            return 1f / Helpers.GetDataTypeById<TaskType>("forage").hungerReduction;
        }

        protected float NumberCutTreesToHungry()
        {
            return 1f / Helpers.GetDataTypeById<TaskType>("cut_trees").hungerReduction;
        }

        protected void SimulateSeconds(float seconds)
        {
            for (var i = 0; i <= seconds / .5f; i++)
                simulation.Simulate(.5f);
        }
    }
}