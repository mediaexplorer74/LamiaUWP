using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LamiaUWP.Core;

namespace LamiaUWP.Views
{
    /// <summary>
    /// Страница для управления исследованиями.
    /// </summary>
    public sealed partial class ResearchPage : Page
    {
        private GameController _gameController;
        private ObservableCollection<ResearchCategoryItem> _categories;
        private ObservableCollection<ResearchItem> _researches;
        private string _selectedCategoryId;

        public ResearchPage()
        {
            this.InitializeComponent();
            _gameController = GameController.Instance;
            
            // Инициализация коллекций для привязки данных
            _categories = new ObservableCollection<ResearchCategoryItem>();
            _researches = new ObservableCollection<ResearchItem>();
            
            CategoriesListView.ItemsSource = _categories;
            ResearchListView.ItemsSource = _researches;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadResearchCategories();
        }

        /// <summary>
        /// Загружает категории исследований из симуляции.
        /// </summary>
        private void LoadResearchCategories()
        {
            _categories.Clear();
            var categoryIds = _gameController.Query.ResearchCategories();
            
            foreach (var id in categoryIds)
            {
                string name = _gameController.Query.ResearchCategoryName(id);
                
                _categories.Add(new ResearchCategoryItem
                {
                    Id = id,
                    Name = name
                });
            }
            
            // Выбираем первую категорию по умолчанию
            if (_categories.Count > 0)
            {
                CategoriesListView.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Загружает исследования для выбранной категории.
        /// </summary>
        private void LoadResearchesForCategory(string categoryId)
        {
            _researches.Clear();
            _selectedCategoryId = categoryId;
            
            if (string.IsNullOrEmpty(categoryId))
            {
                CategoryNameTextBlock.Text = "Выберите категорию";
                return;
            }
            
            // Получение имени категории
            string categoryName = _gameController.Query.ResearchCategoryName(categoryId);
            CategoryNameTextBlock.Text = categoryName;
            
            // Получение исследований в категории
            var researchIds = _gameController.Query.ResearchesInCategory(categoryId);
            var unlockedResearches = _gameController.Query.UnlockedResearches();
            
            foreach (var id in researchIds)
            {
                string name = _gameController.Query.ResearchName(id);
                string description = _gameController.Query.ResearchDescription(id);
                bool isUnlocked = unlockedResearches.Contains(id);
                
                // Получение требований для исследования
                var requirements = _gameController.Query.ResearchRequirements(id);
                string requirementsText = "";
                
                //if (requirements.Count > 0)
                {
                    var requirementTexts = new List<string>();
                    foreach (var reqId in requirements)
                    {
                        string reqName = _gameController.Query.ResearchName(reqId);
                        bool isReqUnlocked = unlockedResearches.Contains(reqId);
                        requirementTexts.Add($"{reqName} {(isReqUnlocked ? "(✓)" : "(✗)")}" );
                    }
                    requirementsText = string.Join(", ", requirementTexts);
                }
                
                // Проверка доступности исследования
                bool isAvailable = true;
                if (!isUnlocked)// && requirements.Count > 0)
                {
                    isAvailable = requirements.All(reqId => unlockedResearches.Contains(reqId));
                }
                
                // Определение статуса исследования
                string status = isUnlocked ? "Изучено" : (isAvailable ? "Доступно" : "Недоступно");
                SolidColorBrush statusColor = isUnlocked ? 
                    new SolidColorBrush(Colors.Green) : 
                    (isAvailable ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Red));
                
                _researches.Add(new ResearchItem
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    Requirements = requirementsText,
                    RequirementsVisibility = string.IsNullOrEmpty(requirementsText) ? Visibility.Collapsed : Visibility.Visible,
                    Status = status,
                    StatusColor = statusColor,
                    IsUnlocked = isUnlocked,
                    IsAvailable = isAvailable
                });
            }
        }

        /// <summary>
        /// Обработчик события выбора категории исследований.
        /// </summary>
        private void CategoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesListView.SelectedItem is ResearchCategoryItem category)
            {
                LoadResearchesForCategory(category.Id);
            }
            else
            {
                LoadResearchesForCategory(null);
            }
        }

        /// <summary>
        /// Обработчик события нажатия на исследование в списке.
        /// </summary>
        private void ResearchListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ResearchItem research)
            {
                if (!research.IsUnlocked && research.IsAvailable)
                {
                    // Разблокировка исследования
                    _gameController.Action.UnlockResearch(research.Id);
                    
                    // Обновление списка исследований
                    LoadResearchesForCategory(_selectedCategoryId);
                }
                else if (!research.IsAvailable)
                {
                    // Показать сообщение о недоступности исследования
                    ShowResearchUnavailableMessage(research);
                }
            }
        }

        /// <summary>
        /// Отображает сообщение о недоступности исследования.
        /// </summary>
        private async void ShowResearchUnavailableMessage(ResearchItem research)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Исследование недоступно",
                Content = $"Для разблокировки исследования \"{research.Name}\" необходимо сначала изучить: {research.Requirements}",
                CloseButtonText = "Понятно"
            };

            await dialog.ShowAsync();
        }
    }

    /// <summary>
    /// Класс для представления категории исследований в пользовательском интерфейсе.
    /// </summary>
    public class ResearchCategoryItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Класс для представления исследования в пользовательском интерфейсе.
    /// </summary>
    public class ResearchItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public Visibility RequirementsVisibility { get; set; }
        public string Status { get; set; }
        public SolidColorBrush StatusColor { get; set; }
        public bool IsUnlocked { get; set; }
        public bool IsAvailable { get; set; }
    }
}