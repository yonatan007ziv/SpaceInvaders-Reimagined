using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using SpaceInvadersGameWindow;
using SpaceInvaders.Components.Miscellaneous;

namespace SpaceInvaders.Components.Renderer
{
    internal class SpriteRenderer : Image
    {
        Transform transform;
        public SpriteRenderer(Transform transform) : base()
        {
            // link to Transform
            this.transform = transform;
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();

            // image settings set up
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(this);
            Stretch = System.Windows.Media.Stretch.Fill;
            StretchDirection = StretchDirection.Both;

            SetPosition();
            SetScale();
        }

        public SpriteRenderer(Transform transform, string imagePath) : base()
        {
            // link to Transform
            this.transform = transform;
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();

            // set source image
            Source = BitmapImageMaker(imagePath);

            // image settings set up
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(this);
            Stretch = System.Windows.Media.Stretch.Fill;
            StretchDirection = StretchDirection.Both;

            SetPosition();
            SetScale();
        }

        public void SetPosition()
        {
            SetValue(Canvas.LeftProperty, (double)transform.CenteredPosition.X);
            SetValue(Canvas.TopProperty, (double)transform.CenteredPosition.Y);
        }
        public void SetScale()
        {
            Width = transform.scale.X;
            Height = transform.scale.Y;
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