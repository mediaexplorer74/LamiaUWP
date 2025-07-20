using System;
using Windows.UI.Core;

namespace LamiaUWP.Core
{
    /// <summary>
    /// Класс для создания таймера, который выполняет действия на основном потоке UI.
    /// </summary>
    public class DispatcherTimer
    {
        private Windows.UI.Xaml.DispatcherTimer _timer;
        
        /// <summary>
        /// Событие, которое вызывается при срабатывании таймера.
        /// </summary>
        public event EventHandler<object> Tick;
        
        /// <summary>
        /// Интервал между срабатываниями таймера.
        /// </summary>
        public TimeSpan Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }
        
        /// <summary>
        /// Создает новый экземпляр класса DispatcherTimer.
        /// </summary>
        public DispatcherTimer()
        {
            _timer = new Windows.UI.Xaml.DispatcherTimer();
            _timer.Tick += OnTimerTick;
        }
        
        /// <summary>
        /// Обработчик события срабатывания таймера.
        /// </summary>
        private void OnTimerTick(object sender, object e)
        {
            Tick?.Invoke(this, e);
        }
        
        /// <summary>
        /// Запускает таймер.
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }
        
        /// <summary>
        /// Останавливает таймер.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }
    }
}