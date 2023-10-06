using NUnit.Framework;
using LamiaSimulation;

namespace Tests
{
    public class SimulationTest
    {
        private Simulation simulation;

        [SetUp]
        public void Setup()
        {
            simulation = new Simulation();
            simulation.Start();
        }

        [Test]
        public void TestSimulationBeginsWithSettlement()
        {
            var settlements = simulation.Query<string[]>(ClientQuery.Settlements);
            Assert.IsNotEmpty(settlements);
            Assert.AreEqual(settlements.Length, 1);
        }
        
        [Test]
        public void TestSimulationBeginsWithLamiaPopulationSpecies()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var species = simulation.Query<string[], string>(ClientQuery.SettlementPopulationSpecies, settlementUuid);
            Assert.IsNotEmpty(species);
            Assert.AreEqual(species.Length, 1);
            Assert.AreEqual(species[0], "lamia");
        }

        [Test]
        public void TestSimulationBeginsWithOneLamiaPopulation()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var population = simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid);
            Assert.IsNotEmpty(population);
            Assert.AreEqual(population.Length, 1);
            var species = simulation.Query<string, string, string>(
                ClientQuery.PopulationMemberSpecies, settlementUuid, population[0]
                );
            Assert.AreEqual(species, "lamia");
        }

    }
}