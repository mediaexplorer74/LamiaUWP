using System;
using System.Collections.Generic;
using System.Linq;

namespace LamiaSimulation
{

    using Text = T;

    internal class GlobalState: IActionReceiver, IQueryable, ISimulated
    {
        public List<string> availablePages { get; set; }
        public List<Location> locations { get; set; }
        public List<Settlement> playerSettlements { get; set; }
        public List<string> messageHistory { get; set; }
        public List<string> unreadMessages { get; set; }
        public List<string> currentlyDisplayedMessages { get; set; }
        public List<string> availableResearch { get; set; }
        public List<string> unlockedResearch { get; set; }
        
        public GlobalState()
        {
            locations = new List<Location>();
            playerSettlements = new List<Settlement>();
            messageHistory = new List<string>();
            unreadMessages = new List<string>();
            currentlyDisplayedMessages = new List<string>();
            availablePages = new List<string>();
            availableResearch = new List<string>();
            unlockedResearch = new List<string>();
        }

        ~GlobalState()
        {
            Simulation.Instance.events.UnlockedPageEvent -= OnUnlockedPageHandler;
        }

        public void Init()
        {
            Simulation.Instance.events.UnlockedPageEvent += OnUnlockedPageHandler;
        }

        public void LoadedFromSave()
        {
            unreadMessages = new List<string>(currentlyDisplayedMessages);
            currentlyDisplayedMessages.Clear();
            DetermineAvailableResearch();
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
                // Unlocks a research
                case ClientAction.UnlockResearch:
                    UnlockResearch(param1.Get as string);
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
                        if (messageHistory.Count > 10)
                            messageHistory.RemoveRange(0, messageHistory.Count - 10);
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
                // Research available
                case ClientQuery.ResearchAvailable:
                    result = new QueryResult<string[]>(availableResearch.ToArray()) as QueryResult<T>;
                    break;
                // Research unlocked
                case ClientQuery.ResearchUnlocked:
                    result = new QueryResult<string[]>(unlockedResearch.ToArray()) as QueryResult<T>;
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
                        new ClientParameter<string>(settlement.ID),
                        new ClientParameter<string>(resourceCost.Key),
                        new ClientParameter<float>(amountToDeduct)
                    );
                }
            }
            availableResearch.Remove(researchId);
            unlockedResearch.Add(researchId);
            DoResearchEffect(researchId);
            DetermineAvailableResearch();
        }

        private void DoResearchEffect(string researchId)
        {
            var research = Helpers.GetDataTypeById<ResearchType>(researchId);
            switch (research.behaviour)
            {
                case ResearchBehaviour.UNLOCK_BUILDING:
                    foreach (var settlement in playerSettlements)
                    {
                        Simulation.Instance.PerformAction(
                            ClientAction.SettlementUnlockBuilding,
                            new ClientParameter<string>(settlement.ID),
                            new ClientParameter<string>(research.unlockId)
                        );
                    }
                    break;
                default:
                    throw new ClientActionException(Text._("Research behaviour not implemented"));
            }
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

        public string PageDisplayName(string pageId)
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
        
        // ---------------------------------------------------
        // Event handlers
        // ---------------------------------------------------

        public void OnUnlockedPageHandler(object sender, UnlockedPageEventArgs e)
        {
            if(e.PageId == Consts.Pages.Research)
                DetermineAvailableResearch();
        }
    }
}