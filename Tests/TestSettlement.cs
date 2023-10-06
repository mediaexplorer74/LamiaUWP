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
            simulation = new Simulation();
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
            var populationUuid = simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask,
                new ClientParameter<string>(settlementUuid),
                new ClientParameter<string>(populationUuid), 
                new ClientParameter<string>("forage")
            );
            for (var i = 0; i <= 17; i++)
                simulation.Simulate(.5f);
            
        }
    }
}