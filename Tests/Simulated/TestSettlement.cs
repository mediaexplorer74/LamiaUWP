using NUnit.Framework;
using LamiaSimulation;

namespace Tests
{
    public class SettlementTest: BaseTest
    {
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
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "forage"
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
                0f,
                simulation.Query<float, string, string>(ClientQuery.PopulationMemberInventoryProgress, settlementUuid, populationUuid)
            );
            var timeToExtract = Helpers.GetDataTypeById<TaskType>("forage").timeToComplete;
            SimulateSeconds(timeToExtract*2);
            Assert.Greater(
                simulation.Query<float, string, string>(ClientQuery.PopulationMemberInventoryProgress, settlementUuid, populationUuid),
                0f
            );
            var amountToExtract = Helpers.GetDataTypeById<TaskType>("forage").behaviour[0].value;
            Assert.AreEqual(
                defaultFoodAtLocation - (amountToExtract * 2),
                simulation.Query<float, string, string>(ClientQuery.LocationResourceAmount, locationUuid, "raw_food")
            );
        }
        
        [Test]
        public void TestWillSpawnMorePopulation()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                1,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
                );
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory, settlementUuid, "raw_food", 10f
                );
            SimulateSeconds(Consts.populationSpawnTime);
            Assert.AreEqual(
                2,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
            );
        }

        [Test]
        public void TestUnlockTask()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                new []{"idle", "forage"},
                simulation.Query<string[], string>(ClientQuery.SettlementTasks, settlementUuid)
            );
            simulation.PerformAction(ClientAction.UnlockTask, settlementUuid,"cut_trees");
            Assert.AreEqual(
                new []{"idle", "forage", "cut_trees"},
                simulation.Query<string[], string>(ClientQuery.SettlementTasks, settlementUuid)
            );
        }

        [Test]
        public void TestRenameSettlement()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                "A Quiet Clearing",
                simulation.Query<string, string>(ClientQuery.SettlementName, settlementUuid)
            );
            simulation.PerformAction(ClientAction.RenameSettlement, settlementUuid, "Nowhere");
            Assert.AreEqual(
                "Nowhere",
                simulation.Query<string, string>(ClientQuery.SettlementName, settlementUuid)
            );
        }
        
        [Test]
        public void TestRemovePopulation()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid = simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            Assert.AreEqual(
                1,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
            );
            simulation.PerformAction(
                ClientAction.SettlementRemovePopulation, settlementUuid,populationUuid
                );
            SimulateSeconds(.1f);
            Assert.AreEqual(
                0,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
            );
        }
        
        // test unlock building
        // ...
        
        // test purchasing building
        // ...
        
        // test unlock upgrade
        // ...
        
        // test querying inventory
        // ...
        
        // test querying buildings
        // ...
        
        // test querying upgrades
        // ...
        
        // test querying tasks
        // ...
        
        // test querying population
        // ...
        
        // test if can afford building
        // ...
        
        // test if can afford upgrade
        // ...
        
        // test GetTimeToCompleteTask
        // ...
        
        // test GetExtractTaskAmount
        // ...
        
        // test cut_trees task unlocks when getting first raw_food
        // ...
        
        // test buildings page unlocks when getting first logs
        // ...
        
        // test Research page unlocks when getting first research
        // ...
        
        // Unlock research task when hitting a Consts.UnlockResearchAtPopulationCount population
        // ...
        
        // Unlock upgrades page when getting first Archives
        // ...
        
    }
}