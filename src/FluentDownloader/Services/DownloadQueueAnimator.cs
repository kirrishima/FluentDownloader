using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FluentDownloader.Services
{
    public class DownloadQueueAnimator
    {
        private readonly UIElement _notificationPanel;
        private readonly UIElement _queuePanel;
        private readonly TranslateTransform _queueTransform;
        private readonly Storyboard _fadeOutNotifications;
        private readonly Storyboard _fadeInNotifications;
        private readonly Storyboard _slideInQueue;
        private readonly Storyboard _slideOutQueue;

        public DownloadQueueAnimator(
            UIElement notificationPanel,
            Storyboard fadeOutNotifications,
            Storyboard fadeInNotifications,
            UIElement queuePanel,
            TranslateTransform queueTransform,
            Storyboard slideInQueue,
            Storyboard slideOutQueue)
        {
            _notificationPanel = notificationPanel;
            _fadeOutNotifications = fadeOutNotifications;
            _fadeInNotifications = fadeInNotifications;
            _queuePanel = queuePanel;
            _queueTransform = queueTransform;
            _slideInQueue = slideInQueue;
            _slideOutQueue = slideOutQueue;

            // Изначальное состояние
            _notificationPanel.Opacity = 1;
            _queuePanel.Visibility = Visibility.Collapsed;
        }

        public async Task ShowQueueAsync()
        {
            // Запустить fade-out без ожидания
            _fadeOutNotifications.Begin();
            _notificationPanel.Visibility = Visibility.Collapsed;

            // Отобразить очередь
            _queuePanel.Visibility = Visibility.Visible;
            await EnsureWidthAsync();

            double width = ((FrameworkElement)_queuePanel).ActualWidth;
            if (double.IsNaN(width) || width <= 0)
                width = 0;

            // Позиционируем за экраном справа
            _queueTransform.X = width;

            // Обновляем storyboard only if width > 0
            var da = _slideInQueue.Children.OfType<DoubleAnimation>().First();
            if (width > 0)
                da.From = width;
            da.To = 0;

            var tcs = new TaskCompletionSource<object>();
            void OnCompleted(object s, object e)
            {
                _slideInQueue.Completed -= OnCompleted;
                tcs.SetResult(null!);
            }
            _slideInQueue.Completed += OnCompleted;
            _slideInQueue.Begin();
            return;
        }

        public async Task HideQueueAsync()
        {
            await EnsureWidthAsync();
            double width = ((FrameworkElement)_queuePanel).ActualWidth;
            if (double.IsNaN(width) || width <= 0)
                width = 0;

            var da = _slideOutQueue.Children.OfType<DoubleAnimation>().First();
            da.From = 0;
            if (width > 0)
                da.To = width;

            var tcs = new TaskCompletionSource<object>();
            void OnSlideOutCompleted(object s, object e)
            {
                _slideOutQueue.Completed -= OnSlideOutCompleted;
                _queuePanel.Visibility = Visibility.Collapsed;

                // Показываем уведомления
                _notificationPanel.Visibility = Visibility.Visible;
                _fadeInNotifications.Begin();
                tcs.SetResult(null!);
            }
            _slideOutQueue.Completed += OnSlideOutCompleted;
            _slideOutQueue.Begin();
        }

        private Task EnsureWidthAsync()
        {
            var fe = _queuePanel as FrameworkElement;
            if (fe != null && fe.ActualWidth > 0)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<object>();
            SizeChangedEventHandler handler = null;
            handler = (s, e) =>
            {
                if (fe.ActualWidth > 0)
                {
                    fe.SizeChanged -= handler;
                    tcs.SetResult(null!);
                }
            };
            fe.SizeChanged += handler;
            return tcs.Task;
        }
    }
}
