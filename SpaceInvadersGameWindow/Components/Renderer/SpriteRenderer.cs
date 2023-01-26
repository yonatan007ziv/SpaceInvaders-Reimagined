using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows;
using System.Windows.Media;
using SpaceInvadersGameWindow;

namespace SpaceInvaders.Components.Renderer
{
    internal class SpriteRenderer : Image
    {
        Vector2 pos, scale;
        public SpriteRenderer(Vector2 pos, Vector2 scale) : base()
        {
            this.pos = pos; this.scale = scale;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(this);
            SetPosition(pos);
            SetScale(scale);
        }

        public SpriteRenderer(string imagePath, Vector2 pos, Vector2 scale) : base()
        {
            this.pos = pos; this.scale = scale;
            Source = BitmapImageMaker(imagePath);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(this);
            SetPosition(pos);
            SetScale(scale);
        }
        public void SetPosition(Vector2 pos)
        {
            SetValue(Canvas.LeftProperty, (double)pos.X - scale.X / 2);
            SetValue(Canvas.TopProperty, (double)pos.Y - scale.Y / 2);
        }
        public void SetScale(Vector2 scale)
        {
            SetValue(Canvas.WidthProperty, (double)scale.X);
            SetValue(Canvas.HeightProperty, (double)scale.Y);
            this.scale = scale;
        }

        public static BitmapImage BitmapImageMaker(string path)
        {
            BitmapImage myBitmapImage = new BitmapImage();

            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri("pack://application:,,,/" + path);
            myBitmapImage.EndInit();

            return myBitmapImage;
        }
        public void Dispose()
        {
            MainWindow.instance!.GameplayCanvas.Children.Remove(this);
        }
    }
}