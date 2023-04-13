using GameWindow.Components.Miscellaneous;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace GameWindow.Components.UIElements
{
    /// <summary>
    /// Interaction logic for CustomLabel.xaml
    /// </summary>
    public partial class CustomLabel : UserControl
    {
        public string Text
        {
            get { return (string)label.Content; }
            set { label.Dispatcher.Invoke(() => label.Content = value); }
        }

        public Transform transform;
        public CustomLabel(Transform transform, string text, System.Windows.Media.Color TextColor)
        {
            InitializeComponent();
            Text = text;
            label.SetValue(ForegroundProperty, new System.Windows.Media.SolidColorBrush(TextColor));

            this.transform = transform;
            transform.PositionChanged += SetPosition;
            transform.ScaleChanged += SetScale;

            MainWindow.instance!.CenteredCanvas.Children.Add(this);
        }
        public void SetPosition()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                SetValue(Canvas.LeftProperty, (double)transform.CenteredPosition.X);
                SetValue(Canvas.TopProperty, (double)transform.CenteredPosition.Y);
            });
        }
        public void SetScale()
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