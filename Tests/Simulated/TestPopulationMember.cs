using System.Linq;
using NUnit.Framework;
using LamiaSimulation;

namespace Tests
{
    public class PopulationMemberTest: BaseTest
    {
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
                "task",
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
            SimulateSeconds(SecondsToMaxInventoryWhenForaging());
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

        [Test]
        public void TestPopulationDepositsResourcesInSettlementInventory()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "forage"
            );
            var secondsToMaxAndDeposit = SecondsToMaxInventoryWhenForaging() + Consts.depositInventoryTime;
            Assert.AreEqual(
                0,
                simulation.Query<string[], string>(ClientQuery.SettlementInventoryCategories, settlementUuid).Length
            );
            Assert.AreEqual(
                0.0f,
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "raw_food")
            );
            SimulateSeconds(secondsToMaxAndDeposit);
            Assert.AreEqual(
                new []{"food"},
                simulation.Query<string[], string>(ClientQuery.SettlementInventoryCategories, settlementUuid)
            );
            Assert.AreEqual(
                new []{"raw_food"},
                simulation.Query<string[], string, string>(ClientQuery.SettlementInventoryResources, settlementUuid, "food")
            );
            var amountToExtract = Helpers.GetDataTypeById<TaskType>("forage").behaviour[0].value;
            Assert.AreEqual(
                amountToExtract * NumberForagesToMaxInventory(),
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "raw_food")
            );
        }

        [Test]
        public void TestPopulationTriesToEatWhenHungerEmpty()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask,
                new ClientParameter<string>(settlementUuid),
                new ClientParameter<string>(populationUuid),
                new ClientParameter<string>("forage")
            );
            var secondsToSimulate =
                System.Math.Ceiling(NumberForagesToHungry() * Helpers.GetDataTypeById<TaskType>("forage").timeToComplete);
            SimulateSeconds((float)secondsToSimulate);
            Assert.AreEqual(
                "forage",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
            Assert.Less(
                simulation.Query<float, string, string>(ClientQuery.PopulationMemberHunger, settlementUuid, populationUuid),
                0.2f
            );
            SimulateSeconds(Consts.populationEatingTime * 2);
            Assert.GreaterOrEqual(
                simulation.Query<float, string, string>(ClientQuery.PopulationMemberHunger, settlementUuid, populationUuid),
                0.3f
            );
        }
       
        [Test]
        public void TestPopulationStarvesWhenHungryAndNoFood()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(ClientAction.UnlockTask, "cut_trees");
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "cut_trees"
            );
            var secondsToSimulate =
                System.Math.Ceiling(NumberCutTreesToHungry() * Helpers.GetDataTypeById<TaskType>("cut_trees").timeToComplete) + (Consts.depositInventoryTime * 2);
            SimulateSeconds((float)secondsToSimulate);
            Assert.AreEqual(
                "cut_trees",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "starving",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Starving!!!",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
        }

        [Test]
        public void TestPopulationDiesWhenStarvingAndTimePasses()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            var populationName =
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberName, settlementUuid, populationUuid);
            simulation.PerformAction(ClientAction.UnlockTask, "cut_trees");
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "cut_trees"
            );
            var secondsToSimulate =
                System.Math.Ceiling(NumberCutTreesToHungry() * Helpers.GetDataTypeById<TaskType>("cut_trees").timeToComplete) +
                (Consts.depositInventoryTime * 2);
            SimulateSeconds((float)secondsToSimulate);
            Assert.AreEqual(
                1,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
            );
            Assert.AreEqual(
                "starving",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            SimulateSeconds(Consts.populationStarvationDeathTime);
            Assert.AreEqual(
                0,
                simulation.Query<int, string>(ClientQuery.SettlementCurrentPopulation, settlementUuid)
            );
            Assert.AreEqual(
                $"{populationName} has starved to death!",
                simulation.Query<string[]>(ClientQuery.UnreadMessages).Last()
            );
        }

        [Test]
        public void TestPopulationStartsEatingWhenStarvingAndFoodArrives()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(ClientAction.UnlockTask, "cut_trees");
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "cut_trees"
            );
            var secondsToSimulate =
                System.Math.Ceiling(NumberCutTreesToHungry() *
                                    Helpers.GetDataTypeById<TaskType>("cut_trees").timeToComplete) +
                (Consts.depositInventoryTime * 2);
            SimulateSeconds((float)secondsToSimulate);
            Assert.AreEqual(
                "cut_trees",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid,
                    populationUuid)
            );
            Assert.AreEqual(
                "starving",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid,
                    populationUuid)
            );
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory, settlementUuid, "raw_food", 10f
            );
            simulation.Simulate(.5f);
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
        }
        
        [Test]
        public void TestPopulationSwitchesBackToTaskAfterFinishingEating()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(ClientAction.UnlockTask, "cut_trees");
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "cut_trees"
            );
            var secondsToSimulate =
                System.Math.Ceiling(NumberCutTreesToHungry() *
                                    Helpers.GetDataTypeById<TaskType>("cut_trees").timeToComplete) +
                (Consts.depositInventoryTime * 2);
            SimulateSeconds((float)secondsToSimulate);
            Assert.AreEqual(
                "cut_trees",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid,
                    populationUuid)
            );
            Assert.AreEqual(
                "starving",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid,
                    populationUuid)
            );
            var numFoodPortionsRequired = System.MathF.Ceiling(1f / Helpers.GetDataTypeById<ResourceType>("raw_food").hungerRecoveryFactor);
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory, settlementUuid, "raw_food", numFoodPortionsRequired
            );
            simulation.Simulate(.01f);
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
            SimulateSeconds(numFoodPortionsRequired * Consts.populationEatingTime + .5f);
            Assert.AreEqual(
                1f,
                simulation.Query<float, string, string>(ClientQuery.PopulationMemberHunger, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "deposit",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Depositing",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
            SimulateSeconds(Consts.depositInventoryTime);
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Cutting trees",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
        }

        [Test]
        public void TestPopulationSwitchesBackToTaskAfterFinishingEatingWithoutFullHunger()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(ClientAction.UnlockTask, "cut_trees");
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask,
                settlementUuid,
                populationUuid,
                "cut_trees"
            );
            var secondsToSimulate =
                System.Math.Ceiling(NumberCutTreesToHungry() *
                                    Helpers.GetDataTypeById<TaskType>("cut_trees").timeToComplete) +
                (Consts.depositInventoryTime * 2);
            SimulateSeconds((float)secondsToSimulate);
            Assert.AreEqual(
                "cut_trees",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid,
                    populationUuid)
            );
            Assert.AreEqual(
                "starving",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid,
                    populationUuid)
            );
            var numFoodPortionsRequired =
                System.MathF.Ceiling(1f / Helpers.GetDataTypeById<ResourceType>("raw_food").hungerRecoveryFactor);
            var numFoodPortionsWereGetting = numFoodPortionsRequired / 2;
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory, settlementUuid, "raw_food", numFoodPortionsWereGetting
            );
            simulation.Simulate(.01f);
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid,
                    populationUuid)
            );
            Assert.AreEqual(
                "eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid,
                    populationUuid)
            );
            Assert.AreEqual(
                "Eating",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid,
                    populationUuid)
            );
            SimulateSeconds(numFoodPortionsWereGetting * Consts.populationEatingTime + 1f);
            Assert.GreaterOrEqual(
                simulation.Query<float, string, string>(ClientQuery.PopulationMemberHunger, settlementUuid, populationUuid),
                .4f
            );
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Cutting trees",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
        }

        [Test]
        public void TestPopulationWaitsIfDestinationInventoryIsFull()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory, 
                settlementUuid, 
                "raw_food",
                Consts.InitialSettlementResourceCapacity["raw_food"]
            );
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "forage"
            );
            SimulateSeconds(SecondsToMaxInventoryWhenForaging());
            Assert.AreEqual(
                "wait",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Waiting",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "No space left to store resource.",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberWaitMessage, settlementUuid, populationUuid)
            );
        }

        [Test]
        public void TestPopulationWillSwitchToTaskWhenSpaceBecomesAvailable()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "forage"
            );
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory,
                settlementUuid,
                "raw_food",
                Consts.InitialSettlementResourceCapacity["raw_food"]
            );
            SimulateSeconds(.1f);
            Assert.AreEqual(
                "wait",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            simulation.PerformAction(
                ClientAction.SubtractResourceFromSettlementInventory,
                settlementUuid,
                "raw_food",
                Consts.InitialSettlementResourceCapacity["raw_food"]
            );
            SimulateSeconds(.5f);
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
        }

        [Test]
        public void TestPopulationDoesResearchAndAddsToDestinationInventory()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(ClientAction.UnlockTask, "research");
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "research"
            );
            Assert.AreEqual(
                0f,
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "research")
            );
            SimulateSeconds(Helpers.GetDataTypeById<TaskType>("research").timeToComplete);
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "research",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Thinking",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );            
            Assert.AreEqual(
                Helpers.GetDataTypeById<TaskType>("research").behaviour[0].value,
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "research")
            );
        }

        [Test]
        public void TestPopulationWaitsIfLocationOutOfResources()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var locationUuid = simulation.Query<string, string>(ClientQuery.SettlementLocation, settlementUuid);
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(
                ClientAction.SubtractResourceFromLocation,
                new ClientParameter<string>(locationUuid),
                new ClientParameter<string>("raw_food"),
                new ClientParameter<float>((float)Helpers.GetDataTypeById<LocationType>("origin").resources["raw_food"])
            );
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask,
                new ClientParameter<string>(settlementUuid),
                new ClientParameter<string>(populationUuid),
                new ClientParameter<string>("forage")
            );
            SimulateSeconds(.5f);
            Assert.AreEqual(
                "wait",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Waiting",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "No resource remaining at location.",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberWaitMessage, settlementUuid, populationUuid)
            );
        }

        [Test]
        public void TestPopulationCraftsItemsFromOthers()
        {
            var settlementUuid = simulation.Query<string[]>(ClientQuery.Settlements)[0];
            var populationUuid =
                simulation.Query<string[], string>(ClientQuery.SettlementPopulationMembers, settlementUuid)[0];
            simulation.PerformAction(ClientAction.UnlockTask, "cook");
            simulation.PerformAction(ClientAction.SettlementUnlockBuilding, settlementUuid, "stockpile");
            for (var i = 0; i <= 2; i++)
            {
                simulation.PerformAction(
                    ClientAction.AddResourceToSettlementInventory, settlementUuid, "logs", 20f
                );
                simulation.PerformAction(
                    ClientAction.AddResourceToSettlementInventory, settlementUuid, "raw_food", 40f
                );
                simulation.PerformAction(
                    ClientAction.SettlementPurchaseBuilding, settlementUuid, "stockpile"
                );
            }
            simulation.PerformAction(ClientAction.SettlementUnlockBuilding, settlementUuid, "pantry");
            var pantry = Helpers.GetDataTypeById<BuildingType>("pantry");
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory, settlementUuid, "logs", pantry.cost["logs"]
            );
            simulation.PerformAction(
                ClientAction.AddResourceToSettlementInventory, settlementUuid, "raw_food", pantry.cost["raw_food"]
            );
            simulation.PerformAction(ClientAction.SettlementPurchaseBuilding, settlementUuid, "pantry");
            simulation.PerformAction(
                ClientAction.PopulationAssignToTask, settlementUuid, populationUuid, "cook"
            );
            SimulateSeconds(.1f);
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "task",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid)
            );
            Assert.AreEqual(
                "Cooking",
                simulation.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid)
            );
            var currentFood =
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "raw_food");
            SimulateSeconds(Helpers.GetDataTypeById<TaskType>("cook").timeToComplete);
            Assert.AreEqual(
                currentFood - Helpers.GetDataTypeById<TaskType>("cook").behaviour[0].value,
                simulation.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, settlementUuid, "raw_food")
            );
            Assert.AreEqual(
                Helpers.GetDataTypeById<TaskType>("cook").behaviour[1].value,
                simulation.Query<float, string, string, string>(ClientQuery.PopulationMemberInventoryResourceAmount, settlementUuid, populationUuid,"ration")
            );
        }
    }
}