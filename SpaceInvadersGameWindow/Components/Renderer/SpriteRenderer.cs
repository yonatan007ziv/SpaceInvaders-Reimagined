using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media;
using SpaceInvadersGameWindow;

namespace SpaceInvaders.Components.Renderer
{
    internal class SpriteRenderer : Image
    {
        public SpriteRenderer(Vector2 scale, Vector2 pos) : base()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(this);

            Stretch = Stretch.Fill;
            StretchDirection = StretchDirection.Both;

            SetScale(scale);
            SetPosition(pos);
        }

        public SpriteRenderer(string imagePath, Vector2 scale, Vector2 pos) : base()
        {
            Source = BitmapImageMaker(imagePath);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(this);

            Stretch = Stretch.Fill;
            StretchDirection = StretchDirection.Both;

            SetScale(scale);
            SetPosition(pos);
        }
        public void SetPosition(Vector2 pos)
        {
            SetValue(Canvas.LeftProperty, (double)pos.X);
            SetValue(Canvas.TopProperty, (double)pos.Y);
        }
        public void SetScale(Vector2 scale)
        {
            Width = scale.X;
            Height = scale.Y;
        }

        public static BitmapImage BitmapImageMaker(string path)
        {
            try
            {
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri("pack://application:,,,/" + path);
                myBitmapImage.EndInit();

                return myBitmapImage;
            }
            catch
            {
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(@"pack://application:,,,/Resources\RawFiles\Images\MissingSprite.png");
                myBitmapImage.EndInit();

                return myBitmapImage;
            }
        }
        public void Dispose()
        {
            MainWindow.instance!.GameplayCanvas.Children.Remove(this);
        }
    }
}