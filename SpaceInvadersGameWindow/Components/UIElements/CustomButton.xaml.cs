using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GameWindow.Components.UIElements
{
    /// <summary>
    /// A custom <see cref="Button"/> implementation
    /// </summary>
    public partial class CustomButton : Button
    {
        public Transform transform;
        private CustomLabel text;

        public string Text
        {
            get { return text.Text; }
            set { text.Text = value; }
        }

        /// <summary>
        /// Builds a button UI element with an image
        /// </summary>
        /// <param name="transform"> The <see cref="Transform"/> to link </param>
        /// <param name="onClick"> What happens on button click </param>
        /// <param name="image"> Path to the image </param>
        /// <param name="text"> Text on button </param>
        public CustomButton(Transform transform, Action onClick, System.Windows.Media.Color color, string text) // Called within an STA thread
        {
            InitializeComponent();

            this.transform = transform;
            Background = new System.Windows.Media.SolidColorBrush(color);
            this.text = new CustomLabel(transform, text, System.Windows.Media.Colors.White);

            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();

            Click += (s, e) => { SoundManager.PlaySound(Sound.MenuClick); onClick(); };

            MainWindow.instance!.CenteredCanvas.Children.Add(this);

            SetValue(Panel.ZIndexProperty, 1);
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
            Dispatcher.Invoke(() =>
            {
                Visibility = visible ? Visibility.Visible : Visibility.Hidden;
                //image.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
                text.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            });
        }

        /// <summary>
        /// Disposes the <see cref="CustomButton"/>
        /// </summary>
        public void Dispose()
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() =>
            {
                MainWindow.instance!.CenteredCanvas.Children.Remove(text);
                MainWindow.instance!.CenteredCanvas.Children.Remove(this);
            });
        }
    }
}