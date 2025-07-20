using System;

namespace LamiaSimulation
{
    /// <summary>
    /// Интерфейс для объектов, которые могут получать действия от клиента.
    /// </summary>
    public interface IActionReceiver
    {
        /// <summary>
        /// Выполняет действие клиента.
        /// </summary>
        /// <param name="action">Тип действия.</param>
        /// <param name="args">Аргументы действия.</param>
        void PerformAction(ClientAction action, params object[] args);
    }
    
    /// <summary>
    /// Интерфейс для объектов, которые могут отвечать на запросы клиента.
    /// </summary>
    /*public interface IQueryable
    {
        /// <summary>
        /// Выполняет запрос клиента и возвращает результат.
        /// </summary>
        /// <typeparam name="T">Тип результата запроса.</typeparam>
        /// <param name="query">Тип запроса.</param>
        /// <returns>Результат запроса.</returns>
        T Query<T>(ClientQuery query);
        
        /// <summary>
        /// Выполняет запрос клиента с одним аргументом и возвращает результат.
        /// </summary>
        /// <typeparam name="T">Тип результата запроса.</typeparam>
        /// <typeparam name="T1">Тип первого аргумента.</typeparam>
        /// <param name="query">Тип запроса.</param>
        /// <param name="arg1">Первый аргумент.</param>
        /// <returns>Результат запроса.</returns>
        T Query<T, T1>(ClientQuery query, T1 arg1);
        
        /// <summary>
        /// Выполняет запрос клиента с двумя аргументами и возвращает результат.
        /// </summary>
        /// <typeparam name="T">Тип результата запроса.</typeparam>
        /// <typeparam name="T1">Тип первого аргумента.</typeparam>
        /// <typeparam name="T2">Тип второго аргумента.</typeparam>
        /// <param name="query">Тип запроса.</param>
        /// <param name="arg1">Первый аргумент.</param>
        /// <param name="arg2">Второй аргумент.</param>
        /// <returns>Результат запроса.</returns>
        T Query<T, T1, T2>(ClientQuery query, T1 arg1, T2 arg2);
        
        /// <summary>
        /// Выполняет запрос клиента и сохраняет результат в объекте QueryResult.
        /// </summary>
        /// <param name="query">Тип запроса.</param>
        /// <param name="result">Объект для сохранения результата.</param>
        void Query(ClientQuery query, ref QueryResult result);
        
        /// <summary>
        /// Выполняет запрос клиента с одним аргументом и сохраняет результат в объекте QueryResult.
        /// </summary>
        /// <param name="query">Тип запроса.</param>
        /// <param name="result">Объект для сохранения результата.</param>
        /// <param name="arg1">Первый аргумент.</param>
        void Query(ClientQuery query, ref QueryResult result, object arg1);
        
        /// <summary>
        /// Выполняет запрос клиента с двумя аргументами и сохраняет результат в объекте QueryResult.
        /// </summary>
        /// <param name="query">Тип запроса.</param>
        /// <param name="result">Объект для сохранения результата.</param>
        /// <param name="arg1">Первый аргумент.</param>
        /// <param name="arg2">Второй аргумент.</param>
        void Query(ClientQuery query, ref QueryResult result, object arg1, object arg2);
    }*/
    
    /// <summary>
    /// Интерфейс для объектов, которые могут быть симулированы.
    /// </summary>
    public interface ISimulated
    {
        /// <summary>
        /// Выполняет симуляцию объекта на указанный временной шаг.
        /// </summary>
        /// <param name="deltaTime">Временной шаг симуляции.</param>
        void Simulate(float deltaTime);
    }
}