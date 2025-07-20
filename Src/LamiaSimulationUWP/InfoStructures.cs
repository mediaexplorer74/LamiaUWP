using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    /// <summary>
    /// Класс для хранения информации о сообщении.
    /// </summary>
    public class Message
    {
        public string Title;
        public string Content;
        public float Timestamp;

        /// <summary>
        /// Уникальный идентификатор сообщения.
        /// </summary>
        public string Uuid { get; set; }
        
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Время создания сообщения (в игровых днях).
        /// </summary>
        public int Day { get; set; }
        
        /// <summary>
        /// Флаг, указывающий, было ли сообщение прочитано.
        /// </summary>
        public bool IsRead { get; set; }
    }
    
    /// <summary>
    /// Класс для хранения информации о поселении.
    /// </summary>
    public class SettlementInfo
    {
        /// <summary>
        /// Уникальный идентификатор поселения.
        /// </summary>
        public string Uuid { get; set; }
        
        /// <summary>
        /// Название поселения.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Тип локации, на которой расположено поселение.
        /// </summary>
        public string LocationType { get; set; }
        
        /// <summary>
        /// Список идентификаторов населения в поселении.
        /// </summary>
        public string[] Population { get; set; }
        
        /// <summary>
        /// Список идентификаторов зданий в поселении.
        /// </summary>
        public string[] Buildings { get; set; }
    }
    
    /// <summary>
    /// Класс для хранения информации о здании.
    /// </summary>
    public class BuildingInfo
    {
        /// <summary>
        /// Идентификатор типа здания.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Отображаемое название здания.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Описание здания.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Словарь ресурсов, необходимых для постройки здания.
        /// </summary>
        public Dictionary<string, float> ResourceCosts { get; set; }
        
        /// <summary>
        /// Словарь ресурсов, которые производит здание.
        /// </summary>
        public Dictionary<string, float> ResourceProduction { get; set; }
        
        /// <summary>
        /// Словарь ресурсов, которые потребляет здание.
        /// </summary>
        public Dictionary<string, float> ResourceConsumption { get; set; }
    }
    
    /// <summary>
    /// Класс для хранения информации об исследовании.
    /// </summary>
    public class ResearchInfo
    {
        /// <summary>
        /// Идентификатор исследования.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Отображаемое название исследования.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Описание исследования.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Словарь ресурсов, необходимых для проведения исследования.
        /// </summary>
        public Dictionary<string, float> ResourceCosts { get; set; }
        
        /// <summary>
        /// Список идентификаторов исследований, которые являются предпосылками для этого исследования.
        /// </summary>
        public string[] Prerequisites { get; set; }
    }
    
    /// <summary>
    /// Класс для хранения информации об улучшении.
    /// </summary>
    public class UpgradeInfo
    {
        /// <summary>
        /// Идентификатор улучшения.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Отображаемое название улучшения.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Описание улучшения.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Словарь ресурсов, необходимых для приобретения улучшения.
        /// </summary>
        public Dictionary<string, float> ResourceCosts { get; set; }
        
        /// <summary>
        /// Список идентификаторов исследований, которые являются предпосылками для этого улучшения.
        /// </summary>
        public string[] Prerequisites { get; set; }
    }
    
    /// <summary>
    /// Класс для хранения информации о задаче.
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// Идентификатор задачи.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Отображаемое название задачи.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Описание задачи.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Словарь ресурсов, которые производит задача.
        /// </summary>
        public Dictionary<string, float> ResourceProduction { get; set; }
    }
}