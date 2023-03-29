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
        public string Text { get { return box.Text; } set { box.Text = value; } }
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
        public void Dispose()
        {
            // UI Objects need to be changed in an STA thread
            Application.Current.Dispatcher.Invoke(() => MainWindow.instance!.CenteredCanvas.Children.Remove(this));
        }
    }
}