using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    /// <summary>
    /// Класс для хранения результатов запросов к симуляции.
    /// </summary>
    public class QueryResult
    {
        // Базовые типы
        public string String { get; set; }
        public int Int { get; set; }
        public float Float { get; set; }
        public bool Bool { get; set; }
        
        // Массивы
        public string[] StringArray { get; set; }
        public int[] IntArray { get; set; }
        public float[] FloatArray { get; set; }
        public bool[] BoolArray { get; set; }
        public Message[] MessageArray { get; set; }
        
        // Словари
        public Dictionary<string, float> ResourceDictionary { get; set; }
        
        // Сложные типы
        public SettlementInfo SettlementInfo { get; set; }
        public BuildingInfo BuildingInfo { get; set; }
        public ResearchInfo ResearchInfo { get; set; }
        public UpgradeInfo UpgradeInfo { get; set; }
        public TaskInfo TaskInfo { get; set; }
        
        /// <summary>
        /// Создает новый экземпляр класса QueryResult.
        /// </summary>
        public QueryResult()
        {
            // Инициализация свойств по умолчанию
            String = string.Empty;
            Int = 0;
            Float = 0f;
            Bool = false;
            
            StringArray = Array.Empty<string>();
            IntArray = Array.Empty<int>();
            FloatArray = Array.Empty<float>();
            BoolArray = Array.Empty<bool>();
            MessageArray = Array.Empty<Message>();
            
            ResourceDictionary = new Dictionary<string, float>();
            
            SettlementInfo = null;
            BuildingInfo = null;
            ResearchInfo = null;
            UpgradeInfo = null;
            TaskInfo = null;
        }
        
        /// <summary>
        /// Очищает все свойства объекта.
        /// </summary>
        public void Clear()
        {
            String = string.Empty;
            Int = 0;
            Float = 0f;
            Bool = false;
            
            StringArray = Array.Empty<string>();
            IntArray = Array.Empty<int>();
            FloatArray = Array.Empty<float>();
            BoolArray = Array.Empty<bool>();
            MessageArray = Array.Empty<Message>();
            
            ResourceDictionary.Clear();
            
            SettlementInfo = null;
            BuildingInfo = null;
            ResearchInfo = null;
            UpgradeInfo = null;
            TaskInfo = null;
        }
    }
}