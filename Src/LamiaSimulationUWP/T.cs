using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    /// <summary>
    /// Класс для локализации текстов.
    /// </summary>
    /*public static class T
    {
        // Словарь для хранения переводов
        private static Dictionary<string, string> translations = new Dictionary<string, string>();
        
        // Текущий язык
        private static string currentLanguage = "ru";
        
        /// <summary>
        /// Статический конструктор для инициализации переводов.
        /// </summary>
        static T()
        {
            // Инициализация базовых переводов
            InitializeTranslations();
        }
        
        /// <summary>
        /// Инициализирует базовые переводы.
        /// </summary>
        private static void InitializeTranslations()
        {
            // Здесь можно добавить базовые переводы
            // Например, для русского языка:
            if (currentLanguage == "ru")
            {
                // Общие
                translations["app_name"] = "Lamia";
                
                // Страницы
                translations["page_main"] = "Главная";
                translations["page_settlement"] = "Поселение";
                translations["page_research"] = "Исследования";
                translations["page_upgrades"] = "Улучшения";
                translations["page_messages"] = "Сообщения";
                
                // Ресурсы
                translations["resource_food"] = "Пища";
                translations["resource_wood"] = "Древесина";
                translations["resource_stone"] = "Камень";
                translations["resource_metal"] = "Металл";
                
                // Категории ресурсов
                translations["resource_category_basic"] = "Базовые ресурсы";
                translations["resource_category_advanced"] = "Продвинутые ресурсы";
                
                // Задачи
                translations["task_idle"] = "Отдых";
                translations["task_gather_food"] = "Сбор пищи";
                translations["task_gather_wood"] = "Сбор древесины";
                translations["task_gather_stone"] = "Сбор камня";
                
                // Здания
                translations["building_hut"] = "Хижина";
                translations["building_storehouse"] = "Склад";
                translations["building_farm"] = "Ферма";
                translations["building_mine"] = "Шахта";
                
                // Исследования
                translations["research_agriculture"] = "Сельское хозяйство";
                translations["research_mining"] = "Горное дело";
                translations["research_construction"] = "Строительство";
                translations["research_metallurgy"] = "Металлургия";
            }
        }
        
        /// <summary>
        /// Устанавливает текущий язык.
        /// </summary>
        /// <param name="language">Код языка (например, "ru", "en").</param>
        public static void SetLanguage(string language)
        {
            currentLanguage = language;
            InitializeTranslations();
        }
        
        /// <summary>
        /// Получает перевод для указанного ключа.
        /// </summary>
        /// <param name="key">Ключ перевода.</param>
        /// <returns>Переведенный текст или ключ, если перевод не найден.</returns>
        //public static string _(string key)
        //{
        //    if (translations.TryGetValue(key, out string translation))
        //    {
        //        return translation;
        //    }
        //    
        //    return key;
        //}
        
        /// <summary>
        /// Добавляет новый перевод.
        /// </summary>
        /// <param name="key">Ключ перевода.</param>
        /// <param name="translation">Переведенный текст.</param>
        public static void AddTranslation(string key, string translation)
        {
            translations[key] = translation;
        }
    }*/
}