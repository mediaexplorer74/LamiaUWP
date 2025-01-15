using System;
using System.Collections.Generic;
using System.Linq;


namespace LamiaSimulation
{
    using Text = T;

    internal class Settlement: SimulationObject, IActionReceiver, IQueryable, ISimulated
    {
        public string name { get; set; }
        public List<PopulationMember> populationMembers { get; set; }
        public int maxPopulationMember { get; set; }
        public Dictionary<string, int> buildings { get; set; }
        public List<string> availableBuildings { get; set; }
        public Dictionary<string, float> inventory { get; set; }
        public Dictionary<string, float> inventoryMemory { get; set; }
        public Dictionary<string, float> inventoryDelta { get; set; }
        public float inventoryMemoryTime { get; set; }
        public Dictionary<string, float> resourceCapacity { get; set; }
        public float spawnTimer { get; set; }
        public bool spawnEnabled { get; set; }
        public string locationUuid { get; set; }
        
        public List<PopulationMember> populationToRemove;
        public static string simulatingSettlement;

        public Settlement()
        {
            populationToRemove = new List<PopulationMember>();
            PostInitSetup();
        }
        
        public Settlement(string name, string locationUuid)
        {
            maxPopulationMember = Consts.InitialSettlementPopulationCapacity;
            buildings = new Dictionary<string, int>();
            availableBuildings = new List<string>();
            inventory = new Dictionary<string, float>();
            inventoryMemory = new Dictionary<string, float>();
            inventoryDelta = new Dictionary<string, float>();
            inventoryMemoryTime = 1.0f;
            resourceCapacity = new Dictionary<string, float>(Consts.InitialSettlementResourceCapacity);
            populationMembers = new List<PopulationMember>();
            populationToRemove = new List<PopulationMember>();
            spawnTimer = Consts.populationSpawnTime;
            spawnEnabled = false;
            this.name = name;
            this.locationUuid = locationUuid;
            PostInitSetup();
        }

        ~Settlement()
        {
            Simulation.Instance.events.SettlementHasNewResourceEvent -= OnSettlementHasNewResourceHandler;
            Simulation.Instance.events.SettlementSpawnedNewPopulationEvent -= OnSettlementSpawnedNewPopulationHandler;
            Simulation.Instance.events.SettlementBuildingPurchasedEvent -= OnSettlementBuildingPurchasedHandler;
        }

        private void SetUpEventHandlers()
        {
            Simulation.Instance.events.SettlementHasNewResourceEvent += OnSettlementHasNewResourceHandler;
            Simulation.Instance.events.SettlementSpawnedNewPopulationEvent += OnSettlementSpawnedNewPopulationHandler;
            Simulation.Instance.events.SettlementBuildingPurchasedEvent += OnSettlementBuildingPurchasedHandler;
        }

        private void PostInitSetup()
        {
            SetUpEventHandlers();
            if (Simulation.Instance.Query<bool, string>(ClientQuery.TaskUnlocked, "cut_trees"))
                spawnEnabled = true;
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
                // Unlock building
                case ClientAction.SettlementUnlockBuilding:
                    UnlockBuilding(param2.Get as string);
                    break;
                // Purchase building
                case ClientAction.SettlementPurchaseBuilding:
                    PurchaseBuilding(param2.Get as string);
                    break;
                // Force add building
                case ClientAction.SettlementForceAddBuilding:
                    ForceAddBuilding(param2.Get as string);
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
                    if(!Simulation.Instance.Query<bool, string>(ClientQuery.TaskUnlocked, taskToAssignTo))
                        throw new ClientActionException(T._("Task not unlocked."));
                    break;
                // Add resource to inventory
                case ClientAction.AddResourceToSettlementInventory:
                    var resourceId = param2.Get as string;
                    if(Helpers.GetResourceTypeById(resourceId) == null)
                        throw new ClientActionException(T._("Resource does not exist."));
                    var amount = param3.Coerce<float>();
                    var newInInventory = !inventory.ContainsKey(resourceId);
                    inventory.TryAdd(resourceId, 0.0f);
                    inventory[resourceId] += amount;
                    var cap = GetInventoryResourceCapacity(resourceId);
                    if (inventory[resourceId] > cap)
                        inventory[resourceId] = cap;
                    // Fire event if new
                    if (newInInventory)
                    {
                        Simulation.Instance.events.OnSettlementHasNewResource(
                            new SettlementHasNewResourceEventArgs
                            {
                                SettlementUuid = ID,
                                ResourceId = resourceId
                            }
                        );
                    }
                    break;
                // Remove resource from inventory
                case ClientAction.SubtractResourceFromSettlementInventory:
                    var subtractResourceId = param2.Get as string;
                    if(Helpers.GetResourceTypeById(subtractResourceId) == null)
                        throw new ClientActionException(T._("Resource does not exist."));
                    if (!inventory.ContainsKey(subtractResourceId))
                        break;
                    var subtractAmount = Math.Min(inventory[subtractResourceId], param3.Coerce<float>());
                    inventory[subtractResourceId] -= subtractAmount;
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
                // Inventory resource categories
                case ClientQuery.SettlementInventoryCategories:
                    result = new QueryResult<string[]>(GetInventoryCategoryList()) as QueryResult<T>;
                    break;
                // Next available food portion 
                case ClientQuery.SettlementAvailableFoodPortion:
                    result = new QueryResult<float>(GetNextAvailableFoodPortion().hungerRecoveryFactor) as QueryResult<T>;
                    break;
                // Buildings list
                case ClientQuery.SettlementBuildings:
                    result = new QueryResult<string[]>(GetBuildingsList()) as QueryResult<T>;
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
                        result = new QueryResult<string>(pop.populationSpeciesTypeName) as QueryResult<T>;
                    else if(query == ClientQuery.PopulationMemberTask)
                        result = new QueryResult<string>(pop.taskAssigment) as QueryResult<T>;
                    break;
                // Inventory resource list by category
                case ClientQuery.SettlementInventoryResources:
                    result = new QueryResult<string[]>(GetInventoryListByCategory(param2.Get as string)) as QueryResult<T>;
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
                // Building amount
                case ClientQuery.SettlementBuildingsAmount:
                    result = new QueryResult<int>(GetBuildingAmount(param2.Get as string)) as QueryResult<T>;
                    break;
                // Building unlocked
                case ClientQuery.SettlementHasBuildingUnlocked:
                    result = new QueryResult<bool>(HasBuildingUnlocked(param2.Get as string)) as QueryResult<T>;
                    break;
                // Building names
                case ClientQuery.SettlementBuildingDisplayName:
                    result = new QueryResult<string>(GetBuildingDisplayName(param2.Get as string)) as QueryResult<T>;
                    break;
                // Building descriptions
                case ClientQuery.SettlementBuildingDescription:
                    result = new QueryResult<string[]>(GetBuildingDescription(param2.Get as string)) as QueryResult<T>;
                    break;
                // Building can afford
                case ClientQuery.SettlementBuildingCanAfford:
                    result = new QueryResult<bool>(GetBuildingCanAfford(param2.Get as string)) as QueryResult<T>;
                    break;
                // Resources required for a building    
                case ClientQuery.SettlementBuildingResourceList:                
                    result = new QueryResult<string[]>(GetBuildingResourceList(param2.Get as string)) as QueryResult<T>;
                    break;
            }
            foreach(var pop in populationMembers)
                pop.Query(ref result, query, param1, param2);
        }

        public void Query<T, T1, T2, T3>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1,
            ClientParameter<T2> param2, ClientParameter<T3> param3)
        {
            if(param1.Get as string != ID)
                return;
            switch (query)
            {
                // Single resource cost required for a building    
                case ClientQuery.SettlementBuildingSingleResourceCost:
                    result = new QueryResult<float>(GetBuildingSingleResourceCost(param2.Get as string, param3.Get as string)) as QueryResult<T>;
                    break;
            }
            foreach(var pop in populationMembers)
                pop.Query(ref result, query, param1, param2, param3);
        }

        // ---------------------------------------------------
        // ISimulated
        // ---------------------------------------------------

        public void Simulate(float deltaTime)
        {
            simulatingSettlement = ID;

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
                            string.Format(T._("{0} has joined the settlement."), newMember.name)
                        )
                    );
                    spawnTimer = Consts.populationSpawnTime;
                    // Fire event
                    Simulation.Instance.events.OnSettlementSpawnedNewPopulationEvent(
                        new SettlementSpawnedNewPopulationEventArgs
                        {
                            SettlementUuid = ID,
                            LocationUuid = locationUuid,
                            PopulationName = newMember.name,
                            SpeciesId = newMember.populationSpeciesTypeName
                        }
                    );
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

        public void LoadedFromSave()
        {
            RecalculatePopulationLimits();
            RecalculateResourceLimits();
            populationMembers.Apply(pop => pop.LoadedFromSave());
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
        
        private void TakeNextAvailableFoodPortion()
        {
            var nextAvailableFood = GetNextAvailableFoodPortion();
            if(nextAvailableFood.resourceType == null)
                throw new ClientActionException(T._("No food available to take."));
            inventory[nextAvailableFood.resourceType.ID] -= 1f;
        }
        
        private void UnlockBuilding(string buildingID)
        {
            if(HasBuildingUnlocked(buildingID))
                throw new ClientActionException(T._("Building already unlocked."));
            var buildingType = DataQuery<BuildingType>.GetByID(buildingID);
            if(buildingType == null)
                throw new ClientActionException(T._("Building type does not exist."));
            availableBuildings.Add(buildingID);
            buildings[buildingID] = 0;
        }

        private void DoPurchaseBuiding(string buildingID)
        {
            buildings.TryAdd(buildingID, 0);
            buildings[buildingID]++;
            RecalculatePopulationLimits();
            RecalculateResourceLimits();
            // Fire event
            Simulation.Instance.events.OnBuildingPurchased(
                new SettlementBuildingPurchasedEventArgs
                {
                    BuildingId = buildingID,
                    SettlementUuid = ID
                }
            );
        }
        
        private void PurchaseBuilding(string buildingID)
        {
            if (!HasBuildingUnlocked(buildingID))
                throw new ClientActionException(T._("Building not unlocked yet."));
            var buildingType = DataQuery<BuildingType>.GetByID(buildingID);
            if(buildingType == null)
                throw new ClientActionException(T._("Building type does not exist."));
            if(!GetBuildingCanAfford(buildingID))
                throw new ClientActionException(T._("Cannot afford building."));
            foreach (var resourceCost in GetBuildingCost(buildingID))
            {
                if (inventory.ContainsKey(resourceCost.Key))
                    inventory[resourceCost.Key] -= resourceCost.Value;
            }
            DoPurchaseBuiding(buildingID);
        }

        private void ForceAddBuilding(string buildingID)
        {
            var buildingType = DataQuery<BuildingType>.GetByID(buildingID);
            if(buildingType == null)
                throw new ClientActionException(T._("Building type does not exist."));
            DoPurchaseBuiding(buildingID);
        }
        
        private void RecalculatePopulationLimits()
        {
            var popLimit = Consts.InitialSettlementPopulationCapacity;
            foreach (var building in buildings)
            {
                var buildingType = Helpers.GetDataTypeById<BuildingType>(building.Key);
                if (buildingType.behaviour.populationCapacity == 0)
                    continue;
                popLimit += buildingType.behaviour.populationCapacity * building.Value;
            }
            maxPopulationMember = popLimit;
        }

        private void RecalculateResourceLimits()
        {
            var allResources = DataQuery<ResourceType>.GetAll();
            foreach (var resourceId in allResources.Keys)
            {
                resourceCapacity[resourceId] = 0f;
                if(Consts.InitialSettlementResourceCapacity.ContainsKey(resourceId))
                    resourceCapacity[resourceId] = Consts.InitialSettlementResourceCapacity[resourceId];
                foreach (var building in buildings)
                {
                    var buildingType = Helpers.GetDataTypeById<BuildingType>(building.Key);
                    if (buildingType.behaviour.storageCapacity == null || buildingType.behaviour.storageCapacity.Count < 0 || !buildingType.behaviour.storageCapacity.ContainsKey(resourceId))
                        continue;
                    resourceCapacity[resourceId] += buildingType.behaviour.storageCapacity[resourceId] * building.Value;
                }
            }
        }
        
        
        // ---------------------------------------------------
        // Query behaviours
        // ---------------------------------------------------
        
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
            var speciesID = populationMembers.Select(pop => pop.populationSpeciesTypeName).Distinct().ToList();
            return speciesID.ToArray();
        }

        private string[] GetSettlementPopulationMembers()
        {
            var popIDs = populationMembers.Select(pop => pop.ID).ToList();
            return popIDs.ToArray();
        }

        private string[] GetSettlementPopulationSpeciesMembers(string speciesID)
        {
            var popIDs = populationMembers.Filter(pop => pop.populationSpeciesTypeName == speciesID).Select(pop => pop.ID).ToList();
            return popIDs.ToArray();
        }
        
        private string[] GetTaskAssignments(string taskId)
        {
            var popIDs = populationMembers.Filter(pop => pop.taskAssigment == taskId).Select(pop => pop.ID).ToList();
            return popIDs.ToArray();
        }
        
        private string[] GetTaskDescription(string taskID)
        {
            return TaskType.GetTaskById(taskID).GetDescriptionDisplay(ID);
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

        private string[] GetInventoryCategoryList()
        {
            var availableCategories = new List<string>();
            foreach (var category in DataQuery<ResourceCategory>.GetAll())
            {
                foreach (var resource in inventory)
                {
                    if (Helpers.GetDataTypeById<ResourceType>(resource.Key).category != category.Key)
                        continue;
                    availableCategories.Add(category.Key);
                    break;
                }
            }
            return availableCategories.ToArray();
        }
        
        private string[] GetInventoryList()
        {
            return inventory.Keys.ToArray();
        }

        private string[] GetInventoryListByCategory(string categoryId)
        {
            var inventoryList = new List<string>();
            foreach (var resource in inventory)
            {
                if (Helpers.GetDataTypeById<ResourceType>(resource.Key).category != categoryId)
                    continue;
                inventoryList.Add(resource.Key);
            }
            return inventoryList.ToArray();
        }
        
        private float GetInventoryResourceAmount(string resourceId)
        {
            return !inventory.ContainsKey(resourceId) ? 0f : inventory[resourceId];
        }

        private float GetInventoryResourceCapacity(string resourceId)
        {
            return resourceCapacity.ContainsKey(resourceId) ? resourceCapacity[resourceId] : 0f;
        }
        
        private float GetInventoryResourceDelta(string resourceId)
        {
            return !inventoryDelta.ContainsKey(resourceId) ? 0f : inventoryDelta[resourceId];
        }

        private (ResourceType resourceType, float hungerRecoveryFactor) GetNextAvailableFoodPortion()
        {
            var highestFactor = 0f;
            ResourceType highestFood = null;
            foreach (var inventoryItem in inventory)
            {
                var resourceType = Helpers.GetResourceTypeById(inventoryItem.Key);
                if (resourceType.hungerRecoveryFactor > highestFactor && inventoryItem.Value > 0f)
                {
                    highestFactor = resourceType.hungerRecoveryFactor;
                    highestFood = resourceType;
                }
            }
            return (highestFood, highestFactor);
        }
        
        private bool HasBuildingUnlocked(string buildingID)
        {
            return availableBuildings.Contains(buildingID);
        }
        
        private string[] GetBuildingsList()
        {
            return buildings.Keys.ToArray();
        }

        private int GetBuildingAmount(string buildingId)
        {
            return !buildings.ContainsKey(buildingId) ? 0 : buildings[buildingId];
        }
        
        private string GetBuildingDisplayName(string buildingID)
        {
            return T._(Helpers.GetDataTypeById<BuildingType>(buildingID).name);
        }

        private string[] GetBuildingDescription(string buildingID)
        {
            return Helpers.GetDataTypeById<BuildingType>(buildingID).GetDescriptionDisplay();
        }

        private bool GetBuildingCanAfford(string buildingID)
        {
            foreach (var resourceCost in GetBuildingCost(buildingID))
            {
                if (!inventory.ContainsKey(resourceCost.Key))
                    return false;
                if (inventory[resourceCost.Key] < resourceCost.Value)
                    return false;
            }
            return true;
        }

        private Dictionary<string, float> GetBuildingCost(string buildingID)
        {
            var actualBuildingCost = new Dictionary<string, float>(); 
            var buildingType = Helpers.GetDataTypeById<BuildingType>(buildingID);
            var currentNumber = GetBuildingAmount(buildingID);
            foreach (var resourceCost in buildingType.cost)
                actualBuildingCost[resourceCost.Key] = (
                    currentNumber == 0 ?
                        resourceCost.Value :
                        resourceCost.Value * MathF.Pow(buildingType.costGrowth, currentNumber)
                    );
            return actualBuildingCost;
        }

        private string[] GetBuildingResourceList(string buildingID)
        {
            var buildingType = Helpers.GetDataTypeById<BuildingType>(buildingID);
            return buildingType.cost.Keys.ToArray();
        }

        private float GetBuildingSingleResourceCost(string buildingID, string resourceID)
        {
            return GetBuildingCost(buildingID)[resourceID];
        }
        
        public static float GetTimeToCompleteTask(string settlementUuid, string taskId)
        {
            var timeToComplete = Helpers.GetTaskTypeById(taskId).timeToComplete;            
            var unlockedUpgrades = Simulation.Instance.Query<string[]>(ClientQuery.UpgradesUnlocked);
            foreach (var upgradeId in unlockedUpgrades)
            {
                var upgrade = Helpers.GetDataTypeById<UpgradeType>(upgradeId);
                foreach (var behaviour in upgrade.behaviour)
                {
                    if (behaviour.method != UpgradeBehaviourMethod.TASK_SPEED_ADJUST || behaviour.id != taskId)
                        continue;
                    timeToComplete *= behaviour.value;
                }
            }
            return timeToComplete;
        }

        public static float GetExtractTaskAmount(string settlementUuid, string taskId, int behaviourIndex = 0)
        {
            var taskBehaviour = Helpers.GetTaskTypeById(taskId).behaviour[behaviourIndex];
            if (taskBehaviour.method != TaskTypeBehaviourMethod.EXTRACT &&
                taskBehaviour.method != TaskTypeBehaviourMethod.RESEARCH)
                return 0f;
            var amount = taskBehaviour.value;            
            var unlockedUpgrades = Simulation.Instance.Query<string[]>(ClientQuery.UpgradesUnlocked);
            foreach (var upgradeId in unlockedUpgrades)
            {
                var upgrade = Helpers.GetDataTypeById<UpgradeType>(upgradeId);
                foreach (var behaviour in upgrade.behaviour)
                {
                    if (behaviour.method != UpgradeBehaviourMethod.TASK_EXTRACT_AMOUNT_ADJUST || behaviour.id != taskId)
                        continue;
                    amount *= behaviour.value;
                }
            }
            return amount;
        }
        
        
        // ---------------------------------------------------
        // Event handlers
        // ---------------------------------------------------
        
        public void OnSettlementHasNewResourceHandler(object sender, SettlementHasNewResourceEventArgs e)
        {
            switch (e.ResourceId)
            {
                // Unlock cut tree task when getting raw food
                case "raw_food":
                    spawnEnabled = true;
                    if (Simulation.Instance.Query<bool, string>(ClientQuery.TaskUnlocked, "cut_trees"))
                        return;
                    Simulation.Instance.PerformAction(ClientAction.UnlockTask, "cut_trees");
                    Simulation.Instance.PerformAction(
                        ClientAction.SendMessage,
                        T._("With food available, it's time to find some better building materials.")
                    );
                    Simulation.Instance.PerformAction(
                        ClientAction.SendMessage,
                        T._("The trees around here will take some work to fell, but they are the only option.")
                    );
                    break;
                // Unlock Buildings page when getting first logs
                case "logs":
                    var hasUnlockedBuildings = Simulation.Instance.Query<bool, string>(
                        ClientQuery.HasUnlockedPage, Consts.Pages.Buildings
                    );
                    if (hasUnlockedBuildings)
                        return;
                    Simulation.Instance.PerformAction(ClientAction.UnlockPage, Consts.Pages.Buildings);
                    Simulation.Instance.PerformAction(
                        ClientAction.SendMessage, T._("We can construct new buildings with wood.")
                    );
                    UnlockBuilding("log_hut");
                    break;
                // Unlock Research page when getting first research
                case "research":
                    var hasUnlockedResearch = Simulation.Instance.Query<bool, string>(
                        ClientQuery.HasUnlockedPage, Consts.Pages.Research
                    );
                    if (hasUnlockedResearch)
                        return;
                    Simulation.Instance.PerformAction(ClientAction.UnlockPage, Consts.Pages.Research);
                    Simulation.Instance.PerformAction(
                        ClientAction.SendMessage,
                        T._("Best apply some of those smart thinkings to new projects.")
                    );
                    break;
            }
        }

        public void OnSettlementSpawnedNewPopulationHandler(object sender, SettlementSpawnedNewPopulationEventArgs e)
        {
            // Unlock research task when hitting a population threshold
            var hasResearch = Simulation.Instance.Query<bool, string>(ClientQuery.TaskUnlocked, "research");
            if (!hasResearch && GetNumPopulation() >= Consts.UnlockResearchAtPopulationCount)
            {
                Simulation.Instance.PerformAction(ClientAction.UnlockTask, "research");
                Simulation.Instance.PerformAction(
                    ClientAction.SendMessage,
                    T._("The last Lamia to join your settlement has some bright ideas.")
                );
            }
        }

        public void OnSettlementBuildingPurchasedHandler(object sender, SettlementBuildingPurchasedEventArgs e)
        {
            switch (e.BuildingId)
            {
                // Unlock upgrades page when getting first Archives
                case "archives":
                    var hasUnlockedUpgrades = Simulation.Instance.Query<bool, string>(
                        ClientQuery.HasUnlockedPage, Consts.Pages.Upgrades
                    );
                    if (hasUnlockedUpgrades)
                        return;
                    Simulation.Instance.PerformAction(ClientAction.UnlockPage, Consts.Pages.Upgrades);
                    Simulation.Instance.PerformAction(
                        ClientAction.SendMessage,
                        T._("It's about time we applied all this accumulated knowledge to improving our existing stuff.")
                    );
                    break;
            }
        }

    }
}