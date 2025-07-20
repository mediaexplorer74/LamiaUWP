using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LamiaUWP.Core;
using LamiaSimulation;
using System.Linq;

namespace LamiaUWP.Views
{
    /// <summary>
    /// Страница обзора, отображающая основную информацию о текущем поселении, последние сообщения и доступные задачи.
    /// </summary>
    public sealed partial class OverviewPage : Page
    {
        private GameController _gameController;
        private ObservableCollection<ResourceItem> _resources;
        private ObservableCollection<MessageItem> _messages;
        private ObservableCollection<TaskItem> _tasks;

        public OverviewPage()
        {
            this.InitializeComponent();
            _gameController = GameController.Instance;
            
            // Инициализация коллекций для привязки данных
            _resources = new ObservableCollection<ResourceItem>();
            _messages = new ObservableCollection<MessageItem>();
            _tasks = new ObservableCollection<TaskItem>();
            
            ResourcesListView.ItemsSource = _resources;
            MessagesListView.ItemsSource = _messages;
            TasksListView.ItemsSource = _tasks;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateUI();
        }

        /// <summary>
        /// Обновляет пользовательский интерфейс актуальными данными из симуляции.
        /// </summary>
        private void UpdateUI()
        {
            UpdateSettlementInfo();
            UpdateMessages();
            UpdateTasks();
        }

        /// <summary>
        /// Обновляет информацию о текущем поселении.
        /// </summary>
        private void UpdateSettlementInfo()
        {
            string settlementId = _gameController.CurrentSettlementUuid;
            if (string.IsNullOrEmpty(settlementId))
                return;

            // Получение информации о поселении
            string name = _gameController.Query.SettlementName(settlementId);
            int population = _gameController.Query.SettlementPopulation(settlementId);
            
            // Обновление текстовых блоков
            SettlementNameTextBlock.Text = name;
            PopulationTextBlock.Text = $"Население: {population}";
            
            // Обновление списка ресурсов
            _resources.Clear();
            var resourceCategories = _gameController.Query.ResourceCategories();
            foreach (var category in resourceCategories)
            {
                var resourcesInCategory = _gameController.Query.ResourcesInCategory(category);
                foreach (var resourceId in resourcesInCategory)
                {
                    string resourceName = _gameController.Query.ResourceName(resourceId);
                    float amount = _gameController.Query.SettlementResourceAmount(settlementId, resourceId);
                    
                    if (amount > 0)
                    {
                        _resources.Add(new ResourceItem
                        {
                            Id = resourceId,
                            Name = resourceName,
                            Amount = amount.ToString("F1")
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Обновляет список последних сообщений.
        /// </summary>
        private void UpdateMessages()
        {
            _messages.Clear();
            var messageHistory = _gameController.Query.MessageHistory();
            
            // Отображаем только последние 5 сообщений
            int count = Math.Min(messageHistory.Count, 5);
            for (int i = messageHistory.Count - 1; i >= messageHistory.Count - count; i--)
            {
                var message = messageHistory[i];
                _messages.Add(new MessageItem
                {
                    Title = message.Title,
                    Content = message.Content,
                    Timestamp = message.Timestamp
                });
            }
        }

        /// <summary>
        /// Обновляет список доступных задач.
        /// </summary>
        private void UpdateTasks()
        {
            _tasks.Clear();
            var availableTasks = _gameController.Query.AvailableTasks();
            
            foreach (var taskId in availableTasks)
            {
                string name = _gameController.Query.TaskName(taskId);
                string description = default;//_gameController.Query.TaskDescription(taskId);
                
                _tasks.Add(new TaskItem
                {
                    Id = taskId,
                    Name = name,
                    Description = description
                });
            }
        }

        /// <summary>
        /// Обработчик события нажатия на задачу в списке.
        /// </summary>
        private void TasksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is TaskItem task)
            {
                // Разблокировка выбранной задачи
                _gameController.Action.UnlockTask(task.Id);
                
                // Обновление интерфейса после разблокировки задачи
                UpdateUI();
            }
        }
    }

    /// <summary>
    /// Класс для представления ресурса в пользовательском интерфейсе.
    /// </summary>
    public class ResourceItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
    }

    /// <summary>
    /// Класс для представления сообщения в пользовательском интерфейсе.
    /// </summary>
    public class MessageItem
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public float Timestamp { get; set; }
    }

    /// <summary>
    /// Класс для представления задачи в пользовательском интерфейсе.
    /// </summary>
    public class TaskItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}