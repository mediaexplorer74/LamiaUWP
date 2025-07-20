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
    /// Страница для управления поселениями.
    /// </summary>
    public sealed partial class SettlementsPage : Page
    {
        private GameController _gameController;
        private ObservableCollection<SettlementItem> _settlements;
        private ObservableCollection<ResourceItem> _resources;
        private ObservableCollection<BuildingItem> _buildings;
        private string _selectedSettlementId;

        public SettlementsPage()
        {
            this.InitializeComponent();
            _gameController = GameController.Instance;
            
            // Инициализация коллекций для привязки данных
            _settlements = new ObservableCollection<SettlementItem>();
            _resources = new ObservableCollection<ResourceItem>();
            _buildings = new ObservableCollection<BuildingItem>();
            
            SettlementsListView.ItemsSource = _settlements;
            ResourcesListView.ItemsSource = _resources;
            BuildingsListView.ItemsSource = _buildings;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadSettlements();
        }

        /// <summary>
        /// Загружает список поселений из симуляции.
        /// </summary>
        private void LoadSettlements()
        {
            _settlements.Clear();
            var settlementIds = _gameController.Query.Settlements();
            
            foreach (var id in settlementIds)
            {
                string name = _gameController.Query.SettlementName(id);
                int population = _gameController.Query.SettlementPopulation(id);
                
                _settlements.Add(new SettlementItem
                {
                    Id = id,
                    Name = name,
                    Population = $"Население: {population}"
                });
            }
            
            // Выбираем текущее поселение
            if (_settlements.Count > 0)
            {
                var currentSettlement = _settlements.FirstOrDefault(s => s.Id == _gameController.CurrentSettlementUuid);
                if (currentSettlement != null)
                {
                    SettlementsListView.SelectedItem = currentSettlement;
                }
                else
                {
                    SettlementsListView.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Загружает информацию о выбранном поселении.
        /// </summary>
        private void LoadSettlementDetails(string settlementId)
        {
            if (string.IsNullOrEmpty(settlementId))
            {
                SettlementDetailsGrid.Visibility = Visibility.Collapsed;
                return;
            }
            
            _selectedSettlementId = settlementId;
            _gameController.CurrentSettlementUuid = settlementId;
            
            // Получение информации о поселении
            string name = _gameController.Query.SettlementName(settlementId);
            int population = _gameController.Query.SettlementPopulation(settlementId);
            
            // Обновление текстовых блоков
            SettlementNameTextBlock.Text = name;
            PopulationTextBlock.Text = population.ToString();
            
            // Загрузка ресурсов поселения
            LoadSettlementResources(settlementId);
            
            // Загрузка зданий поселения
            LoadSettlementBuildings(settlementId);
            
            // Отображение панели с деталями поселения
            SettlementDetailsGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Загружает информацию о ресурсах поселения.
        /// </summary>
        private void LoadSettlementResources(string settlementId)
        {
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
        /// Загружает информацию о зданиях поселения.
        /// </summary>
        private void LoadSettlementBuildings(string settlementId)
        {
            _buildings.Clear();
            var availableBuildings = _gameController.Query.AvailableBuildings();
            var unlockedBuildings = _gameController.Query.UnlockedBuildings();
            
            foreach (var buildingId in availableBuildings)
            {
                string name = _gameController.Query.BuildingName(buildingId);
                string description = default;//_gameController.Query.BuildingDescription(buildingId);
                bool isUnlocked = unlockedBuildings.Contains(buildingId);
                
                _buildings.Add(new BuildingItem
                {
                    Id = buildingId,
                    Name = name + (isUnlocked ? " (Разблокировано)" : ""),
                    Description = description,
                    IsUnlocked = isUnlocked
                });
            }
        }

        /// <summary>
        /// Обработчик события выбора поселения в списке.
        /// </summary>
        private void SettlementsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettlementsListView.SelectedItem is SettlementItem settlement)
            {
                LoadSettlementDetails(settlement.Id);
            }
            else
            {
                SettlementDetailsGrid.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку добавления поселения.
        /// </summary>
        private async void AddSettlementButton_Click(object sender, RoutedEventArgs e)
        {
            // Диалог для выбора локации для нового поселения
            ContentDialog dialog = new ContentDialog
            {
                Title = "Добавить поселение",
                PrimaryButtonText = "Добавить",
                CloseButtonText = "Отмена",
                DefaultButton = ContentDialogButton.Primary
            };

            // Создание элементов диалога
            StackPanel panel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };
            TextBlock instructionText = new TextBlock
            {
                Text = "Выберите локацию для нового поселения:",
                Margin = new Thickness(0, 0, 0, 10)
            };
            
            ComboBox locationsComboBox = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 10)
            };
            
            TextBlock nameLabel = new TextBlock
            {
                Text = "Название поселения:",
                Margin = new Thickness(0, 10, 0, 5)
            };
            
            TextBox nameTextBox = new TextBox
            {
                PlaceholderText = "Введите название поселения",
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Получение доступных локаций
            var locations = _gameController.Query.Locations();
            foreach (var locationId in locations)
            {
                string locationName = _gameController.Query.LocationName(locationId);
                locationsComboBox.Items.Add(new LocationItem { Id = locationId, Name = locationName });
            }

            if (locationsComboBox.Items.Count > 0)
            {
                locationsComboBox.SelectedIndex = 0;
            }

            // Добавление элементов в панель
            panel.Children.Add(instructionText);
            panel.Children.Add(locationsComboBox);
            panel.Children.Add(nameLabel);
            panel.Children.Add(nameTextBox);
            dialog.Content = panel;

            // Отображение диалога и обработка результата
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (locationsComboBox.SelectedItem is LocationItem selectedLocation)
                {
                    string settlementName = nameTextBox.Text;
                    if (string.IsNullOrWhiteSpace(settlementName))
                    {
                        settlementName = "Новое поселение";
                    }

                    // Добавление нового поселения
                    string newSettlementId = _gameController.Action.AddSettlementAtLocation(selectedLocation.Id, settlementName);
                    
                    // Обновление списка поселений
                    LoadSettlements();
                    
                    // Выбор нового поселения
                    var newSettlement = _settlements.FirstOrDefault(s => s.Id == newSettlementId);
                    if (newSettlement != null)
                    {
                        SettlementsListView.SelectedItem = newSettlement;
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку переименования поселения.
        /// </summary>
        private async void RenameSettlementButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedSettlementId))
                return;

            // Диалог для переименования поселения
            ContentDialog dialog = new ContentDialog
            {
                Title = "Переименовать поселение",
                PrimaryButtonText = "Сохранить",
                CloseButtonText = "Отмена",
                DefaultButton = ContentDialogButton.Primary
            };

            // Создание элементов диалога
            StackPanel panel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };
            TextBlock instructionText = new TextBlock
            {
                Text = "Введите новое название поселения:",
                Margin = new Thickness(0, 0, 0, 10)
            };
            
            TextBox nameTextBox = new TextBox
            {
                Text = SettlementNameTextBlock.Text,
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Добавление элементов в панель
            panel.Children.Add(instructionText);
            panel.Children.Add(nameTextBox);
            dialog.Content = panel;

            // Отображение диалога и обработка результата
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string newName = nameTextBox.Text;
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    // Переименование поселения
                    _gameController.Action.RenameSettlement(_selectedSettlementId, newName);
                    
                    // Обновление интерфейса
                    SettlementNameTextBlock.Text = newName;
                    LoadSettlements();
                }
            }
        }

        /// <summary>
        /// Обработчик события нажатия на здание в списке.
        /// </summary>
        private void BuildingsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is BuildingItem building && !building.IsUnlocked)
            {
                // Разблокировка выбранного здания
                _gameController.Action.UnlockBuilding(building.Id);
                
                // Обновление списка зданий
                LoadSettlementBuildings(_selectedSettlementId);
            }
        }
    }

    /// <summary>
    /// Класс для представления поселения в пользовательском интерфейсе.
    /// </summary>
    public class SettlementItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Population { get; set; }
    }

    /// <summary>
    /// Класс для представления ресурса в пользовательском интерфейсе.
    /// </summary>
    /*public class ResourceItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
    }*/

    /// <summary>
    /// Класс для представления здания в пользовательском интерфейсе.
    /// </summary>
    public class BuildingItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsUnlocked { get; set; }
    }

    /// <summary>
    /// Класс для представления локации в пользовательском интерфейсе.
    /// </summary>
    public class LocationItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}