using System;

namespace LamiaSimulation
{
    /// <summary>
    /// Перечисление типов действий, которые клиент может отправить симуляции.
    /// </summary>
    public enum ClientAction
    {
        // Страницы
        UnlockPage,
        
        // Локации и поселения
        AddLocation,
        AddSettlementAtLocation,
        RenameSettlement,
        
        // Население
        AddPopulation,
        PopulationAssignToTask,
        SettlementRemovePopulation,
        
        // Сообщения
        SendMessage,
        
        // Задачи
        UnlockTask,
        
        // Здания
        UnlockBuilding,
        SettlementPurchaseBuilding,
        
        // Ресурсы
        AddResourceToSettlementInventory,
        SubtractResourceFromSettlementInventory,
        SettlementTakeAvailableFoodPortion,
        
        // Исследования
        UnlockResearch,
        ForceUnlockResearch
    }
}