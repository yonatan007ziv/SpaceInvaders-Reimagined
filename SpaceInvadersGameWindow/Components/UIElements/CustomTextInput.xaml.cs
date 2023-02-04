using SpaceInvaders.Components.Miscellaneous;
using System.Numerics;
using System.Windows.Controls;

namespace SpaceInvadersGameWindow.Components.UIElements
{
    /// <summary>
    /// Interaction logic for CustomTextBox.xaml
    /// </summary>
    public partial class CustomTextInput
    {
        Transform transform;
        public CustomTextInput(Vector2 scale, Vector2 pos)
        {
            InitializeComponent();

            transform = new Transform(scale, pos);
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
            MainWindow.instance!.CenteredCanvas.Children.Remove(this);
        }
    }
}