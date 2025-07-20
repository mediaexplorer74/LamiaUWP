using System;

namespace LamiaSimulation
{
    /// <summary>
    /// Перечисление типов запросов, которые клиент может отправить симуляции.
    /// </summary>
    public enum ClientQuery
    {
        // Страницы
        AvailablePages,
        AvailableGlobalPages,
        AvailableSettlementPages,
        HasUnlockedPage,
        PageDisplayName,
        
        // Сообщения
        MessageHistory,
        UnreadMessages,
        Messages,
        
        // Исследования
        ResearchAvailable,
        ResearchUnlocked,
        ResearchDisplayName,
        ResearchDescription,
        ResearchCanAfford,
        ResearchResourceList,
        ResearchSingleResourceCost,
        AvailableResearch,
        ResearchInfo,
        
        // Ресурсы
        ResourceCategories,
        ResourceCategoryDisplayName,
        ResourcesInCategory,
        ResourceDisplayName,
        ResourceDescription,
        ResourceCategory,
        
        // Поселения
        Settlements,
        SettlementName,
        SettlementInfo,
        SettlementPopulation,
        SettlementInventoryResources,
        SettlementInventoryResourceAmount,
        SettlementInventoryResourceCapacity,
        SettlementBuildings,
        SettlementResources,
        
        // Население
        PopulationName,
        PopulationSpecies,
        PopulationTask,
        
        // Задачи
        AvailableTasks,
        TaskDisplayName,
        TaskDescription,
        TaskInfo,
        
        // Здания
        BuildingDisplayName,
        BuildingDescription,
        BuildingCanAfford,
        BuildingResourceList,
        BuildingSingleResourceCost,
        BuildingInfo,
        HasUnlockedBuilding,
        
        // Улучшения
        AvailableUpgrades,
        UpgradeInfo,
        
        // Состояние игры
        CurrentDay,
        Locations,
        Tasks,
        UpgradesAvailable,
        UpgradesUnlocked,
        SpeciesName,
        SpeciesDescription,
        ResourceCategoryName,
        ResourceCategoryDescription,
        ResourceName,
        TaskName,
        TaskUnlocked,
        UpgradeDisplayName,
        UpgradeDescription,
        UpgradeCanAfford,
        UpgradeResourceList,
        UpgradeSingleResourceCost,
        SettlementBuildingsAmount,
        LocationResources,
        LocationResourceAmount,
        PopulationMemberCurrentAction,
        PopulationMemberCurrentActionProgress,
        PopulationMemberCurrentActionName,
        PopulationMemberState,
        PopulationMemberWaitMessage,
        PopulationMemberInventoryProgress,
        PopulationMemberHunger,
        PopulationMemberInventoryResourceAmount,
        SettlementAvailableFoodPortion,
        SettlementLocation,
        SettlementCurrentPopulation,
        SettlementPopulationMax,
        SettlementPopulationSpecies,
        SettlementPopulationMembers,
        SettlementInventoryCategories,
        SettlementTaskDescription,
        SettlementTaskAssignedNum,
        SettlementTaskMaximumCapacity,
        SettlementTaskAssignments,
        SettlementPopulationSpeciesMembers,
        PopulationMemberName,
        PopulationMemberSpecies,
        PopulationMemberTask,
        SettlementInventoryResourceDelta,
        SettlementBuildingDisplayName,
        SettlementBuildingDescription,
        SettlementBuildingCanAfford,
        SettlementBuildingResourceList,
        SettlementBuildingSingleResourceCost
    }
}