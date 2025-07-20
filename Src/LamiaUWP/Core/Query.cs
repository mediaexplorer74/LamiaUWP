using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LamiaSimulation;

namespace LamiaUWP.Core
{
    /// <summary>
    /// Обертка для вызовов ClientQuery, аналогичная той, что используется в Godot-версии.
    /// </summary>
    public class Query
    {
        public List<Tuple<string, string>> AvailablePages()
        {
            var pages = Simulation.Instance.Query<(string, string)[]>(ClientQuery.AvailablePages);
            return pages.Select(p => new Tuple<string, string>(p.Item1, p.Item2)).ToList();
        }

        public List<Tuple<string, string>> AvailableGlobalPages()
        {
            var pages = Simulation.Instance.Query<(string, string)[]>(ClientQuery.AvailableGlobalPages);
            return pages.Select(p => new Tuple<string, string>(p.Item1, p.Item2)).ToList();
        }
        
        public List<Tuple<string, string>> AvailableSettlementPages(string settlementUuid)
        {
            var pages = Simulation.Instance.Query<(string, string)[], string>(ClientQuery.AvailableSettlementPages, settlementUuid);
            return pages.Select(p => new Tuple<string, string>(p.Item1, p.Item2)).ToList();
        }
        
        public List<Message> MessageHistory()
        {
            var msgHistory = new List<Message>();
            try
            {
                msgHistory = Simulation.Instance.Query<List<Message>>(ClientQuery.MessageHistory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ex] MessageHistory error: " + ex.Message);
            }

            if (msgHistory.Count == 0)
                msgHistory.Add(new Message
                {
                    Title = "Welcome",
                    Content = "This is your first message!",
                    Timestamp = 1f
                });
            return msgHistory;
        }
        
        public List<Message> UnreadMessages()
        {
            return Simulation.Instance.Query<List<Message>>(ClientQuery.UnreadMessages);
        }

        public bool HasUnlockedPage(string pageId)
        {
            return Simulation.Instance.Query<bool, string>(ClientQuery.HasUnlockedPage, pageId);
        }

        public bool HasUnlockedBuilding(string buildingId)
        {
            return Simulation.Instance.Query<bool, string>(ClientQuery.HasUnlockedBuilding, buildingId);
        }

        // ----------------------
        // RESEARCH
        // ----------------------

        public string[] ResearchAvailable()
        {
            return Simulation.Instance.Query<string[]>(ClientQuery.ResearchAvailable);
        }

        public string[] ResearchUnlocked()
        {
            return Simulation.Instance.Query<string[]>(ClientQuery.ResearchUnlocked);
        }
        
        public string ResearchDisplayName(string researchId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.ResearchDisplayName, researchId);
        }

        public string ResearchDescription(string researchId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.ResearchDescription, researchId);
        }

        public bool ResearchCanAfford(string researchId)
        {
            return Simulation.Instance.Query<bool, string>(ClientQuery.ResearchCanAfford, researchId);
        }

        public string[] ResearchResourceList(string researchId)
        {
            return Simulation.Instance.Query<string[], string>(ClientQuery.ResearchResourceList, researchId);
        }

        public float ResearchSingleResourceCost(string researchId, string resourceId)
        {
            return Simulation.Instance.Query<float, string, string>(ClientQuery.ResearchSingleResourceCost, researchId, resourceId);
        }
        
        // ----------------------
        // RESOURCES
        // ----------------------

        public string[] ResourceCategories()
        {
            return Simulation.Instance.Query<string[]>(ClientQuery.ResourceCategories);
        }

        public string ResourceCategoryDisplayName(string categoryId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.ResourceCategoryDisplayName, categoryId);
        }

        public string[] ResourcesInCategory(string categoryId)
        {
            return Simulation.Instance.Query<string[], string>(ClientQuery.ResourcesInCategory, categoryId);
        }

        public string ResourceDisplayName(string resourceId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.ResourceDisplayName, resourceId);
        }

        public string ResourceDescription(string resourceId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.ResourceDescription, resourceId);
        }

        public string ResourceCategory(string resourceId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.ResourceCategory, resourceId);
        }

        // ----------------------
        // SETTLEMENTS
        // ----------------------

        public string[] Settlements()
        {
            return Simulation.Instance.Query<string[]>(ClientQuery.Settlements);
        }

        public string SettlementName(string settlementUuid)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.SettlementName, settlementUuid);
        }

        public int SettlementPopulation(string settlementUuid)
        {
            return default;//Simulation.Instance.Query<string[], string>(ClientQuery.SettlementPopulation, settlementUuid);
        }

        public string[] SettlementInventoryResources(string settlementUuid)
        {
            return Simulation.Instance.Query<string[], string>(ClientQuery.SettlementInventoryResources, settlementUuid);
        }

        public float SettlementInventoryResourceAmount(string settlementUuid, string resourceId)
        {
            return Simulation.Instance.Query<float, string, string>(
                ClientQuery.SettlementInventoryResourceAmount, settlementUuid, resourceId
            );
        }

        public float SettlementInventoryResourceCapacity(string settlementUuid, string resourceId)
        {
            return Simulation.Instance.Query<float, string, string>(
                ClientQuery.SettlementInventoryResourceCapacity, settlementUuid, resourceId
            );
        }

        public string[] SettlementBuildings(string settlementUuid)
        {
            return Simulation.Instance.Query<string[], string>(ClientQuery.SettlementBuildings, settlementUuid);
        }

        // ----------------------
        // POPULATION
        // ----------------------

        public string PopulationName(string populationUuid)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.PopulationName, populationUuid);
        }

        public string PopulationSpecies(string populationUuid)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.PopulationSpecies, populationUuid);
        }

        public string PopulationTask(string populationUuid)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.PopulationTask, populationUuid);
        }

        // ----------------------
        // TASKS
        // ----------------------

        public string[] AvailableTasks()
        {
            return Simulation.Instance.Query<string[]>(ClientQuery.AvailableTasks);
        }

        public string TaskDisplayName(string taskId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.TaskDisplayName, taskId);
        }

        public string TaskDescription(string taskId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.TaskDescription, taskId);
        }

        // ----------------------
        // BUILDINGS
        // ----------------------

        public string BuildingDisplayName(string buildingId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.BuildingDisplayName, buildingId);
        }

        public string BuildingDescription(string buildingId)
        {
            return Simulation.Instance.Query<string, string>(ClientQuery.BuildingDescription, buildingId);
        }

        public bool BuildingCanAfford(string settlementUuid, string buildingId)
        {
            return Simulation.Instance.Query<bool, string, string>(
                ClientQuery.BuildingCanAfford, settlementUuid, buildingId
            );
        }

        public string[] BuildingResourceList(string buildingId)
        {
            return Simulation.Instance.Query<string[], string>(ClientQuery.BuildingResourceList, buildingId);
        }

        public float BuildingSingleResourceCost(string buildingId, string resourceId)
        {
            return Simulation.Instance.Query<float, string, string>(
                ClientQuery.BuildingSingleResourceCost, buildingId, resourceId
            );
        }
        
        // ----------------------
        // GAME STATE
        // ----------------------
        
        /// <summary>
        /// Возвращает текущий игровой день.
        /// </summary>
        public int CurrentDay()
        {
            return Simulation.Instance.Query<int>(ClientQuery.CurrentDay);
        }

        public string TaskName(object id)
        {
            return id.ToString(); //TODO
        }

        public IEnumerable<string> UnlockedTasks()
        {
            throw new NotImplementedException();
        }

        public string ResourceName(string resourceId)
        {
            throw new NotImplementedException();
        }

        public float SettlementResourceAmount(string settlementId, string resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> AvailableBuildings()
        {
            throw new NotImplementedException();
        }

        public string UnlockedBuildings()
        {
            throw new NotImplementedException();
        }

        public string BuildingName(object buildingId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Locations()
        {
            throw new NotImplementedException();
        }

        public string LocationName(object locationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ResearchCategories()
        {
            throw new NotImplementedException();
        }

        public string ResearchCategoryName(object id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ResearchesInCategory(string categoryId)
        {
            throw new NotImplementedException();
        }

        public string UnlockedResearches()
        {
            throw new NotImplementedException();
        }

        public string ResearchName(object id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ResearchRequirements(string id)
        {
            throw new NotImplementedException();
        }
    }
}