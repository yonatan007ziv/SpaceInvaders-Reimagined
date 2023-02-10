using GameWindow.Components.Miscellaneous;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace GameWindow.Components.UIElements
{
    /// <summary>
    /// Interaction logic for CustomTextBox.xaml
    /// </summary>
    public partial class CustomTextInput
    {
        public string Text { get { return box.Text; }}
        Transform transform;
        public CustomTextInput(Transform transform)
        {
            InitializeComponent();

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