using NUnit.Framework;
using LamiaSimulation;

namespace Tests
{
    public class PopulationMemberTest
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
        public void TestAssigningPopulationToTaskSwitchesTask()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid = simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask,
                new ClientParameter<string>(settlementUuid),
                new ClientParameter<string>(populationUuid), 
                new ClientParameter<string>("forage")
                );
            Assert.AreEqual(
                "forage",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "extract",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Foraging",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
                );
        }

        [Test]
        public void TestPopulationSwitchesToDepositWhenInventoryFull()
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
            Assert.AreEqual(
                "forage",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "deposit",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Depositing",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
            
        }
    }
}