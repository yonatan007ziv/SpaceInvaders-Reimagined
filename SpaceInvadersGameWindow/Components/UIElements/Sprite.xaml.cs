using GameWindow.Components.Miscellaneous;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GameWindow.Components.UIElements
{
    public partial class Sprite : UserControl
    {
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
                myBitmapImage.UriSource = new Uri(@"pack://application:,,,/Resources\Images\MissingSprite.png");
                myBitmapImage.EndInit();

                return myBitmapImage;
            }
        }

        Transform transform;

        // Called within an STA thread
        public Sprite(Transform transform)
        {
            InitializeComponent();

            // link to Transform
            this.transform = transform;
            transform.PositionChanged += SetPosition;
            transform.ScaleChanged += SetScale;

            // Image set up
            System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
            image.Stretch = System.Windows.Media.Stretch.Fill;
            image.StretchDirection = StretchDirection.Both;

            MainWindow.instance!.CenteredCanvas.Children.Add(this);
        }

        // Called within an STA thread
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
            Application.Current.Dispatcher.BeginInvoke(() =>
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
        public void Visible(bool visible)
        {
            // UI Objects need to be changed in an STA thread
            Application.Current.Dispatcher.Invoke(() => Visibility = visible ? Visibility.Visible : Visibility.Hidden);
        }
        public void ChangeImage(BitmapImage image)
        {
            // UI Objects need to be changed in an STA thread
            Application.Current.Dispatcher.Invoke(() => this.image.Source = image);
        }
        public void Dispose()
        {
            // UI Objects need to be changed in an STA thread
            Application.Current.Dispatcher.Invoke(() => MainWindow.instance!.CenteredCanvas.Children.Remove(this));
        }
    }
}