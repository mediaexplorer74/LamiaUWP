using System;
using System.Collections.Generic;
using System.Linq;

namespace LamiaSimulation
{
    [Serializable]
    internal class Settlement: SimulationObject, IActionReceiver, IQueryable, ISimulated
    {
        private string name;
        private List<PopulationMember> populationMembers;
        private int maxPopulationMember = Consts.InitialSettlementPopulationCapacity;
        private List<string> availableTasks;
        private Dictionary<ResourceType, float> inventory;
        private Dictionary<ResourceType, float> inventoryMemory;
        private Dictionary<ResourceType, float> inventoryDelta;
        private float inventoryMemoryTime;
        private List<PopulationMember> populationToRemove;
        private float spawnTimer;
        private bool spawnEnabled;
        
        public string locationUuid;
        public static string simulatingSettlement;

        public Settlement(string name, string locationUuid)
        {
            inventory = new Dictionary<ResourceType, float>();
            inventoryMemory = new Dictionary<ResourceType, float>();
            inventoryDelta = new Dictionary<ResourceType, float>();
            inventoryMemoryTime = 1.0f;
            populationMembers = new List<PopulationMember>();
            availableTasks = new List<string>();
            populationToRemove = new List<PopulationMember>();
            spawnTimer = Consts.populationSpawnTime;
            spawnEnabled = false;
            this.name = name;
            this.locationUuid = locationUuid;
            UnlockTask("idle");
            UnlockTask("forage");
        }

        // ---------------------------------------------------
        // IActionReceiver
        // ---------------------------------------------------

        public void PerformAction(ClientAction action)
        {
            foreach(var pop in populationMembers)
                pop.PerformAction(action);
        }

        // param1 is always settlement ID
        public void PerformAction<T>(ClientAction action, ClientParameter<T> param1)
        {
            if(param1.Get as string != ID)
                return;
            switch (action)
            {
                // Take next available food
                case ClientAction.SettlementTakeAvailableFoodPortion:
                    TakeNextAvailableFoodPortion();
                    break;
            }
            foreach(var pop in populationMembers)
                pop.PerformAction(action, param1);
        }

        public void PerformAction<T1, T2>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
            if(param1.Get as string != ID)
                return;
            switch(action)
            {
                // Unlock task
                case ClientAction.UnlockTask:
                    UnlockTask(param2.Get as string);
                    break;
                // Rename
                case ClientAction.RenameSettlement:
                    name = param2.Get as string;
                    break;
                // Add population member
                case ClientAction.AddPopulation:
                    var newMember = new PopulationMember(param2.Get as string, ID, locationUuid);
                    AddToPopulation(newMember);
                    break;
                // Remove population from settlement
                case ClientAction.SettlementRemovePopulation:
                    populationToRemove.Add(GetPopulationMemberById(param2.Get as string));
                    break;
            }
            foreach(var pop in populationMembers)
                pop.PerformAction(action, param1, param2);
        }

        public void PerformAction<T1, T2, T3>(ClientAction action, ClientParameter<T1> param1,
            ClientParameter<T2> param2, ClientParameter<T3> param3)
        {
            if(param1.Get as string != ID)
                return;
            switch(action)
            {
                // Assign pop to task
                case ClientAction.PopulationAssignToTask:
                    var taskToAssignTo = param3.Get as string;
                    if(DataQuery<TaskType>.GetByID(taskToAssignTo) == null)
                        throw new ClientActionException(T._("Task does not exist."));
                    if(!availableTasks.Contains(taskToAssignTo))
                        throw new ClientActionException(T._("Task not unlocked in settlement."));
                    break;
                // Add resource to inventory
                case ClientAction.AddResourceToSettlementInventory:
                    var resource = Helpers.GetResourceTypeById(param2.Get as string);
                    var amount = param3.Coerce<float>();
                    inventory.TryAdd(resource, 0.0f);
                    inventory[resource] += amount;
                    var cap = GetInventoryResourceCapacity(resource.ID);
                    if (inventory[resource] > cap)
                        inventory[resource] = cap;
                    break;
            }

            foreach(var pop in populationMembers)
                pop.PerformAction(action, param1, param2, param3);
        }

        // ---------------------------------------------------
        // IQueryable
        // ---------------------------------------------------

        public void Query<T>(ref QueryResult<T> result, ClientQuery query)
        {
            foreach(var pop in populationMembers)
                pop.Query(ref result, query);
        }

        // Param 1 is always the settlement ID
        public void Query<T, T1>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1)
        {
            if(param1.Get as string != ID)
                return;
            switch(query)
            {
                // Settlement location
                case ClientQuery.SettlementLocation:
                    result = new QueryResult<string>(locationUuid) as QueryResult<T>;
                    break;
                // Settlement name
                case ClientQuery.SettlementName:
                    result = new QueryResult<string>(name) as QueryResult<T>;
                    break;
                // Population num
                case ClientQuery.SettlementCurrentPopulation:
                    result = new QueryResult<int>(GetNumPopulation()) as QueryResult<T>;
                    break;
                // Max population
                case ClientQuery.SettlementPopulationMax:
                    result = new QueryResult<int>(GetPopulationMemberCapacity()) as QueryResult<T>;
                    break;
                // Population species
                case ClientQuery.SettlementPopulationSpecies:
                    result = new QueryResult<string[]>(GetSettlementPopulationSpecies()) as QueryResult<T>;
                    break;
                // Population members
                case ClientQuery.SettlementPopulationMembers:
                    result = new QueryResult<string[]>(GetSettlementPopulationMembers()) as QueryResult<T>;
                    break;
                // Available tasks
                case ClientQuery.SettlementTasks:
                    result = new QueryResult<string[]>(GetAvailableTasks()) as QueryResult<T>;
                    break;
                // Inventory resource list
                case ClientQuery.SettlementInventory:
                    result = new QueryResult<string[]>(GetInventoryList()) as QueryResult<T>;
                    break;
                // Next available food portion 
                case ClientQuery.SettlementAvailableFoodPortion:
                    result = new QueryResult<float>(GetNextAvailableFoodPortion().hungerRecoveryFactor) as QueryResult<T>;
                    break;
                
            }
            foreach(var pop in populationMembers)
                pop.Query(ref result, query, param1);
        }

        public void Query<T, T1, T2>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2)
        {
            if(param1.Get as string != ID)
                return;
            switch(query)
            {
                // Task names
                case ClientQuery.SettlementTaskName:
                    result = new QueryResult<string>(GetTaskName(param2.Get as string)) as QueryResult<T>;
                    break;
                // Is task unlocked
                case ClientQuery.SettlementTaskUnlocked:
                    result = new QueryResult<bool>(HasTaskUnlocked(param2.Get as string)) as QueryResult<T>;
                    break;
                // Task descriptions
                case ClientQuery.SettlementTaskDescription:
                    result = new QueryResult<string[]>(GetTaskDescription(param2.Get as string)) as QueryResult<T>;
                    break;
                // Task assignment num
                case ClientQuery.SettlementTaskAssignedNum:
                    result = new QueryResult<int>(GetTaskAssignedNum(param2.Get as string)) as QueryResult<T>;
                    break;
                // Task maximum capacity
                case ClientQuery.SettlementTaskMaximumCapacity:
                    result = new QueryResult<int>(GetTaskMaximumCapacity(param2.Get as string)) as QueryResult<T>;
                    break;
                // Task assignments
                case ClientQuery.SettlementTaskAssignments:
                    result = new QueryResult<string[]>(GetTaskAssignments(param2.Get as string)) as QueryResult<T>;
                    break;
                // Population members by species
                case ClientQuery.SettlementPopulationSpeciesMembers:
                    result = new QueryResult<string[]>(GetSettlementPopulationSpeciesMembers(param2.Get as string)) as QueryResult<T>;
                    break;
                // Population member queries
                case ClientQuery.PopulationMemberName:
                case ClientQuery.PopulationMemberSpecies:
                case ClientQuery.PopulationMemberTask:
                    var pop = GetPopulationMemberById(param2.Get as string);
                    if(pop == null)
                        return;
                    if(query == ClientQuery.PopulationMemberName)
                        result = new QueryResult<string>(pop.name) as QueryResult<T>;
                    else if(query == ClientQuery.PopulationMemberSpecies)
                        result = new QueryResult<string>(pop.species.ID) as QueryResult<T>;
                    else if(query == ClientQuery.PopulationMemberTask)
                        result = new QueryResult<string>(pop.taskAssigment) as QueryResult<T>;
                    break;
                // Inventory resource amount
                case ClientQuery.SettlementInventoryResourceAmount:
                    result = new QueryResult<float>(GetInventoryResourceAmount(param2.Get as string)) as QueryResult<T>;
                    break;
                // Inventory resource maximum amount
                case ClientQuery.SettlementInventoryResourceCapacity:
                    result = new QueryResult<float>(GetInventoryResourceCapacity(param2.Get as string)) as QueryResult<T>;
                    break;
                // Inventory resource delta
                case ClientQuery.SettlementInventoryResourceDelta:
                    result = new QueryResult<float>(GetInventoryResourceDelta(param2.Get as string)) as QueryResult<T>;
                    break;
            }
            foreach(var pop in populationMembers)
                pop.Query(ref result, query, param1, param2);
        }

        public void Query<T, T1, T2, T3>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2, ClientParameter<T3> param3)
        {
            foreach(var pop in populationMembers)
                pop.Query(ref result, query, param1, param2, param3);
        }

        // ---------------------------------------------------
        // ISimulated
        // ---------------------------------------------------

        public void Simulate(float deltaTime)
        {
            simulatingSettlement = ID;
            // Task unlocks
            if (!HasTaskUnlocked("cut_trees"))
            {
                if (inventory.ContainsKey(Helpers.GetResourceTypeById("raw_food")))
                {
                    UnlockTask("cut_trees");
                    Simulation.Instance.PerformAction(
                        ClientAction.SendMessage,
                        new ClientParameter<string>(T._("With food available, it's time to find some better building materials."))
                    );
                    Simulation.Instance.PerformAction(
                        ClientAction.SendMessage,
                        new ClientParameter<string>(T._("The trees around here will take some work to fell, but they are the only option."))
                        );
                    spawnEnabled = true;
                }
            }
            // Spawning population
            if (spawnEnabled && GetNumPopulation() < GetPopulationMemberCapacity())
            {
                spawnTimer -= deltaTime;
                if (spawnTimer <= 0f)
                {
                    var newMember = new PopulationMember("lamia", ID, locationUuid);
                    AddToPopulation(newMember);
                    Simulation.Instance.PerformAction(
                        ClientAction.SendMessage,
                        new ClientParameter<string>(
                            string.Format(T._("{0} has joined your settlement."), newMember.name)
                        )
                    );
                    spawnTimer = Consts.populationSpawnTime;
                }
            }
            // Simulate pop
            populationMembers.Apply(pop => pop.Simulate(deltaTime));
            foreach(var pop in populationToRemove)
                RemoveFromPopulation(pop);
            populationToRemove.Clear();
            // Determine inventory deltas
            inventoryMemoryTime -= deltaTime;
            if (inventoryMemoryTime <= 0f)
            {
                inventoryMemoryTime = 1.0f;
                foreach (var resource in inventory)
                {
                    if (inventoryMemory.ContainsKey(resource.Key))
                    {
                        inventoryDelta[resource.Key] = resource.Value - inventoryMemory[resource.Key];
                        inventoryMemory[resource.Key] = resource.Value;
                    }
                    else
                    {
                        inventoryMemory[resource.Key] = resource.Value;
                        inventoryDelta[resource.Key] = resource.Value;
                    }
                }
            }
        }

        // ---------------------------------------------------
        // Action behaviours
        // ---------------------------------------------------

        private void AddToPopulation(PopulationMember newMember)
        {
            if(GetNumPopulation() >= GetPopulationMemberCapacity())
                throw new ClientActionException(T._("Population at capacity."));
            populationMembers.Add(newMember);
        }

        private void RemoveFromPopulation(PopulationMember population)
        {
            populationMembers.Remove(population);
        }

        private void UnlockTask(string taskID)
        {
            if(HasTaskUnlocked(taskID))
                throw new ClientActionException(T._("Task already unlocked."));
            if(DataQuery<TaskType>.GetByID(taskID) == null)
                throw new ClientActionException(T._("Task does not exist."));
            availableTasks.Add(taskID);
        }

        // ---------------------------------------------------
        // Query behaviours
        // ---------------------------------------------------

        private bool HasTaskUnlocked(string taskID)
        {
            return availableTasks.Contains(taskID);
        }
        
        private int GetNumPopulation()
        {
            return populationMembers.Count;
        }

        private int GetPopulationMemberCapacity()
        {
            return maxPopulationMember;
        }

        private string[] GetSettlementPopulationSpecies()
        {
            var speciesID = populationMembers.Select(pop => pop.species.ID).Distinct().ToList();
            return speciesID.ToArray();
        }

        private string[] GetSettlementPopulationMembers()
        {
            var popIDs = populationMembers.Select(pop => pop.ID).ToList();
            return popIDs.ToArray();
        }

        private string[] GetSettlementPopulationSpeciesMembers(string speciesID)
        {
            var popIDs = populationMembers.Filter(pop => pop.species.ID == speciesID).Select(pop => pop.ID).ToList();
            return popIDs.ToArray();
        }

        private string[] GetAvailableTasks()
        {
            return availableTasks.ToArray();
        }

        private string[] GetTaskAssignments(string taskId)
        {
            var popIDs = populationMembers.Filter(pop => pop.taskAssigment == taskId).Select(pop => pop.ID).ToList();
            return popIDs.ToArray();
        }

        private TaskType GetTaskById(string taskId)
        {
            var filtered = DataQuery<TaskType>.GetByID(taskId);
            if(filtered == null)
                throw new ClientActionException(T._("Task does not exist."));
            if(!availableTasks.Contains(taskId))
                throw new ClientActionException(T._("Task not unlocked."));
            return filtered;
        }

        private string GetTaskName(string taskID)
        {
            return T._(GetTaskById(taskID).name);
        }

        private string[] GetTaskDescription(string taskID)
        {
            return GetTaskById(taskID).GetDescriptionDisplay();
        }

        private int GetTaskAssignedNum(string taskID)
        {
            return populationMembers.Filter(pop => pop.taskAssigment == taskID).Count();
        }

        private int GetTaskMaximumCapacity(string taskID)
        {
            return -1;
        }

        private PopulationMember GetPopulationMemberById(string populationMemberID)
        {
            var filtered = populationMembers.Filter(pop => pop.ID == populationMemberID);
            if(!filtered.Any())
                return null;
            return filtered.First();
        }

        private string[] GetInventoryList()
        {
            var resources = inventory.Keys.Select(resource => resource.ID).Distinct().ToList();
            return resources.ToArray();
        }

        private float GetInventoryResourceAmount(string resourceId)
        {
            var resource = Helpers.GetResourceTypeById(resourceId);
            return !inventory.ContainsKey(resource) ? 0f : inventory[resource];
        }

        private float GetInventoryResourceCapacity(string resourceId)
        {
            return Consts.InitialSettlementResourceCapacity;
        }
        
        private float GetInventoryResourceDelta(string resourceId)
        {
            var resource = Helpers.GetResourceTypeById(resourceId);
            return !inventoryDelta.ContainsKey(resource) ? 0f : inventoryDelta[resource];
        }

        private (ResourceType resourceType, float hungerRecoveryFactor) GetNextAvailableFoodPortion()
        {
            var highestFactor = 0f;
            ResourceType highestFood = null;
            foreach (var inventoryItem in inventory)
            {
                if (inventoryItem.Key.hungerRecoveryFactor > highestFactor && inventoryItem.Value > 0f)
                {
                    highestFactor = inventoryItem.Key.hungerRecoveryFactor;
                    highestFood = inventoryItem.Key;
                }
            }
            return (highestFood, highestFactor);
        }

        private void TakeNextAvailableFoodPortion()
        {
            var nextAvailableFood = GetNextAvailableFoodPortion();
            if(nextAvailableFood.resourceType == null)
                throw new ClientActionException(T._("No food available to take."));
            inventory[nextAvailableFood.resourceType] -= 1f;
        }
    }
}