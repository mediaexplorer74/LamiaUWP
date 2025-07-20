using System;

namespace LamiaSimulation
{
    /// <summary>
    /// Базовый класс для аргументов событий симуляции.
    /// </summary>
    public class SimulationEventArgs : EventArgs
    {
    }
    
    /// <summary>
    /// Аргументы события разблокировки страницы.
    /// </summary>
    /*public class UnlockedPageEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Идентификатор разблокированной страницы.
        /// </summary>
        public string PageId { get; set; }
    }*/
    
    /// <summary>
    /// Аргументы события появления нового ресурса в поселении.
    /// </summary>
    /*public class SettlementHasNewResourceEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Идентификатор поселения.
        /// </summary>
        public string SettlementUuid { get; set; }
        
        /// <summary>
        /// Идентификатор ресурса.
        /// </summary>
        public string ResourceId { get; set; }
    }*/
    
    /// <summary>
    /// Аргументы события появления нового населения в поселении.
    /// </summary>
    /*public class SettlementSpawnedNewPopulationEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Идентификатор поселения.
        /// </summary>
        public string SettlementUuid { get; set; }
        
        /// <summary>
        /// Идентификатор нового населения.
        /// </summary>
        public string PopulationUuid { get; set; }
    }*/
    
    /// <summary>
    /// Аргументы события покупки здания в поселении.
    /// </summary>
    /*public class SettlementBuildingPurchasedEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Идентификатор поселения.
        /// </summary>
        public string SettlementUuid { get; set; }
        
        /// <summary>
        /// Идентификатор здания.
        /// </summary>
        public string BuildingId { get; set; }
    }*/
    
    /// <summary>
    /// Аргументы события разблокировки исследования.
    /// </summary>
    public class ResearchUnlockedEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Идентификатор разблокированного исследования.
        /// </summary>
        public string ResearchId { get; set; }
    }
    
    /// <summary>
    /// Аргументы события разблокировки улучшения.
    /// </summary>
    public class UpgradeUnlockedEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Идентификатор разблокированного улучшения.
        /// </summary>
        public string UpgradeId { get; set; }
    }
    
    /// <summary>
    /// Аргументы события разблокировки задачи.
    /// </summary>
    public class TaskUnlockedEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Идентификатор разблокированной задачи.
        /// </summary>
        public string TaskId { get; set; }
    }
    
    /// <summary>
    /// Аргументы события разблокировки здания.
    /// </summary>
    public class BuildingUnlockedEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Идентификатор разблокированного здания.
        /// </summary>
        public string BuildingId { get; set; }
    }
    
    /// <summary>
    /// Аргументы события добавления сообщения.
    /// </summary>
    public class MessageAddedEventArgs : SimulationEventArgs
    {
        /// <summary>
        /// Добавленное сообщение.
        /// </summary>
        public Message Message { get; set; }
    }
}