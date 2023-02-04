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
    public partial class CustomButton : UserControl
    {
        public string Text
        {
            get { return buttonText.Text; }
            set { buttonText.Text = value; }
        }

        Transform transform;
        public CustomButton(Vector2 scale, Vector2 pos, Action onClick, string imagePath, bool disposeOnClick)
        {
            InitializeComponent();

            transform = new Transform(scale, pos);
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();
            buttonImage.Source = Sprite.BitmapFromPath(imagePath);

            button.Click += (s, e) => onClick();
            if (disposeOnClick)
                button.Click += (s, e) => Dispose();

            MainWindow.instance!.CenteredCanvas.Children.Add(this);
        }
        public CustomButton(Vector2 scale, Vector2 pos, Action onClick, string imagePath, bool disposeOnClick, string text)
        {
            InitializeComponent();

            Text = text;

            transform = new Transform(scale, pos);
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();
            buttonImage.Source = Sprite.BitmapFromPath(imagePath);

            button.Click += (s, e) => onClick();
            if (disposeOnClick)
                button.Click += (s, e) => Dispose();

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
