using System.Linq;
using NUnit.Framework;
using LamiaSimulation;

namespace Tests
{
    public class GlobalStateTest : BaseTest
    {
        [Test]
        public void TestPageUnlock()
        {
            Assert.AreEqual(
                new []
                {
                    (Consts.Pages.Population, GlobalState.PageDisplayName(Consts.Pages.Population))
                },
                simulation.Query<(string, string)[]>(ClientQuery.AvailablePages)
                );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.HasUnlockedPage, Consts.Pages.Buildings)
            );
            simulation.PerformAction(ClientAction.UnlockPage, Consts.Pages.Buildings);
            Assert.AreEqual(
                new []
                {
                    (Consts.Pages.Population, GlobalState.PageDisplayName(Consts.Pages.Population)),
                    (Consts.Pages.Buildings, GlobalState.PageDisplayName(Consts.Pages.Buildings))
                },
                simulation.Query<(string, string)[]>(ClientQuery.AvailablePages)
            );
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.HasUnlockedPage, Consts.Pages.Buildings)
            );
        }

        [Test]
        public void TestMessages()
        {
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.MessageHistory)
            );
            Assert.AreEqual(
                Consts.InitialMessages,
                simulation.Query<string[]>(ClientQuery.UnreadMessages)
            );
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.UnreadMessages)
            );
            simulation.PerformAction(ClientAction.SendMessage, "hello world!");
            Assert.AreEqual(
                new []{"hello world!"},
                simulation.Query<string[]>(ClientQuery.UnreadMessages)
            );
            Assert.AreEqual(
                Consts.InitialMessages,
                simulation.Query<string[]>(ClientQuery.MessageHistory)
            );
            foreach(var i in Enumerable.Range(0, Consts.MessageHistoryLimit))
                simulation.PerformAction(ClientAction.SendMessage, "padding");
            Assert.AreEqual(
                Consts.MessageHistoryLimit,
                simulation.Query<string[]>(ClientQuery.UnreadMessages).Length
            );
            simulation.PerformAction(ClientAction.SendMessage, "send messages to history");
            simulation.Query<string[]>(ClientQuery.UnreadMessages);
            Assert.AreEqual(
                Consts.MessageHistoryLimit,
                simulation.Query<string[]>(ClientQuery.MessageHistory).Length
            );
        }

        [Test]
        public void TestLocations()
        {
            Assert.AreEqual(
                1,
                simulation.Query<string[]>(ClientQuery.Locations).Length
            );
            simulation.PerformAction(ClientAction.AddLocation, "forest");
            Assert.AreEqual(
                2,
                simulation.Query<string[]>(ClientQuery.Locations).Length
            );
        }
        
        [Test]
        public void TestSettlements()
        {
            Assert.AreEqual(
                1,
                simulation.Query<string[]>(ClientQuery.Settlements).Length
            );
            simulation.PerformAction(ClientAction.AddLocation, "forest");
            simulation.PerformAction(ClientAction.AddSettlementAtLocation, simulation.LastID);
            Assert.AreEqual(
                2,
                simulation.Query<string[]>(ClientQuery.Settlements).Length
            );
        }

        [Test]
        public void TestUnlockTask()
        {
            Assert.AreEqual(
                new []{"idle", "forage"},
                simulation.Query<string[]>(ClientQuery.Tasks)
            );
            simulation.PerformAction(ClientAction.UnlockTask,"cut_trees");
            Assert.AreEqual(
                new []{"idle", "forage", "cut_trees"},
                simulation.Query<string[]>(ClientQuery.Tasks)
            );
        }

        [Test]
        public void TestTaskQuerying()
        {
            Assert.AreEqual(
                new[] { "idle", "forage" },
                simulation.Query<string[]>(ClientQuery.Tasks)
            );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.TaskUnlocked, "cut_trees")
            );
            simulation.PerformAction(ClientAction.UnlockTask, "cut_trees");
            Assert.AreEqual(
                new[] { "idle", "forage", "cut_trees" },
                simulation.Query<string[]>(ClientQuery.Tasks)
            );
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.TaskUnlocked, "cut_trees")
            );
            Assert.AreEqual(
                "Cut Trees",
                simulation.Query<string, string>(ClientQuery.TaskName, "cut_trees")
            );
        }

        [Test]
        public void TestResearch()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var heap = Helpers.GetDataTypeById<ResearchType>("heap");
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.ResearchAvailable)
            );
            simulation.PerformAction(ClientAction.UnlockPage, Consts.Pages.Research);
            Assert.AreEqual(
                new []{"heap", "writing"},
                simulation.Query<string[]>(ClientQuery.ResearchAvailable)
            );
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.ResearchUnlocked)
            );
            Assert.AreEqual(
                heap.name,
                simulation.Query<string, string>(ClientQuery.ResearchDisplayName, "heap")
            );
            Assert.AreEqual(
                heap.description,
                simulation.Query<string, string>(ClientQuery.ResearchDescription, "heap")
            );
            Assert.AreEqual(
                heap.cost.Keys.ToArray(),
                simulation.Query<string[], string>(ClientQuery.ResearchResourceList, "heap")
            );
            foreach(var cost in heap.cost)
                Assert.AreEqual(
                    cost.Value,
                    simulation.Query<float, string, string>(ClientQuery.ResearchSingleResourceCost, "heap", cost.Key)
                );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.ResearchCanAfford, "heap")
            );
            foreach(var cost in heap.cost)
                simulation.PerformAction(ClientAction.AddResourceToSettlementInventory, settlementUuid, cost.Key, cost.Value);
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.ResearchCanAfford, "heap")
            );
            simulation.PerformAction(ClientAction.UnlockResearch, "heap");
            Assert.AreEqual(
                new []{"writing", "woodshed"},
                simulation.Query<string[]>(ClientQuery.ResearchAvailable)
            );
            Assert.AreEqual(
                new []{"heap"},
                simulation.Query<string[]>(ClientQuery.ResearchUnlocked)
            );
        }

        [Test]
        public void TestForceUnlockResearch()
        {
            simulation.PerformAction(ClientAction.UnlockPage, Consts.Pages.Research);
            Assert.AreEqual(
                new []{"heap", "writing"},
                simulation.Query<string[]>(ClientQuery.ResearchAvailable)
            );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.ResearchCanAfford, "heap")
            );
            simulation.PerformAction(ClientAction.ForceUnlockResearch, "heap");
            Assert.AreEqual(
                new []{"writing", "woodshed"},
                simulation.Query<string[]>(ClientQuery.ResearchAvailable)
            );
            Assert.AreEqual(
                new []{"heap"},
                simulation.Query<string[]>(ClientQuery.ResearchUnlocked)
            );
            simulation.PerformAction(ClientAction.ForceUnlockResearch, "meal_prep");
            Assert.AreEqual(
                new []{"heap", "meal_prep"},
                simulation.Query<string[]>(ClientQuery.ResearchUnlocked)
            );
        }

        [Test]
        public void TestSpecies()
        {
            Assert.AreEqual(
                Helpers.GetSpeciesTypeById("lamia").name,
                simulation.Query<string, string>(ClientQuery.SpeciesName, "lamia")
            );
            Assert.AreEqual(
                Helpers.GetSpeciesTypeById("lamia").description,
                simulation.Query<string[], string>(ClientQuery.SpeciesDescription, "lamia")
            );
        }

        [Test]
        public void TestResources()
        {
            Assert.AreEqual(
                Helpers.GetDataTypeById<ResourceCategory>("food").name,
                simulation.Query<string, string>(ClientQuery.ResourceCategoryName, "food")
            );
            Assert.AreEqual(
                Helpers.GetDataTypeById<ResourceCategory>("food").description,
                simulation.Query<string, string>(ClientQuery.ResourceCategoryDescription, "food")
            );
            Assert.AreEqual(
                Helpers.GetResourceTypeById("ration").name,
                simulation.Query<string, string>(ClientQuery.ResourceName, "ration")
            );
            Assert.AreEqual(
                Helpers.GetResourceTypeById("ration").description,
                simulation.Query<string, string>(ClientQuery.ResourceDescription, "ration")
            );
        }

        [Test]
        public void TestResearchBehaviourUnlockBuilding()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                false,
                simulation.Query<bool, string, string>(ClientQuery.SettlementHasBuildingUnlocked, settlementUuid, "archives")
            );
            simulation.PerformAction(ClientAction.ForceUnlockResearch, "writing");
            Assert.AreEqual(
                true,
                simulation.Query<bool, string, string>(ClientQuery.SettlementHasBuildingUnlocked, settlementUuid, "archives")
            );
        }

        [Test]
        public void TestResearchBehaviourUnlockTask()
        {
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.TaskUnlocked, "cook")
            );
            simulation.PerformAction(ClientAction.ForceUnlockResearch, "meal_prep");
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.TaskUnlocked, "cook")
            );
        }

        [Test]
        public void TestUnlockResearchShowsUnlockMessage()
        {
            simulation.PerformAction(ClientAction.ForceUnlockResearch, "writing");
            Assert.Contains(
                Helpers.GetDataTypeById<ResearchType>("writing").unlockMessage,
                simulation.Query<string[]>(ClientQuery.UnreadMessages)
            );
        }
        
        [Test]
        public void TestUnlockUpgrade()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.UpgradesUnlocked)
            );
            simulation.PerformAction(
                ClientAction.SettlementForceAddBuilding, settlementUuid, "woodshed"
            );
            simulation.PerformAction(
                ClientAction.SettlementForceAddBuilding, settlementUuid, "archives"
            );
            foreach (var (resource, value) in Helpers.GetDataTypeById<UpgradeType>("stone_axe").cost)
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
                ClientAction.UnlockPage, Consts.Pages.Upgrades
            );
            Assert.AreEqual(
                new []{"stone_axe"},
                simulation.Query<string[]>(ClientQuery.UpgradesAvailable)
            );
            simulation.PerformAction(
                ClientAction.UnlockUpgrade, "stone_axe"
            );
            Assert.AreEqual(
                new []{"stone_axe"},
                simulation.Query<string[]>(ClientQuery.UpgradesUnlocked)
            );
            foreach (var (resource, value) in Helpers.GetDataTypeById<UpgradeType>("stone_axe").cost)
                Assert.AreEqual(
                    0f,
                    simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, resource)
                );
        }

        [Test]
        public void TestForceUnlockUpgrade()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.UpgradesUnlocked)
            );
            simulation.PerformAction(
                ClientAction.ForceUnlockUpgrade, "stone_axe"
            );
            Assert.AreEqual(
                new []{"stone_axe"},
                simulation.Query<string[]>(ClientQuery.UpgradesUnlocked)
            );
        }
        
        [Test]
        public void TestUpgradeQuerying()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var stoneAxe = Helpers.GetDataTypeById<UpgradeType>("stone_axe");
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.UpgradesAvailable)
            );
            simulation.PerformAction(ClientAction.UnlockPage, Consts.Pages.Upgrades);
            Assert.AreEqual(
                new []{"stone_axe"},
                simulation.Query<string[]>(ClientQuery.UpgradesAvailable)
            );
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.UpgradesUnlocked)
            );
            Assert.AreEqual(
                "Stone Axe",
                simulation.Query<string, string>(ClientQuery.UpgradeDisplayName, "stone_axe")
            );
            Assert.AreEqual(
                "Strapping a sharp rock to a stick should make it easier to cut trees down.",
                simulation.Query<string, string>(ClientQuery.UpgradeDescription, "stone_axe")
            );
            Assert.AreEqual(
                new []{"research", "logs"},
                simulation.Query<string[], string>(ClientQuery.UpgradeResourceList, "stone_axe")
            );
            Assert.AreEqual(
                stoneAxe.cost["logs"],
                simulation.Query<float, string, string>(ClientQuery.UpgradeSingleResourceCost, "stone_axe", "logs")
            );
            Assert.AreEqual(
                stoneAxe.cost["research"],
                simulation.Query<float, string, string>(ClientQuery.UpgradeSingleResourceCost, "stone_axe", "research")
            );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.UpgradeCanAfford, "stone_axe")
            );
            simulation.PerformAction(ClientAction.SettlementForceAddBuilding, settlementUuid, "archives");
            simulation.PerformAction(ClientAction.SettlementForceAddBuilding, settlementUuid, "woodshed");
            simulation.PerformAction(ClientAction.AddResourceToSettlementInventory, settlementUuid, "logs", stoneAxe.cost["logs"]);
            simulation.PerformAction(ClientAction.AddResourceToSettlementInventory, settlementUuid, "research", stoneAxe.cost["research"]);
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.UpgradeCanAfford, "stone_axe")
            );
            simulation.PerformAction(ClientAction.UnlockUpgrade, "stone_axe");
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.UpgradesAvailable)
            );
            Assert.AreEqual(
                new []{"stone_axe"},
                simulation.Query<string[]>(ClientQuery.UpgradesUnlocked)
            );
        }
        
        [Test]
        public void TestUnlockUpgradesPageWhenGettingFirstArchives()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.HasUnlockedPage, Consts.Pages.Upgrades)
            );
            simulation.PerformAction(ClientAction.SettlementForceAddBuilding, settlementUuid, "archives");
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.HasUnlockedPage, Consts.Pages.Upgrades)
            );
        }
        
        [Test]
        public void TestUnlockUpgradeShowsUnlockMessage()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            simulation.PerformAction(ClientAction.ForceUnlockUpgrade, "stone_axe");
            Assert.Contains(
                Helpers.GetDataTypeById<UpgradeType>("stone_axe").unlockMessage,
                simulation.Query<string[]>(ClientQuery.UnreadMessages)
            );
        }        
    }
}