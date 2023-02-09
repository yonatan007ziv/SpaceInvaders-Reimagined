using SpaceInvaders.Components.Miscellaneous;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SpaceInvadersGameWindow.Components.UIElements
{
    public partial class Sprite : UserControl
    {
        Transform transform;
        public Sprite(Transform transform)
        {
            InitializeComponent();

            // link to Transform
            this.transform = transform;
            transform.PositionChanged += SetPosition;
            transform.ScaleChanged += SetScale;

            // image settings set up
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            image.Stretch = System.Windows.Media.Stretch.Fill;
            image.StretchDirection = StretchDirection.Both;

            MainWindow.instance!.CenteredCanvas.Children.Add(this);
        }

        public Sprite(Transform transform, string imagePath)
        {
            InitializeComponent();

            // link to Transform
            this.transform = transform;
            transform.PositionChanged += SetPosition;
            transform.ScaleChanged += SetScale;

            // image settings set up
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            image.Stretch = System.Windows.Media.Stretch.Fill;
            image.StretchDirection = StretchDirection.Both;

            // set source image
            image.Source = BitmapFromPath(imagePath);

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

        public static BitmapImage BitmapFromPath(string path)
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
            transform.Dispose();
            MainWindow.instance!.CenteredCanvas.Dispatcher.Invoke
                (
                () => MainWindow.instance!.CenteredCanvas.Children.Remove(this)
                );
        }
    }
}