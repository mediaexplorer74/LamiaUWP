using System;
using System.Linq;
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
            // Doesn't spawn when at capacity
            SimulateSeconds(Consts.populationSpawnTime*2);
            Assert.AreEqual(
                2,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
            );
            // increase capacity and more spawn
            simulation.PerformAction(ClientAction.SettlementForceAddBuilding, settlementUuid, "log_hut");
            SimulateSeconds(Consts.populationSpawnTime);
            Assert.AreEqual(
                3,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
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

        [Test]
        public void TestUnlockBuilding()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[], string>(ClientQuery.SettlementBuildings, settlementUuid)
            );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string, string>(ClientQuery.SettlementHasBuildingUnlocked, settlementUuid, "archives")
            );
            simulation.PerformAction(
                ClientAction.SettlementUnlockBuilding, settlementUuid, "archives"
            );
            Assert.AreEqual(
                new []{"archives"},
                simulation.Query<string[], string>(ClientQuery.SettlementBuildings, settlementUuid)
            );
            Assert.AreEqual(
                true,
                simulation.Query<bool, string, string>(ClientQuery.SettlementHasBuildingUnlocked, settlementUuid, "archives")
            );
        }
        
        [Test]
        public void TestPurchaseBuilding()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            simulation.PerformAction(
                ClientAction.SettlementUnlockBuilding, settlementUuid, "log_hut"
            );
            Assert.AreEqual(
                0,
                simulation.Query<int, string, string>(ClientQuery.SettlementBuildingsAmount, settlementUuid, "log_hut")
            );
            foreach (var (resource, value) in Helpers.GetDataTypeById<BuildingType>("log_hut").cost)
            {
                simulation.PerformAction(
                    ClientAction.AddResourceToSettlementInventory, settlementUuid, resource, value
                );
                Assert.AreEqual(
                    value,
                    simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, resource)
                );
            }
            simulation.PerformAction(
                ClientAction.SettlementPurchaseBuilding, settlementUuid, "log_hut"
            );
            Assert.AreEqual(
                1,
                simulation.Query<int, string, string>(ClientQuery.SettlementBuildingsAmount, settlementUuid, "log_hut")
            );
            foreach (var (resource, value) in Helpers.GetDataTypeById<BuildingType>("log_hut").cost)
                Assert.AreEqual(
                    0f,
                    simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, resource)
                );
        }

        [Test]
        public void TestForceAddBuilding()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                0,
                simulation.Query<int, string, string>(ClientQuery.SettlementBuildingsAmount, settlementUuid, "archives")
            );
            simulation.PerformAction(
                ClientAction.SettlementForceAddBuilding, settlementUuid, "archives"
            );
            Assert.AreEqual(
                1,
                simulation.Query<int, string, string>(ClientQuery.SettlementBuildingsAmount, settlementUuid, "archives")
            );
        }
        
        [Test]
        public void TestInventoryQuerying()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory,settlementUuid, "raw_food", 10f
            );
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory,settlementUuid, "research", 20f
            );
            Assert.AreEqual(
                new []{"special", "food"},
                simulation.Query<string[], string>(ClientQuery.SettlementInventoryCategories, settlementUuid)
            );
            Assert.AreEqual(
                new []{"raw_food"},
                simulation.Query<string[], string, string>(ClientQuery.SettlementInventoryResources, settlementUuid, "food")
            );
            Assert.AreEqual(
                new []{"research"},
                simulation.Query<string[], string, string>(ClientQuery.SettlementInventoryResources, settlementUuid, "special")
            );
            Assert.AreEqual(
                10f,
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "raw_food")
            );
            Assert.AreEqual(
                20f,
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "research")
            );
            Assert.AreEqual(
                Consts.InitialSettlementResourceCapacity["raw_food"],
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceCapacity, settlementUuid, "raw_food")
            );
            Assert.AreEqual(
                Consts.InitialSettlementResourceCapacity["research"],
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceCapacity, settlementUuid, "research")
            );
        }

        [Test]
        public void TestBuildingQuerying()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var logHut = Helpers.GetDataTypeById<BuildingType>("log_hut");
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[], string>(ClientQuery.SettlementBuildings, settlementUuid)
            );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string, string>(ClientQuery.SettlementHasBuildingUnlocked, settlementUuid, "log_hut")
            );
            simulation.PerformAction(ClientAction.SettlementUnlockBuilding, settlementUuid, "log_hut");
            Assert.AreEqual(
                true,
                simulation.Query<bool, string, string>(ClientQuery.SettlementHasBuildingUnlocked, settlementUuid, "log_hut")
            );
            Assert.AreEqual(
                "Log Hut",
                simulation.Query<string, string, string>(ClientQuery.SettlementBuildingDisplayName, settlementUuid, "log_hut")
            );
            Assert.AreEqual(
                new []
                {
                    "Really basic housing, keeps the rain off at least.",
                    "",
                    $"Increase population capacity by {logHut.behaviour.populationCapacity}"
                },
                simulation.Query<string[], string, string>(ClientQuery.SettlementBuildingDescription, settlementUuid, "log_hut")
            );
            Assert.AreEqual(
                new[] {"logs"},
                simulation.Query<string[], string, string>(ClientQuery.SettlementBuildingResourceList, settlementUuid, "log_hut")
            );
            Assert.AreEqual(
                logHut.cost["logs"],
                simulation.Query<float, string, string, string>(ClientQuery.SettlementBuildingSingleResourceCost, settlementUuid, "log_hut", "logs")
            );
            Assert.AreEqual(
                0,
                simulation.Query<int, string, string>(ClientQuery.SettlementBuildingsAmount, settlementUuid, "log_hut")
            );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string, string>(ClientQuery.SettlementBuildingCanAfford, settlementUuid, "log_hut")
            );
            simulation.PerformAction(ClientAction.AddResourceToSettlementInventory, settlementUuid, "logs", logHut.cost["logs"]);
            Assert.AreEqual(
                true,
                simulation.Query<bool, string, string>(ClientQuery.SettlementBuildingCanAfford, settlementUuid, "log_hut")
            );
            simulation.PerformAction(ClientAction.SettlementPurchaseBuilding, settlementUuid, "log_hut");
            Assert.AreEqual(
                new []{"log_hut"},
                simulation.Query<string[], string>(ClientQuery.SettlementBuildings, settlementUuid)
            );
            Assert.AreEqual(
                1,
                simulation.Query<int, string, string>(ClientQuery.SettlementBuildingsAmount, settlementUuid, "log_hut")
            );
            Assert.AreEqual(
                logHut.cost["logs"] * logHut.costGrowth,
                simulation.Query<float, string, string, string>(ClientQuery.SettlementBuildingSingleResourceCost, settlementUuid, "log_hut", "logs")
            );
            simulation.PerformAction(ClientAction.SettlementForceAddBuilding, settlementUuid, "log_hut");
            Assert.AreEqual(
                logHut.cost["logs"] * MathF.Pow(logHut.costGrowth, 2),
                simulation.Query<float, string, string, string>(ClientQuery.SettlementBuildingSingleResourceCost, settlementUuid, "log_hut", "logs")
            );
        }

        [Test]
        public void TestTaskQuerying()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            var cutTrees = Helpers.GetDataTypeById<TaskType>("cut_trees");
            simulation.PerformAction(ClientAction.UnlockTask, "cut_trees");
            Assert.AreEqual(
                new []
                {
                    cutTrees.description,
                    "",
                    $"Extract: {Helpers.GetResourceTypeById(cutTrees.behaviour[0].id).name} {cutTrees.behaviour[0].value}/{cutTrees.timeToComplete} secs"
                },
                simulation.Query<string[], string, string>(ClientQuery.SettlementTaskDescription, settlementUuid, "cut_trees")
            );
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[], string, string>(ClientQuery.SettlementTaskAssignments, settlementUuid, "cut_trees")
            );
            Assert.AreEqual(
                0,
                simulation.Query<int, string, string>(ClientQuery.SettlementTaskAssignedNum, settlementUuid, "cut_trees")
            );
            simulation.PerformAction(ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "cut_trees");
            Assert.AreEqual(
                new []{populationUuid},
                simulation.Query<string[], string, string>(ClientQuery.SettlementTaskAssignments, settlementUuid, "cut_trees")
            );
            Assert.AreEqual(
                1,
                simulation.Query<int, string, string>(ClientQuery.SettlementTaskAssignedNum, settlementUuid, "cut_trees")
            );
            Assert.AreEqual(
                -1,
                simulation.Query<int, string, string>(ClientQuery.SettlementTaskMaximumCapacity, settlementUuid, "cut_trees")
            );
        }

        [Test]
        public void TestPopulationQuerying()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                1,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
            );
            Assert.AreEqual(
                2,
                simulation.Query<int, string>(ClientQuery.SettlementPopulationMax, settlementUuid)
            );
            Assert.AreEqual(
                new []{"lamia"},
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationSpecies, settlementUuid)
            );
            var currentPop =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid);
            Assert.AreEqual(1, currentPop.Length);
            Assert.AreEqual(
                currentPop,
                simulation.Query<string[], string, string>(ClientQuery.SettlementPopulationSpeciesMembers, settlementUuid, "lamia")
            );
            Assert.AreEqual(
                2,
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberName, settlementUuid, currentPop[0]).Split(" ").Length
            );
            Assert.AreEqual(
                "lamia",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberSpecies, settlementUuid, currentPop[0])
            );
            Assert.AreEqual(
                "idle",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid, currentPop[0])
            );
            simulation.PerformAction(ClientAction.PopulationAssignToTask, settlementUuid, currentPop[0], "forage");
            Assert.AreEqual(
                "forage",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid, currentPop[0])
            );
            simulation.PerformAction(ClientAction.AddPopulation, settlementUuid, "lamia");
            Assert.AreEqual(
                2,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
            );
            currentPop =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid);
            Assert.AreEqual(2, currentPop.Length);
            simulation.PerformAction(ClientAction.SettlementForceAddBuilding, settlementUuid, "log_hut");
            Assert.AreEqual(
                3,
                simulation.Query<int, string>(ClientQuery.SettlementPopulationMax, settlementUuid)
            );
        }

        [Test]
        public void TestGetTimeToCompleteTask()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var cutTrees = Helpers.GetTaskTypeById("cut_trees");
            var stoneAxe = Helpers.GetDataTypeById<UpgradeType>("stone_axe");
            Assert.AreEqual(
                cutTrees.timeToComplete,
                Settlement.GetTimeToCompleteTask(settlementUuid, "cut_trees")
            );
            simulation.PerformAction(ClientAction.ForceUnlockUpgrade, "stone_axe");
            foreach (var behaviour in stoneAxe.behaviour)
            {
                if(behaviour.method != UpgradeBehaviourMethod.TASK_SPEED_ADJUST)
                    continue;
                Assert.AreEqual(
                    cutTrees.timeToComplete * behaviour.value,
                    Settlement.GetTimeToCompleteTask(settlementUuid, "cut_trees")
                );
            }
        }
        
        [Test]
        public void TestGetExtractTaskAmount()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var cutTrees = Helpers.GetTaskTypeById("cut_trees");
            var stoneAxe = Helpers.GetDataTypeById<UpgradeType>("stone_axe");
            Assert.AreEqual(
                cutTrees.behaviour[0].value,
                Settlement.GetExtractTaskAmount(settlementUuid, "cut_trees")
            );
            simulation.PerformAction(ClientAction.ForceUnlockUpgrade, "stone_axe");
            foreach (var behaviour in stoneAxe.behaviour)
            {
                if(behaviour.method != UpgradeBehaviourMethod.TASK_EXTRACT_AMOUNT_ADJUST)
                    continue;
                Assert.AreEqual(
                    cutTrees.behaviour[0].value * behaviour.value,
                    Settlement.GetExtractTaskAmount(settlementUuid, "cut_trees")
                );
            }
        }

        [Test]
        public void TestCutTreesTaskUnlocksWhenGettingFirstRawFood()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.TaskUnlocked, "cut_trees")
            );
            simulation.PerformAction(ClientAction.AddResourceToSettlementInventory, settlementUuid, "raw_food", 10f);
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.TaskUnlocked, "cut_trees")
            );
        }
        
        [Test]
        public void TestBuildingsPageUnlocksWhenGettingFirstLogs()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.HasUnlockedPage, Consts.Pages.Buildings)
            );
            simulation.PerformAction(ClientAction.AddResourceToSettlementInventory, settlementUuid, "logs", 10f);
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.HasUnlockedPage, Consts.Pages.Buildings)
            );
        }

        [Test]
        public void TestResearchPageUnlocksWhenGettingFirstResearch()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.HasUnlockedPage, Consts.Pages.Research)
            );
            simulation.PerformAction(ClientAction.AddResourceToSettlementInventory, settlementUuid, "research", 10f);
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.HasUnlockedPage, Consts.Pages.Research)
            );
        }

        [Test]
        public void TestResearchTaskUnlocksWhenHittingPopulationThreshold()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid = simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "forage");
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.TaskUnlocked, "research")
            );
            foreach(var i in Enumerable.Range(0, Consts.UnlockResearchAtPopulationCount - 2))
                simulation.PerformAction(ClientAction.SettlementForceAddBuilding, settlementUuid, "log_hut");
            SimulateSeconds(Consts.populationSpawnTime * (Consts.UnlockResearchAtPopulationCount + 1));
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.TaskUnlocked, "research")
            );
        }
        
    }
}