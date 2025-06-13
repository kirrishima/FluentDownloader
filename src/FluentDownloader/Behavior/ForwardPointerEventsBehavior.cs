using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace FluentDownloader.Behavior
{
    public class ForwardPointerEventsBehavior : Behavior<UIElement>
    {
        public RadioButton TargetRadioButton
        {
            get => (RadioButton)GetValue(TargetRadioButtonProperty);
            set => SetValue(TargetRadioButtonProperty, value);
        }

        public static readonly DependencyProperty TargetRadioButtonProperty =
            DependencyProperty.Register(nameof(TargetRadioButton), typeof(RadioButton),
                typeof(ForwardPointerEventsBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerEntered += OnPointerEntered;
            AssociatedObject.PointerExited += OnPointerExited;
            AssociatedObject.PointerPressed += OnPointerPressed;
            AssociatedObject.PointerReleased += OnPointerReleased;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PointerEntered -= OnPointerEntered;
            AssociatedObject.PointerExited -= OnPointerExited;
            AssociatedObject.PointerPressed -= OnPointerPressed;
            AssociatedObject.PointerReleased -= OnPointerReleased;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (TargetRadioButton != null)
            {
                VisualStateManager.GoToState(TargetRadioButton, "PointerOver", true);
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (TargetRadioButton != null)
            {
                VisualStateManager.GoToState(TargetRadioButton, "Normal", true);
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (TargetRadioButton != null)
            {
                VisualStateManager.GoToState(TargetRadioButton, "Pressed", true);
            }
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (TargetRadioButton != null)
            {
                TargetRadioButton.IsChecked = true;

                VisualStateManager.GoToState(TargetRadioButton, "PointerOver", true);
            }
        }
    }
}
