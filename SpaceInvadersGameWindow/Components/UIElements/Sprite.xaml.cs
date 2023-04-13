using GameWindow.Components.Miscellaneous;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GameWindow.Components.UIElements
{
    public partial class Sprite : Image
    {
        Transform transform;

        // Called within an STA thread
        public Sprite(Transform transform)
        {
            InitializeComponent();

            // link to Transform
            this.transform = transform;
            transform.PositionChanged += SetPosition;
            transform.ScaleChanged += SetScale;

            System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.NearestNeighbor);

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

            System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.NearestNeighbor);

            // set source image
            Source = BitmapFromPath(imagePath);

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

        public void Visible(bool visible)
        {
            // UI Objects need to be changed in an STA thread
            Application.Current.Dispatcher.Invoke(() => Visibility = visible ? Visibility.Visible : Visibility.Hidden);
        }

        public void ChangeImage(string imagePath)
        {
            var bitmap = BitmapFromPath(imagePath);
            bitmap.Freeze();

            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => Source = bitmap);
        }

        public void ChangeImage(BitmapImage bitmap)
        {
            bitmap.Freeze();

            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke((Delegate)(() => Source = bitmap));
        }

        public void Dispose()
        {
            // UI Objects need to be changed in an STA thread
            Application.Current.Dispatcher.Invoke(() => MainWindow.instance!.CenteredCanvas.Children.Remove(this));
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
                myBitmapImage.UriSource = new Uri(@"pack://application:,,,/Resources\Images\MissingSprite.png");
                myBitmapImage.EndInit();
                return myBitmapImage;
            }
        }
    }
}
