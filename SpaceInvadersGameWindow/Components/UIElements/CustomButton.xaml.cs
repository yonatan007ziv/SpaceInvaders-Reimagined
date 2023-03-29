using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Renderer;
using GameWindow.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        Transform transform;
        public CustomButton(Transform transform, Action onClick, string imagePath)
        {
            InitializeComponent();

            this.transform = transform;
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();
            buttonImage.Source = Sprite.BitmapFromPath(imagePath);

            button.Click += (s, e) => { SoundManager.PlaySound("MenuClick"); onClick(); };

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

            button.Click += (s, e) => { SoundManager.PlaySound("MenuClick"); onClick(); };

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