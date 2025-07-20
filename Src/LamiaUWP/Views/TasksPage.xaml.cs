using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LamiaUWP.Core;

namespace LamiaUWP.Views
{
    /// <summary>
    /// Страница для управления задачами.
    /// </summary>
    public sealed partial class TasksPage : Page
    {
        private GameController _gameController;
        private ObservableCollection<TaskItem> _availableTasks;
        private ObservableCollection<TaskItem> _completedTasks;

        public TasksPage()
        {
            this.InitializeComponent();
            _gameController = GameController.Instance;
            
            // Инициализация коллекций для привязки данных
            _availableTasks = new ObservableCollection<TaskItem>();
            _completedTasks = new ObservableCollection<TaskItem>();
            
            AvailableTasksListView.ItemsSource = _availableTasks;
            CompletedTasksListView.ItemsSource = _completedTasks;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadTasks();
        }

        /// <summary>
        /// Загружает списки доступных и выполненных задач из симуляции.
        /// </summary>
        private void LoadTasks()
        {
            LoadAvailableTasks();
            LoadCompletedTasks();
        }

        /// <summary>
        /// Загружает список доступных задач.
        /// </summary>
        private void LoadAvailableTasks()
        {
            _availableTasks.Clear();
            var availableTaskIds = _gameController.Query.AvailableTasks();
            var unlockedTaskIds = _gameController.Query.UnlockedTasks();
            
            // Фильтруем задачи, которые доступны, но еще не разблокированы
            var taskIds = availableTaskIds.Where(id => !unlockedTaskIds.Contains(id)).ToList();
            
            foreach (var id in taskIds)
            {
                string name = _gameController.Query.TaskName(id);
                string description = _gameController.Query.TaskDescription(id);
                
                _availableTasks.Add(new TaskItem
                {
                    Id = id,
                    Name = name,
                    Description = description
                });
            }
        }

        /// <summary>
        /// Загружает список выполненных задач.
        /// </summary>
        private void LoadCompletedTasks()
        {
            _completedTasks.Clear();
            var unlockedTaskIds = _gameController.Query.UnlockedTasks();
            
            foreach (var id in unlockedTaskIds)
            {
                string name = _gameController.Query.TaskName(id);
                string description = _gameController.Query.TaskDescription(id);
                
                _completedTasks.Add(new TaskItem
                {
                    Id = id,
                    Name = name,
                    Description = description
                });
            }
        }

        /// <summary>
        /// Обработчик события нажатия на доступную задачу в списке.
        /// </summary>
        private void AvailableTasksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is TaskItem task)
            {
                // Разблокировка выбранной задачи
                _gameController.Action.UnlockTask(task.Id);
                
                // Обновление списков задач
                LoadTasks();
                
                // Показать сообщение о выполнении задачи
                ShowTaskCompletedMessage(task);
            }
        }

        /// <summary>
        /// Обработчик события нажатия на выполненную задачу в списке.
        /// </summary>
        private void CompletedTasksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is TaskItem task)
            {
                // Показать подробную информацию о выполненной задаче
                ShowTaskDetailsMessage(task);
            }
        }

        /// <summary>
        /// Отображает сообщение о выполнении задачи.
        /// </summary>
        private async void ShowTaskCompletedMessage(TaskItem task)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Задача выполнена",
                Content = $"Вы успешно выполнили задачу \"{task.Name}\".",
                CloseButtonText = "Закрыть"
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Отображает подробную информацию о выполненной задаче.
        /// </summary>
        private async void ShowTaskDetailsMessage(TaskItem task)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = task.Name,
                Content = task.Description,
                CloseButtonText = "Закрыть"
            };

            await dialog.ShowAsync();
        }
    }

    /// <summary>
    /// Класс для представления задачи в пользовательском интерфейсе.
    /// </summary>
    /*public class TaskItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }*/
}