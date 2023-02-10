using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using GameWindow;
using System.Windows.Media;
using Transform = GameWindow.Components.Miscellaneous.Transform; // Due to ambiguity with System.Windows.Media Transform

namespace GameWindow.Components.Renderer
{
    /*
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
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(this);
            Stretch = Stretch.Fill;
            StretchDirection = StretchDirection.Both;
            transform.OnSizeChanged();
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
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(this);
            Stretch = Stretch.Fill;
            StretchDirection = StretchDirection.Both;

            SetPosition();
            SetScale();
            transform.OnSizeChanged();
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
                myBitmapImage.UriSource = new Uri(@"pack://application:,,,/Resources
    \Images\MissingSprite.png");
                myBitmapImage.EndInit();

                return myBitmapImage;
            }
        }

        public void Dispose()
        {
            MainWindow.instance!.GameplayCanvas.Children.Remove(this);
        }
    }
    */
}