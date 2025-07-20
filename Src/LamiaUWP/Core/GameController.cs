using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using LamiaSimulation;

namespace LamiaUWP.Core
{
    /// <summary>
    /// Контроллер игры, управляющий симуляцией и ее состоянием.
    /// </summary>
    public class GameController
    {
        private static GameController _instance;
        public static GameController Instance => _instance ??= new GameController();

        public Simulation Simulation { get; private set; }
        public string CurrentSettlementUuid { get; set; }
        public bool IsReady { get; private set; }

        // Объекты для взаимодействия с симуляцией
        public Action Action { get; private set; }
        public Query Query { get; private set; }

        // Таймер для обновления симуляции
        private DispatcherTimer _simulationTimer;
        private const float SimulationUpdateInterval = 1.0f / 30.0f; // 30 FPS

        private GameController()
        {
            Simulation = Simulation.Instance;
            Action = new Action();
            Query = new Query();
        }

        /// <summary>
        /// Инициализирует игру и запускает симуляцию.
        /// </summary>
        public void Initialize()
        {
            Simulation.LoadGame();
            Simulation.Start();
            CurrentSettlementUuid = Query.Settlements()[0];
            IsReady = true;

            // Настройка таймера для обновления симуляции
            _simulationTimer = new DispatcherTimer();
            _simulationTimer.Tick += SimulationTimerTick;
            _simulationTimer.Interval = TimeSpan.FromSeconds(SimulationUpdateInterval);
            _simulationTimer.Start();
        }

        /// <summary>
        /// Обработчик тика таймера для обновления симуляции.
        /// </summary>
        private void SimulationTimerTick(object sender, object e)
        {
            Simulation.Simulate(SimulationUpdateInterval);
        }

        /// <summary>
        /// Сохраняет текущее состояние игры.
        /// </summary>
        public void SaveGame()
        {
            Simulation.SaveGame();
        }

        /// <summary>
        /// Перезапускает игру с начала.
        /// </summary>
        public void RestartGame()
        {
            Simulation.Reset();
            Simulation.Start();
            CurrentSettlementUuid = Query.Settlements()[0];
        }

        /// <summary>
        /// Останавливает симуляцию и освобождает ресурсы.
        /// </summary>
        public void Shutdown()
        {
            if (_simulationTimer != null)
            {
                _simulationTimer.Stop();
                _simulationTimer = null;
            }

            SaveGame();
        }

        internal void LoadGame()
        {
            throw new NotImplementedException();
        }
    }
}