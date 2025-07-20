using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LamiaUWP.Core;
using LamiaSimulation;

namespace LamiaUWP.Views
{
    /// <summary>
    /// Страница для отображения истории сообщений.
    /// </summary>
    public sealed partial class MessagesPage : Page
    {
        private GameController _gameController;
        private ObservableCollection<MessageViewModel> _messages;

        public MessagesPage()
        {
            this.InitializeComponent();
            _gameController = GameController.Instance;
            _messages = new ObservableCollection<MessageViewModel>();
            MessagesListView.ItemsSource = _messages;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadMessages();
        }

        /// <summary>
        /// Загружает историю сообщений из симуляции.
        /// </summary>
        private void LoadMessages()
        {
            _messages.Clear();
            var messageHistory = _gameController.Query.MessageHistory();
            
            // Отображаем сообщения в обратном порядке (новые сверху)
            for (int i = messageHistory.Count - 1; i >= 0; i--)
            {
                var message = messageHistory[i];
                _messages.Add(new MessageViewModel
                {
                    Title = message.Title,
                    Content = message.Content,
                    Timestamp = message.Timestamp,
                    TimestampFormatted = FormatTimestamp(message.Timestamp)
                });
            }
        }

        /// <summary>
        /// Форматирует временную метку сообщения в удобочитаемый формат.
        /// </summary>
        private string FormatTimestamp(float timestamp)
        {
            // Преобразование временной метки в формат дней и часов
            int totalHours = (int)timestamp;
            int days = totalHours / 24;
            int hours = totalHours % 24;
            
            return $"День {days + 1}, {hours:D2}:00";
        }

        /// <summary>
        /// Обработчик события нажатия на сообщение в списке.
        /// </summary>
        private void MessagesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is MessageViewModel message)
            {
                // Здесь можно добавить логику для отображения подробной информации о сообщении
                // Например, открыть диалоговое окно с полным текстом сообщения
                ShowMessageDetails(message);
            }
        }

        /// <summary>
        /// Отображает подробную информацию о выбранном сообщении.
        /// </summary>
        private async void ShowMessageDetails(MessageViewModel message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = message.Title,
                Content = message.Content,
                CloseButtonText = "Закрыть"
            };

            await dialog.ShowAsync();
        }
    }

    /// <summary>
    /// Модель представления для сообщения.
    /// </summary>
    public class MessageViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public float Timestamp { get; set; }
        public string TimestampFormatted { get; set; }
    }
}