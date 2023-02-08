using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace SpaceInvadersGameWindow.Components.UIElements
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

        Transform transform;
        public CustomButton(Transform transform, Action onClick, string imagePath)
        {
            InitializeComponent();

            this.transform = transform;
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();
            buttonImage.Source = Sprite.BitmapFromPath(imagePath);

            button.Click += (s, e) => onClick();

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

            button.Click += (s, e) => onClick();

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
