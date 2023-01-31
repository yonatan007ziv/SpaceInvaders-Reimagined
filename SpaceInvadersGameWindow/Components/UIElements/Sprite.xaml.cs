using SpaceInvaders.Components.Miscellaneous;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SpaceInvadersGameWindow.Components.UIElements
{
    /// <summary>
    /// Interaction logic for Sprite.xaml
    /// </summary>
    public partial class Sprite : UserControl
    {
        Transform transform;
        public Sprite(Transform transform) : base()
        {
            // link to Transform
            this.transform = transform;
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();

            // image settings set up
            image = new Image();
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(image);
            image.Stretch = System.Windows.Media.Stretch.Fill;
            image.StretchDirection = StretchDirection.Both;

            SetPosition();
            SetScale();
            transform.OnSizeChanged();
        }

        public Sprite(Transform transform, string imagePath) : base()
        {
            // link to Transform
            this.transform = transform;
            transform.PositionChanged += () => SetPosition();
            transform.ScaleChanged += () => SetScale();

            // image settings set up
            image = new Image();
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            MainWindow.instance!.GameplayCanvas.Children.Add(image);
            image.Stretch = System.Windows.Media.Stretch.Fill;
            image.StretchDirection = StretchDirection.Both;

            // set source image
            image.Source = BitmapImageMaker(imagePath);

            SetPosition();
            SetScale();
            transform.OnSizeChanged();
        }

        public void SetPosition()
        {
            image.SetValue(Canvas.LeftProperty, (double)transform.CenteredPosition.X);
            image.SetValue(Canvas.TopProperty, (double)transform.CenteredPosition.Y);
        }
        public void SetScale()
        {
            image.Width = transform.ActualScale.X;
            image.Height = transform.ActualScale.Y;
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
            MainWindow.instance!.GameplayCanvas.Children.Remove(image);
        }
    }
}