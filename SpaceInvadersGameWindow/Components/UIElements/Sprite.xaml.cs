using GameWindow.Components.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GameWindow.Components.UIElements
{
    /// <summary>
    /// Represents the types of available images in the game
    /// </summary>
    public enum Image
    {
        Empty,
        MissingSprite,

        BulletExplosion,
        Bullet,

        Charge1,
        Charge2,
        Charge3,
        Charge4,

        Imperfect1,
        Imperfect2,
        Imperfect3,
        Imperfect4,

        ZigZag1,
        ZigZag2,
        ZigZag3,
        ZigZag4,

        Player,
        PlayerDeath1,
        PlayerDeath2,

        OpponentPlayer,
        OpponentPlayerDeath1,
        OpponentPlayerDeath2,

        Green,
        Red,

        Crab1,
        Crab2,
        Octopus1,
        Octopus2,
        Squid1,
        Squid2,
        UFO,
        InvaderDeath,

        #region Bunker parts
        BottomLeft1,
        BottomLeft2,
        BottomLeft3,
        BottomLeft4,

        BottomRight1,
        BottomRight2,
        BottomRight3,
        BottomRight4,

        MiddleBottomLeft1,
        MiddleBottomLeft2,
        MiddleBottomLeft3,
        MiddleBottomLeft4,

        MiddleBottomRight1,
        MiddleBottomRight2,
        MiddleBottomRight3,
        MiddleBottomRight4,

        MiddleTopLeft1,
        MiddleTopLeft2,
        MiddleTopLeft3,
        MiddleTopLeft4,

        MiddleTopRight1,
        MiddleTopRight2,
        MiddleTopRight3,
        MiddleTopRight4,

        TopLeft1,
        TopLeft2,
        TopLeft3,
        TopLeft4,

        TopRight1,
        TopRight2,
        TopRight3,
        TopRight4,

        BottomLeft1Opponent,
        BottomLeft2Opponent,
        BottomLeft3Opponent,
        BottomLeft4Opponent,

        BottomRight1Opponent,
        BottomRight2Opponent,
        BottomRight3Opponent,
        BottomRight4Opponent,

        MiddleBottomLeft1Opponent,
        MiddleBottomLeft2Opponent,
        MiddleBottomLeft3Opponent,
        MiddleBottomLeft4Opponent,

        MiddleBottomRight1Opponent,
        MiddleBottomRight2Opponent,
        MiddleBottomRight3Opponent,
        MiddleBottomRight4Opponent,

        MiddleTopLeft1Opponent,
        MiddleTopLeft2Opponent,
        MiddleTopLeft3Opponent,
        MiddleTopLeft4Opponent,

        MiddleTopRight1Opponent,
        MiddleTopRight2Opponent,
        MiddleTopRight3Opponent,
        MiddleTopRight4Opponent,

        TopLeft1Opponent,
        TopLeft2Opponent,
        TopLeft3Opponent,
        TopLeft4Opponent,

        TopRight1Opponent,
        TopRight2Opponent,
        TopRight3Opponent,
        TopRight4Opponent,
        #endregion
    }

    /// <summary>
    /// The base class for all rendered images
    /// </summary>
    public partial class Sprite : System.Windows.Controls.Image
    {
        private static readonly Dictionary<Image, BitmapImage> Images = new Dictionary<Image, BitmapImage>();

        /// <summary>
        /// Buffers all images
        /// </summary>
        static Sprite()
        {
            for (int i = 0; i < Enum.GetNames(typeof(Image)).Length; i++)
                LoadImage((Image)i);
        }

        /// <summary>
        /// Buffers an image
        /// </summary>
        /// <param name="toLoad"> Image to load </param>
        private static void LoadImage(Image toLoad) =>
            Images.Add(toLoad, new BitmapImage(new Uri($@"pack://application:,,,/Resources\Images\{toLoad}.png")));

        public Transform transform;

        /// <summary>
        /// Builds a sprite UI element with an image
        /// </summary>
        /// <param name="transform"> The <see cref="Transform"/> to link </param>
        /// <param name="imagePath"> Path to the image </param>
        public Sprite(Transform transform, Image image) // Called within an STA thread
        {
            InitializeComponent();

            // link to Transform
            this.transform = transform;
            transform.PositionChanged += SetPosition;
            transform.ScaleChanged += SetScale;

            System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.NearestNeighbor);

            // set source image
            ChangeImage(image);

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
        /// <param name="imagePath"> The new image </param>
        public void ChangeImage(Image changeTo)
        {
            // UI Objects need to be changed in an STA thread
            Dispatcher.Invoke(() =>
            {
                BitmapImage bitmap;
                if (Images.ContainsKey(changeTo))
                    bitmap = Images[changeTo];
                else
                    bitmap = Images[Image.MissingSprite];

                bitmap?.Freeze();
                Source = bitmap;
            });
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