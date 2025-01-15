using System;
using System.Collections.Generic;
using System.Linq;

namespace LamiaSimulation
{

    using Text = T;

    internal class GlobalState: IActionReceiver, IQueryable, ISimulated
    {
        public List<string> availablePages { get; set; } = new();
        public List<Location> locations { get; set; } = new();
        public List<Settlement> playerSettlements { get; set; } = new();
        public List<string> availableTasks { get; set; } = new();
        public List<string> messageHistory { get; set; } = new();
        public List<string> unreadMessages { get; set; } = new();
        public List<string> currentlyDisplayedMessages { get; set; } = new();
        public List<string> availableResearch { get; set; } = new();
        public List<string> unlockedResearch { get; set; } = new();
        public List<string> availableUpgrades { get; set; } = new();
        public List<string> unlockedUpgrades { get; set; } = new();

        ~GlobalState()
        {
            Simulation.Instance.events.SettlementHasNewResourceEvent -= OnSettlementHasNewResourceHandler;
            Simulation.Instance.events.SettlementSpawnedNewPopulationEvent -= OnSettlementSpawnedNewPopulationHandler;
            Simulation.Instance.events.SettlementBuildingPurchasedEvent -= OnSettlementBuildingPurchasedHandler;
            Simulation.Instance.events.UnlockedPageEvent -= OnUnlockedPageHandler;
        }

        public void Init()
        {
            Simulation.Instance.events.SettlementHasNewResourceEvent += OnSettlementHasNewResourceHandler;
            Simulation.Instance.events.SettlementSpawnedNewPopulationEvent += OnSettlementSpawnedNewPopulationHandler;
            Simulation.Instance.events.SettlementBuildingPurchasedEvent += OnSettlementBuildingPurchasedHandler;
            Simulation.Instance.events.UnlockedPageEvent += OnUnlockedPageHandler;
        }

        public void LoadedFromSave()
        {
            unreadMessages = new List<string>(currentlyDisplayedMessages);
            availableUpgrades ??= new List<string>();
            unlockedUpgrades ??= new List<string>();
            currentlyDisplayedMessages.Clear();
            DetermineAvailableResearch();
            DetermineAvailableUpgrades();
            Init();
            try
            {
                foreach(var location in locations)
                    location.LoadedFromSave();
                foreach(var settlement in playerSettlements)
                    settlement.LoadedFromSave();
            }
            catch(ClientActionException e)
            {
                AddMessage(e.Message);
            }
        }

        // ---------------------------------------------------
        // IActionReceiver
        // ---------------------------------------------------

        public void PerformAction(ClientAction action)
        {
            try
            {
                foreach(var location in locations)
                    location.PerformAction(action);
                foreach(var settlement in playerSettlements)
                    settlement.PerformAction(action);
            }
            catch(ClientActionException e)
            {
                AddMessage(e.Message);
            }
        }

        public void PerformAction<T>(ClientAction action, ClientParameter<T> param1)
        {
            switch(action)
            {
                // Unlock page
                case ClientAction.UnlockPage:
                    if (!availablePages.Contains(param1.Get as string))
                    {
                        availablePages.Add(param1.Get as string);
                        // Fire event
                        Simulation.Instance.events.OnUnlockedPage(
                            new UnlockedPageEventArgs
                            {
                                PageId = param1.Get as string
                            }
                        );
                    }
                    break;
                // Send message to client
                case ClientAction.SendMessage:
                    AddMessage(param1.Get as string);
                    break;
                // Add new location to world
                case ClientAction.AddLocation:
                    locations.Add(new Location(param1.Get as string));
                    break;
                // Creates a new settlement at a location
                case ClientAction.AddSettlementAtLocation:
                    var location = locations.Filter(
                        l => l.ID == param1.Get.ToString()
                    ).FirstOrDefault();
                    if(location == null)
                        throw new ClientActionException(Text._("Location to add settlement to does not exist."));
                    var existingSettlement = playerSettlements.Filter(
                        s => s.locationUuid == param1.Get.ToString()
                    ).FirstOrDefault();
                    if(existingSettlement != null)
                        throw new ClientActionException(Text._("Settlement already exists at location"));
                    var newSettlement = new Settlement(Text._("Unnamed"), param1.Get.ToString());
                    playerSettlements.Add(newSettlement);
                    break;
                // Unlock task
                case ClientAction.UnlockTask:
                    UnlockTask(param1.Get as string);
                    break;
                // Unlocks a research
                case ClientAction.UnlockResearch:
                    UnlockResearch(param1.Get as string);
                    break;
                // Force unlocks a research
                case ClientAction.ForceUnlockResearch:
                    ForceUnlockResearch(param1.Get as string);
                    break;
                // Unlocks an upgrade
                case ClientAction.UnlockUpgrade:
                    UnlockUpgrade(param1.Get as string);
                    break;
                // Force unlocks an upgrade
                case ClientAction.ForceUnlockUpgrade:
                    ForceUnlockUpgrade(param1.Get as string);
                    break;
            }

            try
            {
                foreach(var location in locations)
                    location.PerformAction(action, param1);
                foreach(var settlement in playerSettlements)
                    settlement.PerformAction(action, param1);
            }
            catch(ClientActionException e)
            {
                AddMessage(e.Message);
            }
        }

        public void PerformAction<T1, T2>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
            try
            {
                foreach(var location in locations)
                    location.PerformAction(action, param1, param2);
                foreach(var settlement in playerSettlements)
                    settlement.PerformAction(action, param1, param2);
            }
            catch(ClientActionException e)
            {
                AddMessage(e.Message);
            }
        }

        public void PerformAction<T1, T2, T3>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2,
            ClientParameter<T3> param3)
        {
            try
            {
                foreach(var location in locations)
                    location.PerformAction(action, param1, param2, param3);
                foreach(var settlement in playerSettlements)
                    settlement.PerformAction(action, param1, param2, param3);
            }
            catch(ClientActionException e)
            {
                AddMessage(e.Message);
            }
        }

        // ---------------------------------------------------
        // IQueryable
        // ---------------------------------------------------

        public void Query<T>(ref QueryResult<T> result, ClientQuery query)
        {
            switch(query)
            {
                // Available game pages
                case ClientQuery.AvailablePages:
                    result = new QueryResult<(string, string)[]>(GetAvailablePages().ToArray()) as QueryResult<T>;
                    break;
                // Available global pages
                case ClientQuery.AvailableGlobalPages:
                    result = new QueryResult<(string, string)[]>(GetAvailableGlobalPages().ToArray()) as QueryResult<T>;
                    break;
                // Message history
                case ClientQuery.MessageHistory:
                    result = new QueryResult<string[]>(messageHistory.ToArray()) as QueryResult<T>;
                    break;
                // Unread messages
                case ClientQuery.UnreadMessages:
                    result = new QueryResult<string[]>(unreadMessages.ToArray()) as QueryResult<T>;
                    if (unreadMessages.Count > 0)
                    {
                        foreach (var message in currentlyDisplayedMessages)
                            messageHistory.Add(message);
                        if (messageHistory.Count > Consts.MessageHistoryLimit)
                            messageHistory.RemoveRange(0, messageHistory.Count - Consts.MessageHistoryLimit);
                        currentlyDisplayedMessages.Clear();
                        foreach (var message in unreadMessages)
                            currentlyDisplayedMessages.Add(message);
                        unreadMessages.Clear();
                    }
                    break;
                // Location IDs
                case ClientQuery.Locations:
                    result = new QueryResult<string[]>(locations.Map(s => s.ID).ToArray()) as QueryResult<T>;
                    break;
                // Settlement IDs
                case ClientQuery.Settlements:
                    result = new QueryResult<string[]>(playerSettlements.Map(s => s.ID).ToArray()) as QueryResult<T>;
                    break;
                // Available tasks
                case ClientQuery.Tasks:
                    result = new QueryResult<string[]>(GetAvailableTasks()) as QueryResult<T>;
                    break;
                // Research available
                case ClientQuery.ResearchAvailable:
                    result = new QueryResult<string[]>(availableResearch.ToArray()) as QueryResult<T>;
                    break;
                // Research unlocked
                case ClientQuery.ResearchUnlocked:
                    result = new QueryResult<string[]>(unlockedResearch.ToArray()) as QueryResult<T>;
                    break;
                // Upgrades available
                case ClientQuery.UpgradesAvailable:
                    result = new QueryResult<string[]>(availableUpgrades.ToArray()) as QueryResult<T>;
                    break;
                // Upgrades unlocked
                case ClientQuery.UpgradesUnlocked:
                    result = new QueryResult<string[]>(unlockedUpgrades.ToArray()) as QueryResult<T>;
                    break;
            }

            foreach(var location in locations)
                location.Query(ref result, query);
            foreach(var settlement in playerSettlements)
                settlement.Query(ref result, query);
        }

        public void Query<T, T1>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1)
        {
            switch(query)
            {
                // Has unlocked page?
                case ClientQuery.HasUnlockedPage:
                    result = new QueryResult<bool>(availablePages.Contains(param1.Get as string)) as QueryResult<T>;
                    break;
                // Species name
                case ClientQuery.SpeciesName:
                    result = new QueryResult<string>(Text._(Helpers.GetSpeciesTypeById(param1.Get as string).name)) as QueryResult<T>;
                    break;
                // Species description
                case ClientQuery.SpeciesDescription:
                    result = new QueryResult<string[]>(Text._(Helpers.GetSpeciesTypeById(param1.Get as string).description)) as QueryResult<T>;
                    break;
                // Resource category name
                case ClientQuery.ResourceCategoryName:
                    result = new QueryResult<string>(Text._(Helpers.GetDataTypeById<ResourceCategory>(param1.Get as string).name)) as QueryResult<T>;
                    break;
                // Resource description
                case ClientQuery.ResourceCategoryDescription:
                    result = new QueryResult<string>(Text._(Helpers.GetDataTypeById<ResourceCategory>(param1.Get as string).description)) as QueryResult<T>;
                    break;
                // Resource name
                case ClientQuery.ResourceName:
                    result = new QueryResult<string>(Text._(Helpers.GetResourceTypeById(param1.Get as string).name)) as QueryResult<T>;
                    break;
                // Resource description
                case ClientQuery.ResourceDescription:
                    result = new QueryResult<string>(Text._(Helpers.GetResourceTypeById(param1.Get as string).description)) as QueryResult<T>;
                    break;
                // Task names
                case ClientQuery.TaskName:
                    result = new QueryResult<string>(GetTaskName(param1.Get as string)) as QueryResult<T>;
                    break;
                // Is task unlocked
                case ClientQuery.TaskUnlocked:
                    result = new QueryResult<bool>(availableTasks.Contains(param1.Get as string)) as QueryResult<T>;
                    break;
                // Research name
                case ClientQuery.ResearchDisplayName:
                    result = new QueryResult<string>(Text._(Helpers.GetDataTypeById<ResearchType>(param1.Get as string).name)) as QueryResult<T>;
                    break;
                // Research description
                case ClientQuery.ResearchDescription:
                    result = new QueryResult<string>(Text._(Helpers.GetDataTypeById<ResearchType>(param1.Get as string).description)) as QueryResult<T>;
                    break;
                // Research can afford
                case ClientQuery.ResearchCanAfford:
                    result = new QueryResult<bool>(CanAffordResearch(param1.Get as string)) as QueryResult<T>;
                    break;
                // Research resource cost resource types
                case ClientQuery.ResearchResourceList:
                    result = new QueryResult<string[]>(Helpers.GetDataTypeById<ResearchType>(param1.Get as string).cost.Keys.ToArray()) as QueryResult<T>;
                    break;
                // Upgrade name
                case ClientQuery.UpgradeDisplayName:
                    result = new QueryResult<string>(Text._(Helpers.GetDataTypeById<UpgradeType>(param1.Get as string).name)) as QueryResult<T>;
                    break;
                // Upgrade description
                case ClientQuery.UpgradeDescription:
                    result = new QueryResult<string>(Text._(Helpers.GetDataTypeById<UpgradeType>(param1.Get as string).description)) as QueryResult<T>;
                    break;
                // Upgrade can afford
                case ClientQuery.UpgradeCanAfford:
                    result = new QueryResult<bool>(CanAffordUpgrade(param1.Get as string)) as QueryResult<T>;
                    break;
                // Upgrade resource cost resource types
                case ClientQuery.UpgradeResourceList:
                    result = new QueryResult<string[]>(Helpers.GetDataTypeById<UpgradeType>(param1.Get as string).cost.Keys.ToArray()) as QueryResult<T>;
                    break;
            }

            foreach(var location in locations)
                location.Query(ref result, query, param1);
            foreach(var settlement in playerSettlements)
                settlement.Query(ref result, query, param1);
        }

        public void Query<T, T1, T2>(ref QueryResult<T> result, ClientQuery query,
            ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
            switch (query)
            {
                // Research single resource type cost 
                case ClientQuery.ResearchSingleResourceCost:
                    result = new QueryResult<float>(Helpers.GetDataTypeById<ResearchType>(param1.Get as string).cost[param2.Get as string]) as QueryResult<T>;
                    break;
                // Upgrade single resource type cost 
                case ClientQuery.UpgradeSingleResourceCost:
                    result = new QueryResult<float>(Helpers.GetDataTypeById<UpgradeType>(param1.Get as string).cost[param2.Get as string]) as QueryResult<T>;
                    break;
            }
            foreach(var location in locations)
                location.Query(ref result, query, param1, param2);
            foreach(var settlement in playerSettlements)
                settlement.Query(ref result, query, param1, param2);
        }

        public void Query<T, T1, T2, T3>(ref QueryResult<T> result, ClientQuery query,
            ClientParameter<T1> param1, ClientParameter<T2> param2, ClientParameter<T3> param3)
        {
            foreach(var location in locations)
                location.Query(ref result, query, param1, param2, param3);
            foreach(var settlement in playerSettlements)
                settlement.Query(ref result, query, param1, param2, param3);
        }

        // ---------------------------------------------------
        // ISimulated
        // ---------------------------------------------------

        public void Simulate(float deltaTime)
        {
            locations.Apply(location => location.Simulate(deltaTime));
            playerSettlements.Apply(settlement => settlement.Simulate(deltaTime));
        }

        // ---------------------------------------------------
        // Action behaviours
        // ---------------------------------------------------

        public void AddMessage(string message)
        {
            unreadMessages.Add(message);
        }

        private void UnlockTask(string taskID)
        {
            if(availableTasks.Contains(taskID))
                throw new ClientActionException(T._("Task already unlocked."));
            if(DataQuery<TaskType>.GetByID(taskID) == null)
                throw new ClientActionException(T._("Task does not exist."));
            availableTasks.Add(taskID);
        }
        
        private void DetermineAvailableResearch()
        {
            availableResearch.Clear();
            var allResearch = DataQuery<ResearchType>.GetAll();
            foreach (var research in allResearch)
            {
                if (unlockedResearch.Contains(research.Key))
                    continue;
                if (HavePrerequisitesForResearch(research.Key))
                    availableResearch.Add(research.Key);
            }
        }
        
        private bool HavePrerequisitesForResearch(string researchId)
        {
            var research = Helpers.GetDataTypeById<ResearchType>(researchId);
            if (research.prerequisites == null || research.prerequisites.Count == 0)
                return true;
            foreach (var prerequisite in research.prerequisites)
                if (!unlockedResearch.Contains(prerequisite))
                    return false;
            return true;
        }

        private bool CanAffordResearch(string researchId)
        {
            var research = Helpers.GetDataTypeById<ResearchType>(researchId);
            foreach (var resourceCost in research.cost)
            {
                var accumulatedResource = 0f;
                foreach (var settlement in playerSettlements)
                {
                    accumulatedResource += Simulation.Instance.Query<float, string, string>(
                        ClientQuery.SettlementInventoryResourceAmount, settlement.ID, resourceCost.Key
                    );
                }
                if (accumulatedResource < resourceCost.Value)
                    return false;
            }
            return true;
        }

        private void UnlockResearch(string researchId)
        {
            if (unlockedResearch.Contains(researchId))
                return;
            if (!availableResearch.Contains(researchId))
                return;
            if (!HavePrerequisitesForResearch(researchId))
                return;
            if (!CanAffordResearch(researchId))
                return;
            var research = Helpers.GetDataTypeById<ResearchType>(researchId);
            foreach (var resourceCost in research.cost)
            {
                var resourceLeftToPay = resourceCost.Value;
                foreach (var settlement in playerSettlements)
                {
                    if (resourceLeftToPay <= 0f)
                        continue;
                    var amountAtSettlement = Simulation.Instance.Query<float, string, string>(
                        ClientQuery.SettlementInventoryResourceAmount, settlement.ID, resourceCost.Key
                    );
                    if (amountAtSettlement <= 0f)
                        continue;
                    var amountToDeduct = (
                        amountAtSettlement < resourceLeftToPay ? amountAtSettlement : resourceLeftToPay
                    );
                    resourceLeftToPay -= amountToDeduct;
                    Simulation.Instance.PerformAction(
                        ClientAction.SubtractResourceFromSettlementInventory,
                        settlement.ID,
                        resourceCost.Key,
                        amountToDeduct
                    );
                }
            }
            DoUnlockResearch(researchId);
        }

        private void ForceUnlockResearch(string researchId)
        {
            if(unlockedResearch.Contains(researchId))
                return;
            DoUnlockResearch(researchId);            
        }
        
        private void DoUnlockResearch(string researchId)
        {
            unlockedResearch.Add(researchId);
            DoResearchEffect(researchId);
            var research = Helpers.GetDataTypeById<ResearchType>(researchId);
            if(research.unlockMessage != null)
                Simulation.Instance.PerformAction(ClientAction.SendMessage, research.unlockMessage);
            DetermineAvailableResearch();
        }

        private void DoResearchEffect(string researchId)
        {
            var research = Helpers.GetDataTypeById<ResearchType>(researchId);
            foreach (var behaviour in research.behaviour)
            {
                switch (behaviour.method)
                {
                    case ResearchBehaviourMethod.UNLOCK_BUILDING:
                        foreach (var settlement in playerSettlements)
                        {
                            Simulation.Instance.PerformAction(
                                ClientAction.SettlementUnlockBuilding, 
                                settlement.ID,
                                behaviour.id
                            );
                        }
                        break;
                    case ResearchBehaviourMethod.UNLOCK_TASK:
                        Simulation.Instance.PerformAction(ClientAction.UnlockTask, behaviour.id);
                        break;
                    default:
                        throw new ClientActionException(Text._("Research behaviour not implemented"));
                }
            }
        }

        private void DetermineAvailableUpgrades()
        {
            availableUpgrades.Clear();
            var hasUnlockedUpgrades = Simulation.Instance.Query<bool, string>(
                ClientQuery.HasUnlockedPage, Consts.Pages.Upgrades
            );
            if(!hasUnlockedUpgrades)
                return;
            var allUpgrades = DataQuery<UpgradeType>.GetAll();
            foreach (var upgrade in allUpgrades)
            {
                if (unlockedUpgrades.Contains(upgrade.Key))
                    continue;
                if (HavePrerequisitesForUpgrade(upgrade.Key))
                    availableUpgrades.Add(upgrade.Key);
            }
        }
        
        private bool HavePrerequisitesForUpgrade(string upgradeId)
        {
            var upgrade = Helpers.GetDataTypeById<UpgradeType>(upgradeId);
            if (upgrade.prerequisites == null || upgrade.prerequisites.Count == 0)
                return true;
            foreach (var prerequisite in upgrade.prerequisites)
            {
                switch (prerequisite.method)
                {
                    case UpgradePrerequisiteMethod.HAS_BUILDING:
                        var haveBuilding = false;
                        foreach(var settlement in playerSettlements)
                        {
                            var num = Simulation.Instance.Query<int, string, string>(ClientQuery.SettlementBuildingsAmount,
                                settlement.ID, prerequisite.id);
                            if(num <= 0)
                                continue;
                            haveBuilding = true;
                            break;
                        }
                        if(!haveBuilding)
                            return false;
                        break;
                    case UpgradePrerequisiteMethod.UPGRADE_UNLOCKED:
                        if(!unlockedUpgrades.Contains(prerequisite.id))
                            return false;
                        break;
                    case UpgradePrerequisiteMethod.HAS_RESOURCE:
                        var haveResource = false;
                        foreach(var settlement in playerSettlements)
                        {
                            var num = Simulation.Instance.Query<int, string, string>(ClientQuery.SettlementInventoryResourceAmount,
                                settlement.ID, prerequisite.id);
                            if (num <= 0)
                                continue;
                            haveResource = true;
                            break;
                        }
                        if(!haveResource)
                            return false;
                        break;
                    case UpgradePrerequisiteMethod.RESEARCH_UNLOCKED:
                        var research = Simulation.Instance.Query<string[]>(ClientQuery.ResearchUnlocked);
                        if(!research.Contains(prerequisite.id))
                            return false;
                        break;
                    default:
                        throw new ClientActionException(
                            T._($"Unhandled upgrade prerequisite method {prerequisite.method}")
                            );
                }
            }
            return true;
        }
        
        private bool CanAffordUpgrade(string upgradeId)
        {
            var upgrade = Helpers.GetDataTypeById<UpgradeType>(upgradeId);
            foreach(var resourceCost in upgrade.cost)
            {
                var accumulatedResource = 0f;
                foreach(var settlement in playerSettlements)
                {
                    accumulatedResource += Simulation.Instance.Query<float, string, string>(
                        ClientQuery.SettlementInventoryResourceAmount, settlement.ID, resourceCost.Key
                    );
                }
                if(accumulatedResource < resourceCost.Value)
                    return false;
            }
            return true;
        }

        private void DoUnlockUpgrade(string upgradeId)
        {
            availableUpgrades.Remove(upgradeId);
            unlockedUpgrades.Add(upgradeId);
            var upgrade = Helpers.GetDataTypeById<UpgradeType>(upgradeId);
            if(upgrade.unlockMessage != null)
                Simulation.Instance.PerformAction(ClientAction.SendMessage, upgrade.unlockMessage);
            DetermineAvailableUpgrades();
        }

        private void UnlockUpgrade(string upgradeId)
        {
            if (unlockedUpgrades.Contains(upgradeId))
                return;
            if (!availableUpgrades.Contains(upgradeId))
                return;
            if (!HavePrerequisitesForUpgrade(upgradeId))
                return;
            if (!CanAffordUpgrade(upgradeId))
                return;
            var upgrade = Helpers.GetDataTypeById<UpgradeType>(upgradeId);
            foreach (var resourceCost in upgrade.cost)
            {
                var resourceLeftToPay = resourceCost.Value;
                foreach (var settlement in playerSettlements)
                {
                    if (resourceLeftToPay <= 0f)
                        continue;
                    var amountAtSettlement = Simulation.Instance.Query<float, string, string>(
                        ClientQuery.SettlementInventoryResourceAmount, settlement.ID, resourceCost.Key
                    );
                    if (amountAtSettlement <= 0f)
                        continue;
                    var amountToDeduct = (
                        amountAtSettlement < resourceLeftToPay ? amountAtSettlement : resourceLeftToPay
                    );
                    resourceLeftToPay -= amountToDeduct;
                    Simulation.Instance.PerformAction(
                        ClientAction.SubtractResourceFromSettlementInventory,
                        settlement.ID,
                        resourceCost.Key,
                        amountToDeduct
                    );
                }
            }            
            DoUnlockUpgrade(upgradeId);
        }
        
        private void ForceUnlockUpgrade(string upgradeId)
        {
            DoUnlockUpgrade(upgradeId);
        }
        
        // ---------------------------------------------------
        // Query behaviours
        // ---------------------------------------------------

        public List<(string, string)> GetAvailablePages()
        {
            var pageList = new List<(string, string)>();
            foreach(var pageId in availablePages)
                pageList.Add((pageId, PageDisplayName(pageId)));
            return pageList;
        }

        public List<(string, string)> GetAvailableGlobalPages()
        {
            var pageList = new List<(string, string)>();
            foreach (var pageId in availablePages)
                if(Consts.GlobalPages.Contains(pageId))
                    pageList.Add((pageId, PageDisplayName(pageId)));
            return pageList;
        }
        
        public static string PageDisplayName(string pageId)
        {
            switch (pageId)
            {
                case "population":
                    return T._("Population");
                case "buildings":
                    return T._("Buildings");
                case "research":
                    return T._("Research");
                case "upgrades":
                    return T._("Upgrades");
            }
            throw new NotImplementedException("No specified page name");
        }
        
        private string[] GetAvailableTasks()
        {
            return availableTasks.ToArray();
        }

        private string GetTaskName(string taskID)
        {
            return T._(TaskType.GetTaskById(taskID).name);
        }
        
        // ---------------------------------------------------
        // Event handlers
        // ---------------------------------------------------

        public void OnUnlockedPageHandler(object sender, UnlockedPageEventArgs e)
        {
            switch (e.PageId)
            {
                case "research":
                    DetermineAvailableResearch();
                    break;
                case "upgrades":
                    DetermineAvailableUpgrades();
                    break;
            }
        }

        public void OnSettlementHasNewResourceHandler(object sender, SettlementHasNewResourceEventArgs e)
        {
            DetermineAvailableUpgrades();
        }

        public void OnSettlementSpawnedNewPopulationHandler(object sender, SettlementSpawnedNewPopulationEventArgs e)
        {
            DetermineAvailableUpgrades();
        }

        public void OnSettlementBuildingPurchasedHandler(object sender, SettlementBuildingPurchasedEventArgs e)
        {
            DetermineAvailableUpgrades();
        }
    }
}