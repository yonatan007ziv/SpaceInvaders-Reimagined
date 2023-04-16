using GameWindow.Components.Miscellaneous;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GameWindow.Components.UIElements
{
    /// <summary>
    /// The base class for all rendered images
    /// </summary>
    public partial class Sprite : Image
    {
        private static readonly BitmapImage MissingSprite = new BitmapImage(new Uri(@"pack://application:,,,/Resources\Images\MissingSprite.png"));

        public Transform transform;

        /// <summary>
        /// Gets a <see cref="BitmapImage"/> from a path
        /// </summary>
        /// <param name="path"> The path to the image location </param>
        /// <returns> If the path is legitimate, it returns the requested image, otherwise; returns the default texture </returns>
        public static BitmapImage BitmapFromPath(string path)
        {
            try
            {
                BitmapImage myBitmapImage = null!;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    myBitmapImage = new BitmapImage();
                    myBitmapImage.BeginInit();
                    myBitmapImage.UriSource = new Uri("pack://application:,,,/" + path);
                    myBitmapImage.EndInit();
                });
                return myBitmapImage;
            }
            catch
            {
                return MissingSprite;
            }
        }

        /// <summary>
        /// Builds a sprite UI element with an image
        /// </summary>
        /// <param name="transform"> The <see cref="Transform"/> to link </param>
        /// <param name="imagePath"> Path to the image </param>
        public Sprite(Transform transform, string imagePath) // Called within an STA thread
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

            SetValue(Panel.ZIndexProperty, 1);
        }

        /// <summary>
        /// Sets the position according to the linked transform
        /// </summary>
        private void SetPosition()
        {
            Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                SetValue(Canvas.LeftProperty, (double)transform.CenteredPosition.X);
                SetValue(Canvas.TopProperty, (double)transform.CenteredPosition.Y);
            });
        }

        /// <summary>
        /// Sets the scale according to the linked transform
        /// </summary>
        private void SetScale()
        {
            Dispatcher.Invoke(() =>
            { // UI Objects need to be changed in an STA thread
                Width = transform.ActualScale.X;
                Height = transform.ActualScale.Y;
            });
        }

        /// <summary>
        /// Sets the visibility
        /// </summary>
        /// <param name="visible"> Whether it should be visible or not </param>
        public void Visible(bool visible)
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => Visibility = visible ? Visibility.Visible : Visibility.Hidden);
        }

        /// <summary>
        /// Changes the image 
        /// </summary>
        /// <param name="imagePath"> Path to new image </param>
        public void ChangeImage(string imagePath)
        {
            var bitmap = BitmapFromPath(imagePath);

            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => { bitmap.Freeze(); Source = bitmap; });
        }

        /// <summary>
        /// Changes the image 
        /// </summary>
        /// <param name="imagePath"> The new image </param>
        public void ChangeImage(BitmapImage bitmap)
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => { bitmap?.Freeze(); Source = bitmap; });
        }

        /// <summary>
        /// Disposes the <see cref="Sprite"/>
        /// </summary>
        public void Dispose()
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() => MainWindow.instance!.CenteredCanvas.Children.Remove(this));
        }
    }
}