using SpaceInvaders.Components.Miscellaneous;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace SpaceInvadersGameWindow.Components.UIElements
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

        Transform transform;
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
            SetValue(Canvas.LeftProperty, (double)transform.CenteredPosition.X);
            SetValue(Canvas.TopProperty, (double)transform.CenteredPosition.Y);
        }
        public void SetScale()
        {
            Width = transform.ActualScale.X;
            Height = transform.ActualScale.Y;
        }
        public void Dispose()
        {
            transform.Dispose();
            Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.instance!.CenteredCanvas.Children.Remove(this);
            });
        }
    }
}