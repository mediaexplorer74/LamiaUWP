using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    /// <summary>
    /// Класс для работы с типами задач.
    /// </summary>
    public class TaskType
    {
        /// <summary>
        /// Идентификатор задачи.
        /// </summary>
        public string id;
        
        /// <summary>
        /// Название задачи.
        /// </summary>
        public string name;
        
        /// <summary>
        /// Описание задачи.
        /// </summary>
        public string description;
        
        /// <summary>
        /// Словарь ресурсов, которые производит задача.
        /// </summary>
        public Dictionary<string, float> resourceProduction;
        
        // Статический словарь для хранения всех типов задач
        private static Dictionary<string, TaskType> allTasks = new Dictionary<string, TaskType>();
        
        /// <summary>
        /// Статический конструктор для инициализации типов задач.
        /// </summary>
        static TaskType()
        {
            // Инициализация базовых типов задач
            RegisterTask(new TaskType
            {
                id = "idle",
                name = "Отдых",
                description = "Персонаж отдыхает и не производит ресурсов.",
                resourceProduction = new Dictionary<string, float>()
            });
            
            RegisterTask(new TaskType
            {
                id = "gather_food",
                name = "Сбор пищи",
                description = "Персонаж собирает пищу в окрестностях поселения.",
                resourceProduction = new Dictionary<string, float>
                {
                    { "food", 1.0f }
                }
            });
            
            RegisterTask(new TaskType
            {
                id = "gather_wood",
                name = "Сбор древесины",
                description = "Персонаж собирает древесину в окрестностях поселения.",
                resourceProduction = new Dictionary<string, float>
                {
                    { "wood", 1.0f }
                }
            });
            
            RegisterTask(new TaskType
            {
                id = "gather_stone",
                name = "Сбор камня",
                description = "Персонаж собирает камень в окрестностях поселения.",
                resourceProduction = new Dictionary<string, float>
                {
                    { "stone", 1.0f }
                }
            });
        }
        
        /// <summary>
        /// Регистрирует новый тип задачи.
        /// </summary>
        /// <param name="task">Тип задачи для регистрации.</param>
        public static void RegisterTask(TaskType task)
        {
            if (!allTasks.ContainsKey(task.id))
            {
                allTasks.Add(task.id, task);
            }
        }
        
        /// <summary>
        /// Получает тип задачи по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор задачи.</param>
        /// <returns>Тип задачи или null, если задача не найдена.</returns>
        public static TaskType GetTaskById(string id)
        {
            if (allTasks.TryGetValue(id, out TaskType task))
            {
                return task;
            }
            
            return null;
        }
        
        /// <summary>
        /// Получает все зарегистрированные типы задач.
        /// </summary>
        /// <returns>Массив типов задач.</returns>
        public static TaskType[] GetAllTasks()
        {
            TaskType[] result = new TaskType[allTasks.Count];
            allTasks.Values.CopyTo(result, 0);
            return result;
        }
    }
}