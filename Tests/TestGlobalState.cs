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
        public void TestResearch()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var stockpile = Helpers.GetDataTypeById<ResearchType>("stockpile");
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.ResearchAvailable)
            );
            simulation.PerformAction(ClientAction.UnlockPage, Consts.Pages.Research);
            Assert.AreEqual(
                new []{"stockpile", "writing"},
                simulation.Query<string[]>(ClientQuery.ResearchAvailable)
            );
            Assert.AreEqual(
                new string[]{},
                simulation.Query<string[]>(ClientQuery.ResearchUnlocked)
            );
            Assert.AreEqual(
                stockpile.name,
                simulation.Query<string, string>(ClientQuery.ResearchDisplayName, "stockpile")
            );
            Assert.AreEqual(
                stockpile.description,
                simulation.Query<string, string>(ClientQuery.ResearchDescription, "stockpile")
            );
            Assert.AreEqual(
                stockpile.cost.Keys.ToArray(),
                simulation.Query<string[], string>(ClientQuery.ResearchResourceList, "stockpile")
            );
            foreach(var cost in stockpile.cost)
                Assert.AreEqual(
                    cost.Value,
                    simulation.Query<float, string, string>(ClientQuery.ResearchSingleResourceCost, "stockpile", cost.Key)
                );
            Assert.AreEqual(
                false,
                simulation.Query<bool, string>(ClientQuery.ResearchCanAfford, "stockpile")
            );
            foreach(var cost in stockpile.cost)
                simulation.PerformAction(ClientAction.AddResourceToSettlementInventory, settlementUuid, cost.Key, cost.Value);
            Assert.AreEqual(
                true,
                simulation.Query<bool, string>(ClientQuery.ResearchCanAfford, "stockpile")
            );
            simulation.PerformAction(ClientAction.UnlockResearch, "stockpile");
            Assert.AreEqual(
                new []{"writing", "warehouse"},
                simulation.Query<string[]>(ClientQuery.ResearchAvailable)
            );
            Assert.AreEqual(
                new []{"stockpile"},
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
    }
}