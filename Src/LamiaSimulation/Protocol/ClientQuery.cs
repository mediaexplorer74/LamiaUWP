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
        CurrentDay
    }
}