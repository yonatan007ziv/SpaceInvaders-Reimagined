using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Controls;

namespace SpaceInvadersGameWindow.Components.UIElements
{
    /// <summary>
    /// Interaction logic for NavigableButton.xaml
    /// </summary>
    public partial class NavigableButton : UserControl
    {
        Transform transform;
        public NavigableButton(Vector2 scale, Vector2 pos, Action onClick, bool disposeOnClick)
        {
            InitializeComponent();
            transform = new Transform(scale, pos);
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();
            buttonImage.Source = Sprite.BitmapImageMaker(@"Resources\RawFiles\Images\Player\PlayerDeath1.png");

            button.Click += (s, e) => onClick();
            if (disposeOnClick)
                button.Click += (s, e) => Dispose();

            //MainWindow.instance!.GameplayCanvas.Children.Add(this);
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
        private void Dispose()
        {
            transform.Dispose();
            MainWindow.instance!.GameplayCanvas.Children.Remove(this);
        }
    }
}
