using NUnit.Framework;
using LamiaSimulation;

namespace Tests
{
    public class SettlementTest
    {
        private Simulation simulation;

        [SetUp]
        public void Setup()
        {
            simulation = Simulation.Instance;
            simulation.Reset();
            simulation.Start();
        }

        [Test]
        public void TestStartingSettlementIsCalledAQuietClearing()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                simulation.Query<string, string>(ClientQuery.SettlementName, settlementUuid),
                "A Quiet Clearing"
            );
        }
        
        [Test]
        public void TestSimulationResourcesAreExtractedFromLocationToSettlement()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var locationUuid = simulation.Query<string, string>(ClientQuery.SettlementLocation, settlementUuid);
            var populationUuid = simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask,
                new ClientParameter<string>(settlementUuid),
                new ClientParameter<string>(populationUuid), 
                new ClientParameter<string>("forage")
            );
            Assert.AreEqual(
                new []{"raw_food", "logs"},
                simulation.Query<string[], string>(ClientQuery.LocationResources, locationUuid)
            );
            var defaultFoodAtLocation = Helpers.GetDataTypeById<LocationType>("origin").resources["raw_food"];
            Assert.AreEqual(
                defaultFoodAtLocation,
                simulation.Query<float, string, string>(ClientQuery.LocationResourceAmount, locationUuid, "raw_food")
            );
            var defaultLogsAtLocation = Helpers.GetDataTypeById<LocationType>("origin").resources["logs"];
            Assert.AreEqual(
                defaultLogsAtLocation,
                simulation.Query<float, string, string>(ClientQuery.LocationResourceAmount, locationUuid, "logs")
            );
            Assert.AreEqual(
                0,
                simulation.Query<string[], string>(ClientQuery.SettlementInventoryCategories, settlementUuid).Length
            );
            Assert.AreEqual(
                0.0f,
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "raw_food")
            );
            for (var i = 0; i <= 12; i++)
                simulation.Simulate(.5f);
            Assert.AreEqual(
                new []{"food"},
                simulation.Query<string[], string>(ClientQuery.SettlementInventoryCategories, settlementUuid)                
            );
            Assert.AreEqual(
                new []{"raw_food"},
                simulation.Query<string[], string, string>(ClientQuery.SettlementInventoryResources, settlementUuid, "food")                
            );
            var amountToExtract = Helpers.GetDataTypeById<TaskType>("forage").amount;
            Assert.AreEqual(
                amountToExtract * 2,
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "raw_food")
            );
            Assert.AreEqual(
                defaultFoodAtLocation - (amountToExtract * 2),
                simulation.Query<float, string, string>(ClientQuery.LocationResourceAmount, locationUuid, "raw_food")
            );
        }
    }
}