using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Dispatching;

namespace MyApp.Helpers
{
    /// <summary>
    /// Управляет плавным появлением/скрытием FrameworkElement
    /// на основе изменения булевого свойства в INotifyPropertyChanged.
    /// При создании подписывается на событие PropertyChanged источника.
    /// </summary>
    public class VisibilityAnimator : IDisposable
    {
        private readonly FrameworkElement _target;
        private readonly INotifyPropertyChanged _source;
        private readonly string _propertyName;
        private readonly Func<bool> _valueGetter;
        private readonly Storyboard _showStoryboard;
        private readonly Storyboard _hideStoryboard;
        private readonly DispatcherQueue _dispatcher;
        private readonly Action? _prepareShowAction;

        private bool _isDisposed;
        private bool _isShowing;
        private bool _isHiding;

        public VisibilityAnimator(
            FrameworkElement target,
            INotifyPropertyChanged source,
            string propertyName,
            Func<bool> valueGetter,
            Storyboard showStoryboard,
            Storyboard hideStoryboard,
            Action? prepareShowAction = null,
            DispatcherQueue? dispatcher = null)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            _valueGetter = valueGetter ?? throw new ArgumentNullException(nameof(valueGetter));
            _showStoryboard = showStoryboard ?? throw new ArgumentNullException(nameof(showStoryboard));
            _hideStoryboard = hideStoryboard ?? throw new ArgumentNullException(nameof(hideStoryboard));
            _prepareShowAction = prepareShowAction;
            _dispatcher = dispatcher ?? DispatcherQueue.GetForCurrentThread();

            // Подписываемся
            _source.PropertyChanged += Source_PropertyChanged;

            // Сохраняем начальное состояние (вызов на UI-потоке)
            _dispatcher.TryEnqueue(() => UpdateVisual(_valueGetter(), initial: true));
        }

        private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName || string.IsNullOrEmpty(e.PropertyName))
            {
                bool newValue;
                try
                {
                    newValue = _valueGetter();
                }
                catch
                {
                    // если получение значения упало — игнорируем
                    return;
                }

                // Выполняем на UI-потоке
                _dispatcher.TryEnqueue(() => UpdateVisual(newValue, initial: false));
            }
        }

        private void UpdateVisual(bool shouldBeVisible, bool initial)
        {
            if (_isDisposed) return;

            if (shouldBeVisible)
            {
                // Если уже показываем — ничего не делаем
                if (_target.Visibility == Visibility.Visible && !_isHiding) return;

                // Остановим hide, если он идёт
                if (_isHiding)
                {
                    _hideStoryboard.Completed -= Hide_Completed;
                    _hideStoryboard.Stop();
                    _isHiding = false;
                }

                // Подготовка перед показом (например, задать Opacity=0 и Translate.Y)
                _prepareShowAction?.Invoke();

                // Сделаем видимым и стартуем show
                _target.Visibility = Visibility.Visible;

                // Гарантируем, что не висит предыдущая Completed-подписка
                _showStoryboard.Completed -= Show_Completed;
                _showStoryboard.Completed += Show_Completed;

                _isShowing = true;
                _showStoryboard.Begin();
            }
            else
            {
                // Если уже скрыт и нет анимации — ничего не делаем
                if (_target.Visibility == Visibility.Collapsed && !_isShowing) return;

                // Остановим show, если он идёт
                if (_isShowing)
                {
                    _showStoryboard.Completed -= Show_Completed;
                    _showStoryboard.Stop();
                    _isShowing = false;
                }

                // Подготовка Completed-обработчика, чтобы поставить Collapsed после окончания
                _hideStoryboard.Completed -= Hide_Completed;
                _hideStoryboard.Completed += Hide_Completed;

                _isHiding = true;
                _hideStoryboard.Begin();
            }

            // Для начальной установки, если initial == true и мы показываем, можно не проигрывать анимацию,
            // но в нашем подходе выше мы уже запустили showStoryboard — если хочется избежать анимации при старте,
            // можно дописать логику: если initial==true -> _showStoryboard.SkipToFill() / set properties напрямую.
        }

        private void Show_Completed(object? sender, object e)
        {
            _showStoryboard.Completed -= Show_Completed;
            _isShowing = false;
            // ничего дополнительного — оставляем Visibility=Visible
        }

        private void Hide_Completed(object? sender, object e)
        {
            _hideStoryboard.Completed -= Hide_Completed;
            _isHiding = false;

            // Скрываем элемент по завершении
            _target.Visibility = Visibility.Collapsed;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            try
            {
                _source.PropertyChanged -= Source_PropertyChanged;
            }
            catch { /* ignore */ }

            // Остановим анимации и отпишем обработчики
            try
            {
                _showStoryboard.Completed -= Show_Completed;
                _hideStoryboard.Completed -= Hide_Completed;
                _showStoryboard.Stop();
                _hideStoryboard.Stop();
            }
            catch { /* ignore */ }
        }
    }
}
