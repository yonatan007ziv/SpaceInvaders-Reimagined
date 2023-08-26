using GameWindow.Components.Miscellaneous;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GameWindow.Components.UIElements
{
    /// <summary>
    /// A custom <see cref="Label"/> implementation
    /// </summary>
    public partial class CustomLabel : Viewbox
    {
        public Transform transform;

        public string Text
        {
            get { return (string)label.Content; }
            set { Dispatcher.Invoke(() => label.Content = value); }
        }

        /// <summary>
        /// Builds a label UI element
        /// </summary>
        /// <param name="transform"> The <see cref="Transform"/> to link </param>
        /// <param name="text"> Text to display </param>
        /// <param name="TextColor"> Color of the text </param>
        public CustomLabel(Guid key, Transform transform, string text, System.Windows.Media.Color TextColor) // Called within an STA thread
        {
            InitializeComponent();
            Text = text;
            label.SetValue(Control.ForegroundProperty, new System.Windows.Media.SolidColorBrush(TextColor));

            this.transform = transform;
            transform.PositionChanged += SetPosition;
            transform.ScaleChanged += SetScale;

            MainWindow.instance!.CenteredCanvas.Children.Add(this);

            SetValue(Panel.ZIndexProperty, 2);
        }

        /// <summary>
        /// Sets the position according to the linked transform
        /// </summary>
        private void SetPosition()
        {
            Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                SetValue(Canvas.LeftProperty, (double)transform.CenteredPosition.X);
                SetValue(Canvas.TopProperty, (double)transform.CenteredPosition.Y);
            });
        }

        /// <summary>
        /// Sets the scale according to the linked transform
        /// </summary>
        private void SetScale()
        {
            Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                Width = transform.ActualScale.X;
                Height = transform.ActualScale.Y;
            });
        }

        /// <summary>
        /// Sets the visibility
        /// </summary>
        /// <param name="visible"> Whether it should be visible or not </param>
        public void Visible(bool visible)
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => Visibility = visible ? Visibility.Visible : Visibility.Hidden);
        }

        /// <summary>
        /// Disposes the <see cref="CustomLabel"/>
        /// </summary>
        public void Dispose()
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => MainWindow.instance!.CenteredCanvas.Children.Remove(this));
        }
    }
}