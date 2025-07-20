using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LamiaUWP.Core;
using LamiaUWP.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LamiaUWP
{
    /// <summary>
    /// Главная страница приложения Lamia.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация контроллера игры
            GameController.Instance.Initialize();

            // Обновление отображения текущего дня
            UpdateCurrentDayText();

            // Выбор первого пункта меню по умолчанию
            if (MenuListView.Items.Count > 0)
            {
                MenuListView.SelectedIndex = 0;
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Сохранение игры и остановка симуляции при выходе
            GameController.Instance.Shutdown();
        }

        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListView.SelectedItem is ListViewItem selectedItem)
            {
                string tag = selectedItem.Tag.ToString();
                
                // Навигация к соответствующей странице в зависимости от выбранного пункта меню
                switch (tag)
                {
                    case "Overview":
                        ContentFrame.Navigate(typeof(OverviewPage));
                        break;
                    case "Messages":
                        ContentFrame.Navigate(typeof(MessagesPage));
                        break;
                    case "Settlements":
                        ContentFrame.Navigate(typeof(SettlementsPage));
                        break;
                    case "Research":
                        ContentFrame.Navigate(typeof(ResearchPage));
                        break;
                    case "Tasks":
                        ContentFrame.Navigate(typeof(TasksPage));
                        break;
                }
            }
        }
        
        /// <summary>
        /// Обработчик нажатия на кнопку "Следующий день".
        /// </summary>
        private void NextDayButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход к следующему игровому дню
            GameController.Instance.Simulation.Simulate(1.0f);
            
            // Обновление отображения текущего дня
            UpdateCurrentDayText();
            
            // Обновление содержимого текущей страницы путем повторной навигации к текущей странице
            if (ContentFrame.Content is Page currentPage)
            {
                Type currentPageType = currentPage.GetType();
                ContentFrame.Navigate(currentPageType);
            }
        }
        
        /// <summary>
        /// Обновляет отображение текущего игрового дня.
        /// </summary>
        private void UpdateCurrentDayText()
        {
            // Получение текущего дня из симуляции
            int currentDay = GameController.Instance.Query.CurrentDay();
            CurrentDayText.Text = $"День: {currentDay}";
        }
    }
}
