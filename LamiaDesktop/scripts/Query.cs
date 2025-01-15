using System.Collections.Generic;
using System.Linq;
using Godot;
using LamiaSimulation;

/*
 * Thin wrapper around all ClientQuery calls as you (understandably) cannot call generic C# methods from GDScript. 
 */
public partial class Query: Node
{
    public override void _Ready()
    {
    }

    private Godot.Collections.Array<Godot.Collections.Array<string>> ConstructPageArray((string, string)[] pages)
    {
        var pageReturn = new Godot.Collections.Array<Godot.Collections.Array<string> >();
        foreach (var pageTuple in pages)
            pageReturn.Add(
                new Godot.Collections.Array<string>(new [] { pageTuple.Item1, pageTuple.Item2 })
            );
        return pageReturn;
    }
    
    public Godot.Collections.Array<Godot.Collections.Array<string> > AvailablePages()
    {
        return ConstructPageArray(Simulation.Instance.Query<(string, string)[]>(ClientQuery.AvailablePages));
    }

    public Godot.Collections.Array<Godot.Collections.Array<string> > AvailableGlobalPages()
    {
        return ConstructPageArray(Simulation.Instance.Query<(string, string)[]>(ClientQuery.AvailableGlobalPages));
    }
    
    public Godot.Collections.Array<Godot.Collections.Array<string> > AvailableSettlementPages(string settlementUuid)
    {
        return ConstructPageArray(Simulation.Instance.Query<(string, string)[], string>(ClientQuery.AvailableSettlementPages, settlementUuid));
    }
    
    public string[] MessageHistory()
    {
        return Simulation.Instance.Query<string[]>(ClientQuery.MessageHistory);
    }
    
    public string[] UnreadMessages()
    {
        return Simulation.Instance.Query<string[]>(ClientQuery.UnreadMessages);
    }

    public bool HasUnlockedPage(string pageId)
    {
        return Simulation.Instance.Query<bool, string>(ClientQuery.HasUnlockedPage, pageId);
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

    public string ResourceCategoryName(string resourceCategoryId)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.ResourceCategoryName, resourceCategoryId);
    }
    
    public string ResourceCategoryDescription(string resourceCategoryId)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.ResourceCategoryDescription, resourceCategoryId);
    }
    
    public string ResourceName(string resourceId)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.ResourceName, resourceId);
    }
    
    public string ResourceDescription(string resourceId)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.ResourceDescription, resourceId);
    }
    
    // ----------------------
    // LOCATIONS
    // ----------------------

    public string[] Locations()
    {
        return Simulation.Instance.Query<string[]>(ClientQuery.Locations);
    }
    
    public string[] LocationResources(string locationUuid)
    {
        return Simulation.Instance.Query<string[], string>(ClientQuery.LocationResources, locationUuid);
    }

    public float LocationResourceAmount(string locationUuid, string resourceId)
    {
        return Simulation.Instance.Query<float, string, string>(ClientQuery.LocationResourceAmount, locationUuid, resourceId );
    }

    // ----------------------
    // SETTLEMENTS
    // ----------------------
    
    public string[] Settlements()
    {
        return Simulation.Instance.Query<string[]>(ClientQuery.Settlements);
    }

    public string SettlementLocation(string uuid)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.SettlementLocation, uuid);
    }
    
    public string SettlementName(string uuid)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.SettlementName, uuid);
    }
    
    public int SettlementCurrentPopulation(string uuid)
    {
        return Simulation.Instance.Query<int, string>(ClientQuery.SettlementCurrentPopulation, uuid);
    }

    public int SettlementPopulationMax(string uuid)
    {
        return Simulation.Instance.Query<int, string>(ClientQuery.SettlementPopulationMax, uuid);
    }
    
    public string[] SettlementPopulationMembers(string uuid)
    {
        return Simulation.Instance.Query<string[], string>(ClientQuery.SettlementPopulationMembers, uuid);
    }
    
    public string[] SettlementPopulationSpecies(string uuid)
    {
        return Simulation.Instance.Query<string[], string>(ClientQuery.SettlementPopulationSpecies, uuid);
    }

    public string[] SettlementPopulationSpeciesMembers(string uuid, string speciesId)
    {
        return Simulation.Instance.Query<string[], string, string>(ClientQuery.SettlementPopulationSpeciesMembers, uuid, speciesId);
    }

    public string[] SettlementInventoryCategories(string uuid)
    {
        return Simulation.Instance.Query<string[], string>(ClientQuery.SettlementInventoryCategories, uuid);
    }
    
    public string[] SettlementInventoryResources(string uuid, string categoryId)
    {
        return Simulation.Instance.Query<string[], string, string>(ClientQuery.SettlementInventoryResources, uuid, categoryId);
    }

    public float SettlementInventoryResourceAmount(string uuid, string resourceId)
    {
        return Simulation.Instance.Query<float, string, string>(ClientQuery.SettlementInventoryResourceAmount, uuid, resourceId);
    }
    
    public float SettlementInventoryResourceCapacity(string uuid, string resourceId)
    {
        return Simulation.Instance.Query<float, string, string>(ClientQuery.SettlementInventoryResourceCapacity, uuid, resourceId);
    }

    public float SettlementInventoryResourceDelta(string uuid, string resourceId)
    {
        return Simulation.Instance.Query<float, string, string>(ClientQuery.SettlementInventoryResourceDelta, uuid, resourceId);
    }
    
    public float SettlementAvailableFoodPortion(string uuid)
    {
        return Simulation.Instance.Query<float, string>(ClientQuery.SettlementAvailableFoodPortion, uuid);
    }
    
    public string[] SettlementBuildings(string uuid)
    {
        return Simulation.Instance.Query<string[], string>(ClientQuery.SettlementBuildings, uuid);
    }

    public int SettlementBuildingsAmount(string uuid, string buildingId)
    {
        return Simulation.Instance.Query<int, string, string>(ClientQuery.SettlementBuildingsAmount, uuid, buildingId);
    }
    
    public string SettlementBuildingDisplayName(string uuid, string buildingId)
    {
        return Simulation.Instance.Query<string, string, string>(ClientQuery.SettlementBuildingDisplayName, uuid, buildingId);
    }
    
    public string[] SettlementBuildingDescription(string uuid, string buildingId)
    {
        return Simulation.Instance.Query<string[], string, string>(ClientQuery.SettlementBuildingDescription, uuid, buildingId);
    }

    public bool SettlementBuildingCanAfford(string uuid, string buildingId)
    {
        return Simulation.Instance.Query<bool, string, string>(ClientQuery.SettlementBuildingCanAfford, uuid, buildingId);
    }
    
    public string[] SettlementBuildingResourceList(string uuid, string buildingId)
    {
        return Simulation.Instance.Query<string[], string, string>(ClientQuery.SettlementBuildingResourceList, uuid, buildingId);
    }

    public float SettlementBuildingSingleResourceCost(string uuid, string buildingId, string resourceId)
    {
        return Simulation.Instance.Query<float, string, string, string>(ClientQuery.SettlementBuildingSingleResourceCost, uuid, buildingId, resourceId);
    }
    
    
    // ----------------------
    // POPULATION
    // ----------------------
    
    public string PopulationMemberName(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<string, string, string>(ClientQuery.PopulationMemberName, settlementUuid, populationUuid);
    }

    public string PopulationMemberSpecies(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<string, string, string>(ClientQuery.PopulationMemberSpecies, settlementUuid, populationUuid);
    }

    public string PopulationMemberTask(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<string, string, string>(ClientQuery.PopulationMemberTask, settlementUuid, populationUuid);
    }

    public string PopulationMemberCurrentAction(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<string, string, string>(ClientQuery.PopulationMemberCurrentAction, settlementUuid, populationUuid);
    }
    
    public float PopulationMemberCurrentActionProgress(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<float, string, string>(ClientQuery.PopulationMemberCurrentActionProgress, settlementUuid, populationUuid);
    }

    public string PopulationMemberCurrentActionName(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<string, string, string>(ClientQuery.PopulationMemberCurrentActionName, settlementUuid, populationUuid);
    }
    
    public string PopulationMemberState(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<string, string, string>(ClientQuery.PopulationMemberState, settlementUuid, populationUuid);
    }

    public string PopulationMemberWaitMessage(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<string, string, string>(ClientQuery.PopulationMemberWaitMessage, settlementUuid, populationUuid);
    }
    
    public float PopulationMemberInventoryProgress(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<float, string, string>(ClientQuery.PopulationMemberInventoryProgress, settlementUuid, populationUuid);
    }

    public float PopulationMemberHunger(string settlementUuid, string populationUuid)
    {
        return Simulation.Instance.Query<float, string, string>(ClientQuery.PopulationMemberHunger, settlementUuid, populationUuid);
    }

    
    // ----------------------
    // UPGRADES
    // ----------------------

    public string[] UpgradesAvailable()
    {
        return Simulation.Instance.Query<string[]>(ClientQuery.UpgradesAvailable);
    }

    public string[] UpgradesUnlocked()
    {
        return Simulation.Instance.Query<string[]>(ClientQuery.UpgradesUnlocked);
    }
    
    public string UpgradeDisplayName(string upgradeId)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.UpgradeDisplayName, upgradeId);
    }

    public string UpgradeDescription(string upgradeId)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.UpgradeDescription, upgradeId);
    }

    public bool UpgradeCanAfford(string upgradeId)
    {
        return Simulation.Instance.Query<bool, string>(ClientQuery.UpgradeCanAfford, upgradeId);
    }

    public string[] UpgradeResourceList(string upgradeId)
    {
        return Simulation.Instance.Query<string[], string>(ClientQuery.UpgradeResourceList, upgradeId);
    }

    public float UpgradeSingleResourceCost(string researchId, string upgradeId)
    {
        return Simulation.Instance.Query<float, string, string>(ClientQuery.UpgradeSingleResourceCost,researchId, upgradeId);
    }

    
    // ----------------------
    // SPECIES
    // ----------------------
    
    public string SpeciesName(string speciesId)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.SpeciesName, speciesId);
    }
    
    public string[] SpeciesDescription(string speciesId)
    {
        return Simulation.Instance.Query<string[], string>(ClientQuery.SpeciesDescription, speciesId);
    }
    
    // ----------------------
    // TASKS
    // ----------------------
    
    public string[] Tasks()
    {
        return Simulation.Instance.Query<string[]>(ClientQuery.Tasks);
    }
  
    public bool TaskUnlocked(string taskId)
    {
        return Simulation.Instance.Query<bool, string>(ClientQuery.TaskUnlocked, taskId);
    }
    
    public string TaskName(string taskId)
    {
        return Simulation.Instance.Query<string, string>(ClientQuery.TaskName, taskId);
    }
    
    public string[] SettlementTaskDescription(string settlementId, string taskId)
    {
        return Simulation.Instance.Query<string[], string, string>(ClientQuery.SettlementTaskDescription, settlementId, taskId);
    }

    public string[] SettlementTaskAssignments(string settlementUuid, string taskId)
    {
        return Simulation.Instance.Query<string[], string, string>(ClientQuery.SettlementTaskAssignments, settlementUuid, taskId);
    }
    
    public int SettlementTaskAssignedNum(string settlementUuid, string taskId)
    {
        return Simulation.Instance.Query<int, string, string>(ClientQuery.SettlementTaskAssignedNum, settlementUuid, taskId);
    }
    
    public int SettlementTaskMaximumCapacity(string settlementUuid, string taskId)
    {
        return Simulation.Instance.Query<int, string, string>(ClientQuery.SettlementTaskMaximumCapacity, settlementUuid, taskId);
    }
    
}