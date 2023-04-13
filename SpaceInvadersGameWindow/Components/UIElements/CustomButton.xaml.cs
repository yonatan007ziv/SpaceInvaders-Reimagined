using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GameWindow.Components.UIElements
{
    /// <summary>
    /// Interaction logic for NavigableButton.xaml
    /// </summary>
    public partial class CustomButton
    {
        public string Text
        {
            get { return buttonText.Text; }
            set { buttonText.Text = value; }
        }

        public Transform transform;
        public CustomButton(Transform transform, Action onClick, string imagePath)
        {
            InitializeComponent();

            this.transform = transform;
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();
            buttonImage.Source = Sprite.BitmapFromPath(imagePath);

            button.Click += (s, e) => { SoundManager.PlaySound(Sound.MenuClick); onClick(); };

            System.Windows.Media.RenderOptions.SetBitmapScalingMode(buttonImage, System.Windows.Media.BitmapScalingMode.NearestNeighbor);

            MainWindow.instance!.CenteredCanvas.Children.Add(this);
        }
        public CustomButton(Transform transform, Action onClick, string imagePath, string text)
        {
            InitializeComponent();

            Text = text;

            this.transform = transform;
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();
            buttonImage.Source = Sprite.BitmapFromPath(imagePath);

            button.Click += (s, e) => { SoundManager.PlaySound(Sound.MenuClick); onClick(); };

            System.Windows.Media.RenderOptions.SetBitmapScalingMode(buttonImage, System.Windows.Media.BitmapScalingMode.NearestNeighbor);

            MainWindow.instance!.CenteredCanvas.Children.Add(this);
        }

        private void SetPosition()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                SetValue(Canvas.LeftProperty, (double)transform.CenteredPosition.X);
                SetValue(Canvas.TopProperty, (double)transform.CenteredPosition.Y);
            });
        }

        private void SetScale()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                Width = transform.ActualScale.X;
                Height = transform.ActualScale.Y;
            });
        }
        public void Visible(bool visible)
        {
            // UI Objects need to be changed in an STA thread
            Application.Current.Dispatcher.Invoke(() => Visibility = visible ? Visibility.Visible : Visibility.Hidden);
        }
        public void Dispose()
        {
            // UI Objects need to be changed in an STA thread
            Application.Current.Dispatcher.Invoke(() => MainWindow.instance!.CenteredCanvas.Children.Remove(this));
        }
    }
}